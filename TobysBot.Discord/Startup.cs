using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using TobysBot.Discord.Client.Configuration;

namespace TobysBot.Discord
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddHttpClient();

            var lavalinkConfig = Configuration.GetSection("Lavalink");

            services.AddDiscordClient(options =>
                {
                    options.Token = Configuration.GetSection("Discord")["Token"];
                    options.Prefix = Configuration.GetSection("Discord")["Prefix"];
                    options.TobyId = Configuration.GetSection("Discord").GetValue<ulong>("TobyId");
                })
                .AddLavaNode(options =>
                {
                    options.SelfDeaf = false;

                    options.Hostname = lavalinkConfig["Hostname"];
                    options.Port = lavalinkConfig.GetValue<ushort>("Port");
                    options.Authorization = lavalinkConfig["Authorization"];

                    options.LogSeverity = LogSeverity.Verbose;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
