using BLL;
using DAL;
using Microsoft.EntityFrameworkCore;
using WebApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var dbDirectory = FilesystemHelpers.GetDbDirectory() + Path.DirectorySeparatorChar;

connectionString = connectionString.Replace("<db_file>", $"{dbDirectory}app.db");

// dbContext - scoped (once for web request)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IRepository<GameConfiguration>, ConfigRepositoryEf>();
builder.Services.AddScoped<IRepository<GameBrain>, GameRepositoryEf>();

//builder.Services.AddScoped<IRepository<GameConfiguration>, ConfigRepositoryJson>();
//builder.Services.AddScoped<IRepository<GameBrain>, GameRepositoryJson>();

// Add SignalR services
builder.Services.AddSignalR();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

// Add SignalR hub mapping
app.MapHub<GameHub>("/gameHub");

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();