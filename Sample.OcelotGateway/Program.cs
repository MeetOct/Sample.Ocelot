using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Sample.OcelotGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(context.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName.ToLower()}.json", true, true)
                        .AddJsonFile("configuration.json", true, true)
                        .AddJsonFile($"configuration.{context.HostingEnvironment.EnvironmentName.ToLower()}.json", true,true);
                })
                .ConfigureServices(services =>
                {
//                    var authProviderKey = "TestKey";
                    services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme).AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = "http://localhost:59381";
                        options.ApiName = "api";
                        options.RequireHttpsMetadata = false;
                        options.ApiSecret = "secret";
                    });
                    services.AddOcelot();
                })
                .ConfigureLogging((context, logging) => { logging.AddConsole(); })
                .Configure(async app => { await app.UseOcelot(); })
//                .UseStartup<Startup>()
                .Build();
        }
    }
}