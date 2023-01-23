using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using DinkToPdf;
using DinkToPdf.Contracts;

[assembly: FunctionsStartup(typeof(HtmlToPdfFunction.Startup))]

namespace HtmlToPdfFunction
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            // Add converter to DI
            builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
           
        }
    }
}
