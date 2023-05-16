using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using QoDL.DataAnnotations.Extensions;
using System.Text.Json;

namespace DevTesting.NetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            QoDLDataAnnotationsGlobalConfig.DefaultSerializerSettings = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
