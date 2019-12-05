using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NaughtyOrNice.Entities;

namespace NaughtyOrNice.Functions
{
    public static class ReceiveLetter
    {
        [FunctionName("ReceiveLetter")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Queue("naughty-or-nice-received-letters")] IAsyncCollector<Letter> queue,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function to processed recieved letters.");

            var letter = new Letter();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Letter data = JsonConvert.DeserializeObject<Letter>(requestBody);

            if(!string.IsNullOrEmpty(data.Who) || !string.IsNullOrEmpty(data.Message))
            {
                await queue.AddAsync(data);

                return (ActionResult)new OkObjectResult($"Hello, {data.Who}. Thank you for your letter");
            }
            else
            {
                return new BadRequestObjectResult("Please pass a name and letter in the request body");
            }
        }
    }
}
