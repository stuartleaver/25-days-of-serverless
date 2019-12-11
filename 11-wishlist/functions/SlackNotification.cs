using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Slack.Webhooks;

namespace WishList.Functions
{
    public static class SlackNotification
    {
        [FunctionName("SlackNotification")]
        public static void Run([CosmosDBTrigger(
            databaseName: "wishlist",
            collectionName: "items",
            ConnectionStringSetting = "CosmosDBConnection",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName = "leases")] IReadOnlyList<Document> documents,
            ILogger log)
        {
            if (documents != null && documents.Count > 0)
            {
                var client = new SlackClient(Environment.GetEnvironmentVariable("SlackWebhook"));

                foreach(var document in documents)
                {
                    var message = new SlackMessage {
                        Text = $"{document.GetPropertyValue<string>("name")} has wished - {document.GetPropertyValue<string>("wish")}"
                    };

                    client.Post(message);
                }
            }
        }
    }
}
