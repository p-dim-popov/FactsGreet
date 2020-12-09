namespace FactsGreet.Web
{
    using System;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.Kestrel.Https;
    using Microsoft.Extensions.Hosting;

    public static class Program
    {
        public static string Port => Environment.GetEnvironmentVariable("PORT") ?? "5001";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel()
                        .UseStartup<Startup>()
                        // .UseUrls($"http://+.:{Port}")
                            ;
                });
    }
}
