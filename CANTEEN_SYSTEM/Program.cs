using CANTEEN_SYSTEM.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// AzureSql takes priority when it is configured. If it is empty,
// the app falls back to the local SQLite file for development.
var connectionString = builder.Configuration.GetConnectionString("AzureSql");
var fallbackConnection = builder.Configuration.GetConnectionString("LocalSqlite")
    ?? "Data Source=canteen.db";

builder.Services.AddDbContext<CanteenDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        options.UseAzureSql(connectionString);
    }
    else
    {
        options.UseSqlite(fallbackConnection);
    }
});

var app = builder.Build();

// Ensure the chosen database exists and has starter records.
await DbInitializer.InitializeAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
