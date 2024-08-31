using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PUStatsCommon;
using System.Text.Json;
using PRUNStatsCommon;
using PRUNStatsSynchronizer;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var serviceContainer = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
    .AddDbContext<StatsContext>(
    options => options
        .UseSqlServer(configuration.GetConnectionString("StatsContext")))
    .AddHttpClient();
var serviceProvider = serviceContainer.BuildServiceProvider();

var syncer = ActivatorUtilities.CreateInstance<FIOSynchronizer>(serviceProvider);
await syncer.SynchronizeAsync();