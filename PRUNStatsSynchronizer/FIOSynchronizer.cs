using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PRUNStatsCommon;
using PRUNStatsCommon.Bases;
using PRUNStatsCommon.Companies.DTOs;
using PRUNStatsCommon.Companies.Models;
using PRUNStatsCommon.Corporations;
using PRUNStatsCommon.Planets;
using PRUNStatsCommon.Users;

namespace PRUNStatsSynchronizer
{
    public class FIOSynchronizer(IConfiguration _configuration, IHttpClientFactory _httpClientFactory, StatsContext _statsContext)
    {
        public async Task SynchronizeAsync()
        {
            await SynchronizeCompaniesAsync();
            Console.WriteLine("Done!");
        }

        private async Task<List<CompanyDto>> GetCompanyDTOsAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["FIOAPIURL"]!);

            return await client.GetFromJsonAsync<List<CompanyDto>>("/company/all") ?? [];
        }

        /// <summary>
        /// Syncs planets, companies, users, corporations and bases from FIORest
        /// </summary>
        private async Task SynchronizeCompaniesAsync()
        {
            Console.WriteLine("Fetching companies from FIO...");
            var companyDtos = await GetCompanyDTOsAsync();

            //get UTC now
            var now = DateTime.UtcNow;

            Console.WriteLine("Parsing DTOs...");
            //now deconstruct the DTOs

            Console.WriteLine("Parsing planets...");
            var planets = companyDtos
                .SelectMany(c => c.Planets ?? [])
                .DistinctBy(c => c.PlanetId)
                .ToList();

            var allPlanets = await _statsContext.Planets.ToListAsync();

            foreach (var planetDto in planets)
            {
                var planet = allPlanets.FirstOrDefault(p => p.PRGUID == planetDto.PlanetId) ?? new PlanetModel
                {
                    Name = planetDto.PlanetName,
                    NaturalId = planetDto.PlanetNaturalId,
                    PRGUID = planetDto.PlanetId,
                    FirstImportedAtUTC = now,
                    LastUpdatedAtUTC = now
                };

                _statsContext.Update(planet);
            }

            Console.WriteLine("Saving planets to db...");
            await _statsContext.SaveChangesAsync();

            var allCompanies = await _statsContext.Companies.ToListAsync();
            var allUsers = await _statsContext.Users.ToListAsync();
            var allCorporations = await _statsContext.Corporations.ToListAsync();

            var skipped = 0;
            foreach (var companyDto in companyDtos)
            {

                if (string.IsNullOrWhiteSpace(companyDto.UserName)) continue; //skip companies without a user
                if (string.IsNullOrWhiteSpace(companyDto.CompanyName)) continue; //skip companies without a name
                if (string.IsNullOrWhiteSpace(companyDto.CompanyCode)) continue; //skip companies without a name

                //check if the timestamp is after the last update
                var company = allCompanies.FirstOrDefault(c => c.PRGUID == companyDto.CompanyId);
                if (companyDto.Timestamp <= company?.LastUpdatedFIO)
                {
                    skipped++;
                    continue; //skip if the company is already up to date
                }

                //handle user create/update
                var user = allUsers.FirstOrDefault(u => u.PRGUID == companyDto.UserId) ?? new UserModel
                {
                    Username = companyDto.UserName,
                    PRGUID = companyDto.UserId,
                    FirstImportedAtUTC = now,
                    LastUpdatedAtUTC = now,
                };
                user.LastUpdatedAtUTC = now;

                //handle corporation create/update
                var corporation = allCorporations.FirstOrDefault(c => c.PRGUID == companyDto.CorporationId);
                if (corporation is null && companyDto.CorporationId is not null)
                {
                    corporation = new CorporationModel
                    {
                        CorporationCode = companyDto.CorporationCode!,
                        CorporationName = companyDto.CorporationName!,
                        PRGUID = companyDto.CorporationId!,
                        FirstImportedAtUTC = now,
                        LastUpdatedAtUTC = now
                    };
                    corporation.LastUpdatedAtUTC = now;
                }

                Faction? faction = companyDto.CountryCode switch
                {
                    "AI" => Faction.AntaresInitiative,
                    "CI" => Faction.CastilloItoMercantile,
                    "EC" => Faction.ExodusCouncil,
                    "IC" => Faction.InsitorCooperative,
                    "NC" => Faction.NEOCharterExploration,
                    _ => null
                };

                //handle company create/update
                company ??= new CompanyModel
                            {
                                CompanyCode = companyDto.CompanyCode,
                                CompanyName = companyDto.CompanyName,
                                PRGUID = companyDto.CompanyId,
                                Corporation = corporation,
                                Faction = faction,
                                User = user,
                                FirstImportedAtUTC = now,
                                LastUpdatedAtUTC = now,
                                LastUpdatedFIO = companyDto.Timestamp,
                            };

                user.LastUpdatedAtUTC = now;
                if (corporation is not null) corporation.LastUpdatedAtUTC = now;
                company.LastUpdatedAtUTC = now;
                company.LastUpdatedFIO = companyDto.Timestamp;

                //handle epoch time conversion
                DateTime? createdAt = DateTime.UnixEpoch.AddMilliseconds(companyDto.CreatedEpochMs);
                if (createdAt?.Year == 1970) createdAt = null;
                company.CreatedAt = createdAt;

                _statsContext.Companies.Update(company);
            }

            Console.WriteLine($"Skipped {skipped} companies due to them being up-to-date");
            Console.WriteLine("Saving companies to db...");
            await _statsContext.SaveChangesAsync();

            Console.WriteLine("Parsing bases...");

            allPlanets = await _statsContext.Planets
                .Include(p => p.Bases)
                .ToListAsync();
            allCompanies = await _statsContext.Companies
                .Include(c => c.Bases)
                .ToListAsync();
            var allBases = await _statsContext.Bases
                .Include(b => b.Planet)
                .ToListAsync();

            foreach (var cDto in companyDtos)
            {
                //Console.WriteLine($"Parsing company bases {progress} / {companyDtos.Count} ({cDto.CompanyCode})");
                //for each base
                foreach (var b in cDto.Planets)
                {
                    //get planet
                    var planet = allPlanets.FirstOrDefault(p => p.PRGUID == b.PlanetId);

                    //get company
                    var company = allCompanies.FirstOrDefault(c => c.PRGUID == cDto.CompanyId);

                    if (planet is null || company is null) continue; //skip if planet or company is not found (should not happen)

                    //handle base create/update
                    var baseModel = allBases
                        .FirstOrDefault(x => x.Planet.PRGUID == planet.PRGUID && x.Company.PRGUID == company.PRGUID)
                    ??
                    new BaseModel
                    {
                        Planet = planet,
                        Company = company,
                        FirstImportedAtUTC = now,
                        LastUpdatedAtUTC = now
                    };
                    _statsContext.Bases.Update(baseModel);
                }

            }

            Console.WriteLine("Saving bases to db...");
            await _statsContext.SaveChangesAsync();

        }
    }
}
