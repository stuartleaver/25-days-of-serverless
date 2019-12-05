using System;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NaughtyOrNice.Entities;

namespace NaughtyOrNice.Functions
{
    public static class AnalyseLetterForSentiment
    {
        private static readonly string _subscriptionKey = Environment.GetEnvironmentVariable("TEXT_ANALYTICS_SUBSCRIPTION_KEY");

        private static readonly string _endPoint = Environment.GetEnvironmentVariable("TEXT_ANALYTICS_ENDPOINT");

        [FunctionName("AnalyseLetterForSentiment")]
        public static async Task RunAsync(
            [QueueTrigger("naughty-or-nice-translated-letters", Connection = "AzureWebJobsStorage")] TranslatedLetter letter,
            [Table("NaughtyOrNiceAnaylysedLetters", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function to analyse letters for sentiment");

            var sentimentScore = SentimentPredict(letter);

            var analysedLetter = new AnalysedLetter(letter.Who, letter.Message, letter.FromLanguage, letter.ToLanguage, letter.TranslationConfidenceScore, letter.TranslatedMessage, sentimentScore);

            var operation = TableOperation.Insert(analysedLetter);

            try
            {
                await table.ExecuteAsync(operation);
            }
            catch (StorageException ex)
            {
                log.LogError($"An error occured while trying to store the analysed letter - {ex.InnerException.ToString()}");
            }
        }

        public static double SentimentPredict(TranslatedLetter letter)
        {
            ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(_subscriptionKey);
            TextAnalyticsClient client = new TextAnalyticsClient(credentials)
            {
                Endpoint = _endPoint
            };

            var result = client.Sentiment(letter.TranslatedMessage, letter.ToLanguage);
            Console.WriteLine($"Sentiment Score: {result.Score:0.00}");

            return result.Score ?? 0;
        }
    }
}