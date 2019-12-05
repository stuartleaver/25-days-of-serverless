using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NaughtyOrNiceMvc.Entities;
using NaughtyOrNiceMvc.Models;

namespace NaughtyOrNiceMvc.Controllers
{
    public class ListController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;

        public ListController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;

            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            return View(await GetLettersAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<List<Letter>> GetLettersAsync()
        {
            var storageAccount = CloudStorageAccount.Parse(_configuration.GetValue<string>("StorageAccount"));
            var client = storageAccount.CreateCloudTableClient();
            var table = client.GetTableReference("NaughtyOrNiceAnaylysedLetters");

            TableContinuationToken token = null;

            var query = new TableQuery<AnalysedLetter>();

            var letters = new List<Letter>();

            do
            {
                var analysedLetters = await table.ExecuteQuerySegmentedAsync(query, token);

                token = analysedLetters.ContinuationToken;

                foreach(var analysedLetter in analysedLetters)
                {
                    letters.Add(new Letter {
                        Who = analysedLetter.Who,
                        Message = analysedLetter.Message,
                        TranslatedMessage = analysedLetter.TranslatedMessage,
                        Sentiment = analysedLetter.Sentiment
                    });
                }

            } while (token != null);

            return letters;
        }
    }
}
