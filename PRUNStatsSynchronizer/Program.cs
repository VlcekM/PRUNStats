using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PRUNStatsCommon;
using PRUNStatsSynchronizer;
using System.Reflection;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables("PRUNStatsSynchronizer__")
    .AddUserSecrets<Program>()
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