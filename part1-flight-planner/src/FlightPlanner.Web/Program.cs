using FlightPlanner.Application.Calculators;
using FlightPlanner.Application.Services;
using FlightPlanner.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var sqliteConnectionString = builder.Configuration.GetConnectionString("FlightPlannerDb")
    ?? throw new InvalidOperationException("Connection string 'FlightPlannerDb' was not found in configuration.");
builder.Services.AddFlightPlannerInfrastructure(sqliteConnectionString);

builder.Services.AddSingleton<IDistanceCalculator, HaversineDistanceCalculator>();
builder.Services.AddSingleton<IFuelCalculator, FuelCalculator>();
builder.Services.AddScoped<IFlightService, FlightService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FlightPlannerDbContext>();
    await dbContext.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.SeedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Flights}/{action=Index}/{id?}");

app.Run();
