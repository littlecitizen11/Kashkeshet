using Microsoft.Extensions.Configuration;
using Serilog;
using ServerKashkeshet.Server;

namespace ServerKashkeshet
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerKishKush s = new ServerKishKush();
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
