using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Markdig;
using System.Net;
using System.Net.Http;
using System.Text;
using Octokit;
using System.Linq;

namespace Christmas.Card
{
    public static class ChristmasCard
    {
        [FunctionName("ChristmasCard")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string card = req.Query["card"];

            var cache = lazyConnection.Value.GetDatabase();

            var html = string.Empty;

            var cachedCard = cache.StringGet(card).ToString();

            if(!string.IsNullOrEmpty(cachedCard))
            {
                html = cachedCard;
            }
            else
            {
                var gist = await GetGist(card);

                html = Markdown.ToHtml(gist);

                cache.StringSet(card, html);
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(html, Encoding.UTF8, "text/html")
            };
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = Environment.GetEnvironmentVariable("RedisConnectionString");
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        private static async Task<string> GetGist(string gistId)
        {
            var oktokit = new GitHubClient(new ProductHeaderValue("TestGitHutAPI"));

            var gist = await oktokit.Gist.Get(gistId);

            return gist.Files[gist.Files.Keys.ToList()[0]].Content;
        }
    }
}
