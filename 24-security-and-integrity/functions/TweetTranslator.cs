using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Christmas.Sweden.Entities;
using Christmas.Sweden.TranslationEntities;
using Microsoft.Azure.Documents;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Christmas.Sweden
{
    public static class TweetTranslator
    {
        private static KeyVaultClient _kv;

        private static string _kvEndpoint;

        [FunctionName("TweetTranslator")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "gavlebocken",
            collectionName: "tweets",
            ConnectionStringSetting = "CosmosDbConnectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input,
            [Queue("translated-tweets")] IAsyncCollector<AnalysedTweet> queue,
            ILogger log)
        {
            // Declares a KeyVaultClient instance.
            var _azureServiceTokenProvider = new AzureServiceTokenProvider();
            _kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(_azureServiceTokenProvider.KeyVaultTokenCallback));

            _kvEndpoint = Environment.GetEnvironmentVariable("KEY_VAULT_ENDPOINT");

            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);

                for (var i = 0; i < input.Count; i++)
                {
                    var tweet = new Tweet
                    {
                        TweetId = input[i].Id,
                        Text = input[i].GetPropertyValue<string>("text"),
                        Username = input[i].GetPropertyValue<string>("username"),
                        SentTimestamp = input[i].GetPropertyValue<string>("sentTimestamp"),
                    };

                    var analysedTweet = await Translate(tweet);

                    await queue.AddAsync(analysedTweet);
                }
            }
        }

        private static async Task<AnalysedTweet> Translate(Tweet tweet)
        {
            object[] body = new object[] { new { Text = tweet.Text } };
            var requestBody = JsonConvert.SerializeObject(body);

            var endPoint = await _kv.GetSecretAsync(_kvEndpoint, "TEXT-TRANSLATOR-ENDPOINT");
            var subscriptionKey = await _kv.GetSecretAsync(_kvEndpoint, "TEXT-TRANSLATOR-SUBSCRIPTION-KEY");

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                // Set the method to Post.
                request.Method = HttpMethod.Post;

                // Construct the URI and add headers.
                request.RequestUri = new Uri(endPoint.Value + "/translate?api-version=3.0&to=en");
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey.Value);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();

                // Deserialize the response using the classes created earlier.
                TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);

                // Print the detected input language and confidence score.
                Console.WriteLine("Detected input language: {0}\nConfidence score: {1}\n", deserializedOutput[0].DetectedLanguage.Language, deserializedOutput[0].DetectedLanguage.Score);

                Console.WriteLine("Translated to {0}: {1}", deserializedOutput[0].Translations[0].To, deserializedOutput[0].Translations[0].Text);

                return new AnalysedTweet(
                    tweet.TweetId,
                    tweet.Text,
                    deserializedOutput[0].Translations[0].Text,
                    deserializedOutput[0].DetectedLanguage.Language,
                    deserializedOutput[0].Translations[0].To,
                    tweet.Username,
                    tweet.SentTimestamp);
            }
        }
    }
}
