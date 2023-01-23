using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Net.Http;

namespace HtmlToPdfFunction
{
    public  class Convert
    {
        private readonly HttpClient _client;
        private readonly IConverter _converter;


        public Convert(IHttpClientFactory httpClientFactory, IConverter converter)
        {
            this._client = httpClientFactory.CreateClient();
            this._converter = converter;
        }
        [FunctionName("Convert")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                //grab the incoming file
                var file = req.Form.Files["file"];
               // read it
                using var reader = new StreamReader(file.OpenReadStream());

                var fileString =    await reader.ReadToEndAsync();

                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Landscape,
                        PaperSize = PaperKind.A4Plus,
                    },
                                    Objects = {
                        new ObjectSettings() {
                            PagesCount = true,
                            HtmlContent = fileString,
                            WebSettings = { DefaultEncoding = "utf-8" },
                            //HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                        }
                    }
                };

                byte[] pdf = _converter.Convert(doc);

                log.LogInformation("Successfull PDF");

                return new FileContentResult(pdf, "application/pdf")
                {
                    FileDownloadName = "test.pdf"
                };
            }
            catch (Exception ex)
            {
                log.LogError(new EventId(),ex, "Error Creating PDF");
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
