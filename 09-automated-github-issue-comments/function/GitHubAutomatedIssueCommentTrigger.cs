using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;

namespace GitHub.Automation
{
    public static class GitHubAutomatedIssueCommentTrigger
    {
        [FunctionName("GitHubAutomatedIssueCommentTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string commentsUrl = data?.issue.comments_url;
            string login = data?.issue.user.login;

            HttpResponseMessage response = null;
            string result = string.Empty;

            var githubToken = await GetSecret("GITHUB_TOKEN_SECRET_KEY_VAULT_URL");

            if (data?.action == "opened")
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage())
                {
                    request.RequestUri = new Uri(commentsUrl);
                    request.Method = HttpMethod.Post;
                    request.Headers.Add("User-Agent", "GitHubIssueTrigger-stuartleaver");
                    request.Headers.Add("Authorization", $"Token {githubToken}");

                    requestBody = JsonConvert.SerializeObject(new
                    {
                        body = $"Thank you @{login} from your friendly **Azure Function** for submitting this issue. We will take a look and respond in due course. Have a Happy Holiday season."
                    });

                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    response = await client.SendAsync(request).ConfigureAwait(false);

                    result = await response.Content.ReadAsStringAsync();
                }

            }

            return response.StatusCode == HttpStatusCode.Created
                ? (ActionResult)new OkObjectResult(result)
                : new BadRequestObjectResult(result);
        }

        private static async Task<string> GetSecret(string secretKey)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var secret = await keyVaultClient.GetSecretAsync(Environment.GetEnvironmentVariable(secretKey)).ConfigureAwait(false);

            return secret.Value;
        }
    }
}