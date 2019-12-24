using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Christmas.Sweden.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Christmas.Sweden.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;

            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var model = await GetTweetsToDisplay();

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<List<TweetModel>> GetTweetsToDisplay()
        {
            var tweetsModel = new List<TweetModel>();

            var storageAccount = CloudStorageAccount.Parse(_configuration["TWEETS-STORAGE-ACCOUNT-CONNECTION-STRING"]);

            var client = storageAccount.CreateCloudTableClient();

            var table = client.GetTableReference("processedtweets");

            var query = new TableQuery<TweetModel>();

            TableContinuationToken token = null;

            do
            {
                var tweets = await table.ExecuteQuerySegmentedAsync(query, token);

                token = tweets.ContinuationToken;

                foreach(var tweet in tweets)
                {
                    tweetsModel.Add(tweet);
                }
            }
            while (token != null);

            return tweetsModel;
        }
    }
}
