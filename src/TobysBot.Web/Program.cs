using TobysBot.Configuration;
using TobysBot.Misc.Configuration;
using TobysBot.Mongo.Configuration;
using TobysBot.Music.Configuration;
using TobysBot.Voice.Configuration;
using TobysBot.Web.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables("TOBYSBOT_");

builder.Configuration.AddJsonFile("secrets.json", true);
builder.Configuration.AddJsonFile($"secrets.{builder.Environment.EnvironmentName}.json", true);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddTobysBot(builder.Configuration)
    .AddHostingService<WebHostingService>()
    .AddVoiceModule(builder.Configuration.GetSection("Voice"))
    .AddMusicModule(builder.Configuration.GetSection("Music"))
    .AddMiscModule(builder.Configuration.GetSection("Star"))
    .AddMongoDatabase(builder.Configuration.GetSection("Mongo"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();