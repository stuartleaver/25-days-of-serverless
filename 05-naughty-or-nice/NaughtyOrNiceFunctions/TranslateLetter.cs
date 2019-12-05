using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using NaughtyOrNice.Entities;
using NaughtyOrNice.TranslationEntities;
using Newtonsoft.Json;

namespace NaughtyOrNice.Functions
{
    public static class TranslateLetter
    {
        private static readonly string _subscriptionKey = Environment.GetEnvironmentVariable("TRANSLATOR_TEXT_SUBSCRIPTION_KEY");

        private static readonly string _endPoint = Environment.GetEnvironmentVariable("TRANSLATOR_TEXT_ENDPOINT");

        [FunctionName("TranslateLetter")]
        public static async Task RunAsync(
            [QueueTrigger("naughty-or-nice-received-letters", Connection = "AzureWebJobsStorage")] Letter letter,
            [Queue("naughty-or-nice-translated-letters")] IAsyncCollector<Letter> queue,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function to translater recieved letters");

            var translatedLetter = await Translate(letter);

            try
            {
                await queue.AddAsync(translatedLetter);
            }
            catch (StorageException ex)
            {
                log.LogError($"An error occured while trying to store the translated letter - {ex.InnerException.ToString()}");
            }
        }

        private static async Task<TranslatedLetter> Translate(Letter letter)
        {
            object[] body = new object[] { new { Text = letter.Message } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                // Set the method to Post.
                request.Method = HttpMethod.Post;

                // Construct the URI and add headers.
                request.RequestUri = new Uri(_endPoint + "/translate?api-version=3.0&to=en");
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();

                // Deserialize the response using the classes created earlier.
                TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);

                // Print the detected input language and confidence score.
                Console.WriteLine("Detected input language: {0}\nConfidence score: {1}\n", deserializedOutput[0].DetectedLanguage.Language, deserializedOutput[0].DetectedLanguage.Score);

                Console.WriteLine("Translated to {0}: {1}", deserializedOutput[0].Translations[0].To, deserializedOutput[0].Translations[0].Text);

                return new TranslatedLetter
                {
                    Who = letter.Who,
                    Message = letter.Message,
                    FromLanguage = deserializedOutput[0].DetectedLanguage.Language,
                    ToLanguage = deserializedOutput[0].Translations[0].To,
                    TranslationConfidenceScore = deserializedOutput[0].DetectedLanguage.Score,
                    TranslatedMessage = deserializedOutput[0].Translations[0].Text
                };
            }
        }
    }
}