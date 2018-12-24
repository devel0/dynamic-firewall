using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace dynamic_firewall
{

    public class Program
    {
        public static void Main(string[] args)
        {
            if (!File.Exists(Config.Pathfilename))
            {
                System.Console.WriteLine($"can't find [{Config.Pathfilename}]");
                Environment.Exit(1);
            }

            // ensure token manager starts
            var global = Global.Instance;

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseUrls("http://192.168.1.254:5000")
                .UseStartup<Startup>();
    }
}
