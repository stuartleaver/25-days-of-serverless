using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using System.Collections.Generic;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Net;

namespace Christmas.Sweden.KeyVault.Backup
{
    public static class RestoreKeyVault
    {
        private static AzureServiceTokenProvider _azureServiceTokenProvider;

        private static KeyVaultClient _kv;

        private static string _baseUri;

        [FunctionName("RestoreKeyVault")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "restorekeyvault/{timestamp}")] HttpRequest req,
            string timestamp,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function to restore a Key Vault.");

            // Declares a KeyVaultClient instance.
            _azureServiceTokenProvider = new AzureServiceTokenProvider();
            _kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(_azureServiceTokenProvider.KeyVaultTokenCallback));

            // Set the vase uri of the Key Vault being backed up
            _baseUri = Environment.GetEnvironmentVariable("RESTORE_KEY_VAULT_ENDPOINT");

            // Restore secrets
            var secrets = await DownloadAsync<BackupSecretResult>(timestamp, "secrets").ConfigureAwait(false);
            var secretsResult = await RestoreSecretsAsync(secrets).ConfigureAwait(false);

            // Restore keys
            var keys = await DownloadAsync<BackupKeyResult>(timestamp, "keys").ConfigureAwait(false);
            var keysResult = await RestoreKeysAsync(keys).ConfigureAwait(false);

            // Restore certificates
            var certificates = await DownloadAsync<BackupCertificateResult>(timestamp, "certificates").ConfigureAwait(false);
            var certificatesResult = await RestoreCertificatesAsync(certificates).ConfigureAwait(false);

            var results = new RestoreResult
            {
                SecretsResult = secretsResult,
                KeysResult = keysResult,
                CertificatesResult = certificatesResult
            };

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(results), Encoding.UTF8, "application/json")
            };
        }

        public static async Task<List<T>> DownloadAsync<T>(string timestamp, string type)
        {
            // Declares the BlobClient instance.
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var client = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();

            // Gets the Blob container.
            var containerName = Environment.GetEnvironmentVariable("BACKUP_CONTAINER_NAME");
            var container = client.GetContainerReference(containerName);

            // Gets the Blob.
            var blobName = $"{timestamp}_{type}.json";
            var blob = container.GetBlockBlobReference(blobName);

            // Downloads the Blob content.
            var downloaded = await blob.DownloadTextAsync().ConfigureAwait(false);

            // Deserialises the contents.
            var results = JsonConvert.DeserializeObject<List<T>>(downloaded);

            // Returns the result.
            return results;
        }

        public static async Task<List<string>> RestoreSecretsAsync(List<BackupSecretResult> secrets)
        {
            // Performs the restore and add the result into the list.
            var results = new List<SecretBundle>();

            foreach (var secret in secrets)
            {
                var result = await _kv.RestoreSecretAsync(_baseUri, secret.Value).ConfigureAwait(false);
                results.Add(result);
            }

            // Returns the list of secret names.
            return results.Select(p => p.SecretIdentifier.Name).ToList();
        }

        public static async Task<List<string>> RestoreKeysAsync(List<BackupKeyResult> keys)
        {
            // Performs the restore and add the result into the list.
            var results = new List<KeyBundle>();

            foreach (var key in keys)
            {
                var result = await _kv.RestoreKeyAsync(_baseUri, key.Value).ConfigureAwait(false);
                results.Add(result);
            }

            // Returns the list of secret names.
            return results.Select(p => p.KeyIdentifier.Name).ToList();
        }

        public static async Task<List<string>> RestoreCertificatesAsync(List<BackupCertificateResult> certificates)
        {
            // Performs the restore and add the result into the list.
            var results = new List<CertificateBundle>();

            foreach (var certificate in certificates)
            {
                var result = await _kv.RestoreCertificateAsync(_baseUri, certificate.Value).ConfigureAwait(false);
                results.Add(result);
            }

            // Returns the list of secret names.
            return results.Select(p => p.CertificateIdentifier.Name).ToList();
        }
    }
}