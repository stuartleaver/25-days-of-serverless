using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System.Text;

namespace Christmas.PosadasApi
{
    public static class PosadasFinder
    {
        [FunctionName("PosadasFinder")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var path = Path.Combine(context.FunctionAppDirectory, "posadas.json");
            var posadaLocations = File.ReadAllText(path);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(posadaLocations, Encoding.UTF8, "application/json")
            };
        }
    }
}