using System;
using System.Threading.Tasks;
using Christmas.Sweden.Entities;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Christmas.Sweden
{
    public static class TweetSentimentAnalyser
    {
        private static KeyVaultClient _kv;

        private static string _kvEndpoint;

        [FunctionName("TweetSentimentAnalyser")]
        public static async Task Run(
            [QueueTrigger("translated-tweets", Connection = "AzureWebJobsStorage")] AnalysedTweet tweet,
            [Table("processed-tweets", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            // Declares a KeyVaultClient instance.
            var _azureServiceTokenProvider = new AzureServiceTokenProvider();
            _kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(_azureServiceTokenProvider.KeyVaultTokenCallback));

            _kvEndpoint = Environment.GetEnvironmentVariable("KEY_VAULT_ENDPOINT");

            tweet.Sentiment = await SentimentPredict(tweet.TranslatedText);

            var operation = TableOperation.Insert(tweet);

            try
            {
                await table.ExecuteAsync(operation);
            }
            catch (StorageException ex)
            {
                log.LogError($"An error occured while trying to store the analysed letter - {ex.InnerException.ToString()}");
            }
        }

        public static async Task<double> SentimentPredict(string text)
        {
            var subscriptionKey = await _kv.GetSecretAsync(_kvEndpoint, "SENTIMENT_ANALYSER_SUBSCRIPTION_KEY");

            var textAnalyticsClientEndpoint = await _kv.GetSecretAsync(_kvEndpoint, "TEXT_ANALYTICS_CLIENT_ENDPOINT");

            ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(subscriptionKey.Value);
            TextAnalyticsClient client = new TextAnalyticsClient(credentials)
            {
                Endpoint = textAnalyticsClientEndpoint.Value
            };

            var result = client.Sentiment(text);
            Console.WriteLine($"Sentiment Score: {result.Score:0.00}");

            return result.Score ?? 0;
        }
    }
}