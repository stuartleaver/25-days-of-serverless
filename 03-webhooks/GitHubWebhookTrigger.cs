using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;

namespace GitHub.Webhook
{
    public static class GitHubWebhookTrigger
    {
        [FunctionName("GitHubWebhookTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JObject.Parse(requestBody);

            string htmlUrl = data?.SelectToken("repository.html_url").ToString();
            string refs = data?.SelectToken("ref").ToString().Replace("refs/heads", "");

            var fileUrls = GetAddedPngFiles(data, htmlUrl, refs); 

            var result = SaveFileUrls(fileUrls, log);

            return result
                ? (ActionResult)new OkObjectResult($"File URLs saved to the database - {fileUrls.Count}.")
                : new BadRequestObjectResult("An error occured. Please investigate.");
        }

        private static List<string> GetAddedPngFiles(dynamic data, string htmlUrl, string refs)
        {
            var files = new List<String>();

            foreach(dynamic file in data.SelectToken("head_commit.added"))
            {
                if(file.Value.ToString().ToLower().EndsWith("png"))
                {
                    var url = string.Concat(htmlUrl, "/blob", refs, "/", file.Value.ToString());

                    files.Add(url);
                }
            }

            return files;
        }

        private static bool SaveFileUrls(List<string> fileUrls, ILogger log)
        {
            try
            {
                var sql = "INSERT INTO dbo.GitHubFiles (FileUrl) VALUES (@FileUrl)";

                using(var connection = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionString")))
                {
                    using(var command = new SqlCommand(sql, connection))
                    {
                        connection.Open();

                        command.Parameters.Add("@FileUrl", SqlDbType.NVarChar);

                        foreach(var fileUrl in fileUrls)
                        {
                            command.Parameters["@FileUrl"].Value = fileUrl;

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return true;
            }
            catch(SqlException ex)
            {
                log.LogInformation($"Opps, something went wrint inserting the data. Error - {ex.InnerException}");

                return false;
            }
        }
    }
}
