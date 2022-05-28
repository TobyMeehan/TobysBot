using TobysBot.Commands.Modules;
using TobysBot.Configuration;
using TobysBot.Voice.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables("TOBYSBOT_");

builder.Configuration.AddJsonFile("secrets.json", true);
builder.Configuration.AddJsonFile($"secrets.{builder.Environment.EnvironmentName}.json", true);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddTobysBot(builder.Configuration)
    .AddModule<PongModule>()
    .AddVoiceModule();

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

app.MapFallbackToFile("index.html");
;

app.Run();