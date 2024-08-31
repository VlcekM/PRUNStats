using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using PRUNStatsApp.Components;
using PRUNStatsCommon;
using PRUNStatsCommon.Companies.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<StatsContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("StatsContext"));
});
builder.Services.AddTransient<CompanyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
