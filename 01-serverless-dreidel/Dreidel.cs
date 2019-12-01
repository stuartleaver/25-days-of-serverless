using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Dreidel.Spinner.Services;

namespace Dreidel.Spinner
{
    public static class Dreidel
    {
        private static IDreidelService _dreidelService;

        [FunctionName("SpinDreidel")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dreidel/spin")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Happy Hanukkah - C# HTTP trigger function to spin a dreidel.");

            _dreidelService = new DreidelService();

            var _spinResult = await _dreidelService.Spin();

            return _spinResult != null
                ? (ActionResult)new OkObjectResult(_spinResult)
                : new BadRequestObjectResult("Opps, you dreidel failed to spin!");
        }
    }
}
