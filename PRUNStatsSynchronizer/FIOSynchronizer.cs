using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PRUNStatsCommon;
using PRUNStatsCommon.Bases;
using PRUNStatsCommon.Companies;
using PRUNStatsCommon.Corporations;
using PRUNStatsCommon.Planets;
using PRUNStatsCommon.Users;

namespace PRUNStatsSynchronizer
{
    public class FIOSynchronizer(IConfiguration _configuration, IHttpClientFactory _httpClientFactory, StatsContext _statsContext)
    {
        public async Task SynchronizeAsync()
        {
            Console.WriteLine("Fetching companies from FIORest...");
            var companyDtos = await GetCompanyDTOsAsync();

            //get UTC now
            var now = DateTime.UtcNow;

            Console.WriteLine("Parsing DTOs...");
            //now deconstruct the DTOs
            foreach (var companyDto in companyDtos)
            {
                //handle user create/update
                var user = await _statsContext.Users.FirstOrDefaultAsync(u => u.PRGUID == companyDto.UserId) ?? new UserModel
                {
                    Username = companyDto.UserName,
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
                                          Corporation = corporation,
                                          Faction = faction,
                                          User = user,
                                          FirstImportedAtUTC = now,
                                          LastUpdatedAtUTC = now
                                      };

                //handle planet create/update
                foreach (var planetDto in companyDto.Planets)
                {
                    var planet = await _statsContext.Planets.FirstOrDefaultAsync(p => p.PRGUID == planetDto.PlanetId) ?? new PlanetModel
                    {
                        Name = planetDto.PlanetName,
                        NaturalId = planetDto.PlanetNaturalId,
                        FirstImportedAtUTC = now,
                        LastUpdatedAtUTC = now
                    };

                    //now that we got the planet, let's handle the base
                    var planetBase = await _statsContext.Bases
                        .FirstOrDefaultAsync(b => b.Company == company && b.Planet == planet)
                    ?? new BaseModel
                    {
                        Planet = planet,
                        Company = company,
                        FirstImportedAtUTC = now,
                        LastUpdatedAtUTC = now
                    };
                }

                user.LastUpdatedAtUTC = now;
                if (corporation is not null) corporation.LastUpdatedAtUTC = now;
                company.LastUpdatedAtUTC = now;
                foreach(var b in company.Bases)
                {
                    b.LastUpdatedAtUTC = now;
                    b.Planet.LastUpdatedAtUTC = now;
                }

                _statsContext.Update(user);
            }

            Console.WriteLine("Saving changes to db...");
            //now save changes
            await _statsContext.SaveChangesAsync();

            Console.WriteLine("Done!");
        }

        private async Task<List<CompanyDto>> GetCompanyDTOsAsync()
        {
            //TEMP CODE UNTIL THE /COMPANY/ALL ENDPOINT IS IMPLEMENTED IN FIORest

            //get all codes for 26 letters
            var twoCodes = new List<string>(26 * 26);
            for (var i = 0; i < 26; i++)
            {
                for (var j = 0; j < 26; j++)
                {
                    twoCodes.Add(((char)('A' + i)).ToString() + ((char)('A' + j)).ToString());
                }
            }

            var companies = new List<CompanyDto>();

            //request data for each code from FIO api
            await Parallel.ForEachAsync(twoCodes, new ParallelOptions { MaxDegreeOfParallelism = 100 }, async (code, token) =>
            {
                var response = await _httpClientFactory.CreateClient().GetAsync($"https://rest.fnar.net/company/code/{code}", token);

                if (!response.IsSuccessStatusCode) return;

                //get stream
                var stream = await response.Content.ReadAsStreamAsync(token);

                if (stream.Length == 0) return;

                //deserialize
                var company = await JsonSerializer.DeserializeAsync<CompanyDto>(stream, cancellationToken: token);

                companies.Add(company);
            });

            return companies;
        }
    }
}
