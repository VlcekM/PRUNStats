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
            Console.WriteLine("Fetching companies from FIORest...");
            var companyDtos = await GetCompanyDTOsAsync();

            //get UTC now
            var now = DateTime.UtcNow;

            Console.WriteLine("Parsing DTOs...");
            //now deconstruct the DTOs

            var progress = 0;
            Console.WriteLine("Parsing planets...");
            var planets = companyDtos
                .SelectMany(c => c.Planets ?? [])
                .DistinctBy(c => c.PlanetId)
                .ToList();

            foreach (var planetDto in planets)
            {
                progress++;
                Console.WriteLine($"Parsing planet {progress} / {planets.Count} ({planetDto.PlanetNaturalId})");

                var planet = await _statsContext.Planets.FirstOrDefaultAsync(p => p.PRGUID == planetDto.PlanetId) ?? new PlanetModel
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

            progress = 0;
            foreach (var companyDto in companyDtos)
            {
                progress++;
                Console.WriteLine($"Parsing company {progress} / {companyDtos.Count} ({companyDto.CompanyCode})");

                if (string.IsNullOrWhiteSpace(companyDto.UserName)) continue; //skip companies without a user
                if (string.IsNullOrWhiteSpace(companyDto.CompanyName)) continue; //skip companies without a name
                if (string.IsNullOrWhiteSpace(companyDto.CompanyCode)) continue; //skip companies without a name

                //handle user create/update
                var user = await _statsContext.Users.FirstOrDefaultAsync(u => u.PRGUID == companyDto.UserId) ?? new UserModel
                {
                    Username = companyDto.UserName,
                    PRGUID = companyDto.UserId,
                    FirstImportedAtUTC = now,
                    LastUpdatedAtUTC = now
                };
                user.LastUpdatedAtUTC = now;

                //handle corporation create/update
                var corporation = await _statsContext.Corporations.FirstOrDefaultAsync(c => c.PRGUID == companyDto.CorporationId);
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
                var company = await _statsContext.Companies.FirstOrDefaultAsync(c => c.PRGUID == companyDto.CompanyId) ??
                                      //if it does not exist, create it
                                      new CompanyModel
                                      {
                                          CompanyCode = companyDto.CompanyCode,
                                          CompanyName = companyDto.CompanyName,
                                          PRGUID = companyDto.CompanyId,
                                          Corporation = corporation,
                                          Faction = faction,
                                          User = user,
                                          FirstImportedAtUTC = now,
                                          LastUpdatedAtUTC = now
                                      };

                user.LastUpdatedAtUTC = now;
                if (corporation is not null) corporation.LastUpdatedAtUTC = now;
                company.LastUpdatedAtUTC = now;

                _statsContext.Companies.Update(company);
            }

            Console.WriteLine("Saving companies to db...");
            await _statsContext.SaveChangesAsync();

            Console.WriteLine("Parsing bases...");

            progress = 0;
            foreach (var cDto in companyDtos)
            {
                progress++;
                Console.WriteLine($"Parsing company bases {progress} / {companyDtos.Count} ({cDto.CompanyCode})");
                //for each base
                foreach (var b in cDto.Planets)
                {
                    //get planet
                    var planet = await _statsContext.Planets.FirstOrDefaultAsync(p => p.PRGUID == b.PlanetId);

                    //get company
                    var company = await _statsContext.Companies.FirstOrDefaultAsync(c => c.PRGUID == cDto.CompanyId);

                    if (planet is null || company is null) continue; //skip if planet or company is not found (should not happen)

                    //handle base create/update
                    var baseModel = await _statsContext.Bases
                        .FirstOrDefaultAsync(b => b.Planet.PRGUID == planet.PRGUID && b.Company.PRGUID == company.PRGUID)
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
