using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;

namespace cnrun.dchwebcamdownload
{
    public class WebCamDCHDownload
    {
        private static string StartWestUrl="http://dc-h.spdns.de:8080/record/current.jpg";
        private static string StartNordUrl="http://dc-h-1.spdns.de:8080/record/current.jpg";
        private static string GebietWestUrl="http://dc-h.spdns.de:8081/record/current.jpg";
        private static string BurgNordUrl="https://www.dc-hohenneuffen.de/cam/armin/hohenneuffen-vga.jpg";
        private static string GrabestettenUrl="http://www.grabenstetten.info/webcam/grabicam.jpg";
        [FunctionName("WebCamDCHDownload")]
        public async Task Run([TimerTrigger("0 */1 6-17 * * *")] TimerInfo myTimer,
                              [Blob("webcam/start-west/sw-{DateTime}.jpg", FileAccess.Write)] Stream StartWestStream,
                              [Blob("webcam/start-nord/sn-{DateTime}.jpg", FileAccess.Write)] Stream StartNordStream,
                              [Blob("webcam/gebiet-west/gw-{DateTime}.jpg", FileAccess.Write)] Stream GebietWestStream,
                              [Blob("webcam/burg-nord/bn-{DateTime}.jpg", FileAccess.Write)] Stream BurgNordStream,
                              [Blob("webcam/grabenstetten/gr-{DateTime}.jpg", FileAccess.Write)] Stream GrabenstettenStream,
                              ILogger log
                              )
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(15);
            var tasks = new []{
                (await client.GetAsync(StartWestUrl)).Content.CopyToAsync(StartWestStream),
                (await client.GetAsync(StartNordUrl)).Content.CopyToAsync(StartNordStream),
                (await client.GetAsync(GebietWestUrl)).Content.CopyToAsync(GebietWestStream),
                (await client.GetAsync(BurgNordUrl)).Content.CopyToAsync(BurgNordStream),
                (await client.GetAsync(GrabestettenUrl)).Content.CopyToAsync(GrabenstettenStream)
            };
            Task t = Task.WhenAll(tasks);
            try {
                t.Wait();
            }
            catch {}   

            if (t.Status == TaskStatus.RanToCompletion)
                log.LogInformation("all images done.");
            else if (t.Status == TaskStatus.Faulted)
                log.LogError("can't upload save all images");     
        }

        // Write to Blob:
        // https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob-output?tabs=csharp
        // https://docs.microsoft.com/en-us/azure/developer/javascript/how-to/with-web-app/azure-function-file-upload
        // https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob#output
        // https://www.cyotek.com/blog/upload-data-to-blob-storage-with-azure-functions
        // https://betterprogramming.pub/timertrigger-azure-function-to-upload-to-azure-blob-daily-c4e761a8ee4c
        // https://docs.microsoft.com/de-de/azure/azure-functions/storage-considerations
        // https://docs.microsoft.com/en-us/azure/azure-functions/functions-add-output-binding-storage-queue-vs-code?tabs=in-process&pivots=programming-language-csharp
        // https://www.c-sharpcorner.com/article/create-container-and-upload-blob-using-azure-function-in-net-core/
        // https://github.com/jurajsucik/azure-functions-file-upload/blob/master/FileUploadHttpTriggerv2.cs
        // --> https://docs.microsoft.com/de-de/azure/storage/blobs/storage-quickstart-blobs-dotnet-legacy
    }
}
