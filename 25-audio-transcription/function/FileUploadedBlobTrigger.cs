using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Christmas.AudioTranscription
{
    public class FileUploadedBlobTrigger
    {
        private ILogger _log;

        [FunctionName("FileUploadedBlobTrigger")]
        public async Task Run(
            [BlobTrigger("uploads/{name}", Connection = "AzureWebJobsStorage")]
            Stream blob,
            string name,
            Uri uri,
            [Blob("transcribed/{name}.txt", FileAccess.Write)] TextWriter text,
            [TwilioSms(AccountSidSetting = "TWILIO_ACCOUNT_SID", AuthTokenSetting = "TWILIO_AUTH_TOKEN", From = "%TWILIO_FROM_NUMBER%")] IAsyncCollector<CreateMessageOptions> message,
            ILogger log)
        {
            _log = log;

            _log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {blob.Length} Bytes");

            var tempFileLocation = $"{Path.GetTempPath()}\\{name}";

            DownloadTempFile(uri.ToString(), tempFileLocation);

            // Use Cognitive Services to recognize the speech
            var result = await RecognizeSpeechAsync(tempFileLocation);

            // Write the results to blob storage
            await text.WriteAsync(result);

            await message.AddAsync(
                new CreateMessageOptions(new PhoneNumber(Environment.GetEnvironmentVariable("TWILIO_TO_NUMBER").ToString()))
                {
                    Body = $"Aarti, good news, one of your friends has just uploaded a new recipe"
                }
            );

            DeleteTempFile(tempFileLocation);
        }

        private async Task<string> RecognizeSpeechAsync(string uri)
        {
            var substriptionKey = Environment.GetEnvironmentVariable("SPEECH_SUBSCRIPTION_KEY");
            var serviceRegion = Environment.GetEnvironmentVariable("SPEECH_SERVICE_REGION");

            var config = SpeechConfig.FromSubscription(substriptionKey, serviceRegion);

            using (var audioInput = AudioConfig.FromWavFileInput(uri))
            {
                using (var recognizer = new SpeechRecognizer(config, audioInput))
                {
                    _log.LogInformation("Recognizing first result...");
                    var result = await recognizer.RecognizeOnceAsync();

                    if (result.Reason == ResultReason.RecognizedSpeech)
                    {
                        _log.LogInformation($"We recognized: {result.Text}");
                    }
                    else if (result.Reason == ResultReason.NoMatch)
                    {
                        _log.LogInformation($"NOMATCH: Speech could not be recognized.");
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = CancellationDetails.FromResult(result);
                        _log.LogInformation($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            _log.LogInformation($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            _log.LogInformation($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                            _log.LogInformation($"CANCELED: Did you update the subscription info?");
                        }
                    }

                    return result.Text;
                }
            }
        }

        private void DownloadTempFile(string uri, string tempFileLocation)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(uri, tempFileLocation);
            }
        }

        private void DeleteTempFile(string tempFileLocation)
        {
            File.Delete(tempFileLocation);
        }
    }
}
