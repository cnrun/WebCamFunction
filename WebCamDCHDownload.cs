using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace cnrun.dchwebcamdownload
{
    public class WebCamDCHDownload
    {
        [FunctionName("WebCamDCHDownload")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
                        ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executing at: {DateTime.Now}");
            var imageUrl="http://dc-h.spdns.de:8080/record/current.jpg";
            // var currentFileName="current.jpg";
            DateTime now = DateTime.Now;
            var fileName=Path.Combine(Path.GetTempPath(), $"cam-west{now.Year}{now.Month}{now.Day}-{now.Hour}{now.Minute}.jpg");
            HttpClient client = new HttpClient();
            log.LogInformation($"query: {imageUrl}");
            var response = await client.GetAsync(imageUrl);
            using (var fs = new FileStream(fileName, FileMode.CreateNew))
            {
                log.LogInformation($"save in : {fileName}");
                await response.Content.CopyToAsync(fs);
                log.LogInformation($"{new FileInfo(fileName).Length} bytes saved");
            }
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
