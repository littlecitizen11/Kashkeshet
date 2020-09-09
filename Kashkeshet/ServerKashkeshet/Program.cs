using Microsoft.Extensions.Configuration;
using Serilog;
using System;

namespace ServerKashkeshet
{
    class Program
    {
        static void Main(string[] args)
        {
            Server s = new Server();
            IConfiguration configSerilog = new ConfigurationBuilder()
.AddJsonFile("appsettings.json", true, true)
.Build();


            ILogger logger= new LoggerConfiguration()
                 .ReadFrom.Configuration(configSerilog)
                          .CreateLogger();

            s.StartServer(logger);
        }
    }
}
