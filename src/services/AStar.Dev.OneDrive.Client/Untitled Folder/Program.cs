using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AspnetCore_Changed_Files
{
    public class Program
    {
#pragma warning disable ASPDEPR008
        public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();
#pragma warning restore ASPDEPR008

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
#pragma warning disable ASPDEPR008
            WebHost.CreateDefaultBuilder(args)
#pragma warning restore ASPDEPR008
                .UseStartup<Startup>();
    }
}
