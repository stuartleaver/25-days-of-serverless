using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.KeyVault;
using System.Linq;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.WindowsAzure.Storage;
using System.Net.Http;
using System.Text;
using System.Net;

namespace Christmas.BackupKeyVault
{
    public static class BackupKeyVault
    {
        private static AzureServiceTokenProvider _azureServiceTokenProvider;

        private static KeyVaultClient _kv;

        private static string _baseUri;

        [FunctionName("BackupKeyVault")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "backupkeyvault")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Declares a KeyVaultClient instance.
            _azureServiceTokenProvider = new AzureServiceTokenProvider();
            _kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(_azureServiceTokenProvider.KeyVaultTokenCallback));

            // Set the vase uri of the Key Vault being backed up
            _baseUri = Environment.GetEnvironmentVariable("KEY_VAULT_ENDPOINT");

            // Gets the list of secrets.
            var secrets = await GetSecretsAsync().ConfigureAwait(false);
            var keys = await GetKeysAsync().ConfigureAwait(false);
            var certificates = await GetCertificatesAsync().ConfigureAwait(false);

            // Remove any certificates from the list of secrets and keys
            secrets = secrets.Except(certificates).ToList();
            keys = keys.Except(certificates).ToList();

            // Performs the backup.
            var resultsSecrets = await BackupSecretsAsync(secrets).ConfigureAwait(false);
            var resultsKeys = await BackupKeysAsync(keys).ConfigureAwait(false);
            var resultsCertificates = await BackupCertificatesAsync(certificates).ConfigureAwait(false);

            // Uploads the backup data.
            var results = new BackupResult
            {
                Secrets = resultsSecrets,
                Keys = resultsKeys,
                Certificates = resultsCertificates
            };

            results.SecretsUploaded = await UploadAsync(resultsSecrets, "secrets").ConfigureAwait(false);
            results.KeysUploaded = await UploadAsync(resultsKeys, "keys").ConfigureAwait(false);
            results.CertificatesUploaded = await UploadAsync(resultsCertificates, "certificates").ConfigureAwait(false);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(results), Encoding.UTF8, "application/json")
            };
        }

        private static async Task<List<string>> GetSecretsAsync()
        {

            // Gets the list of secrets.
            var secrets = await _kv.GetSecretsAsync(_baseUri).ConfigureAwait(false);

            // Returns the list of secret names.
            return secrets.Select(p => p.Identifier.Name).ToList();
        }

        private static async Task<List<string>> GetKeysAsync()
        {
            // Gets the list of keys.
            var secrets = await _kv.GetKeysAsync(_baseUri).ConfigureAwait(false);

            // Returns the list of key names.
            return secrets.Select(p => p.Identifier.Name).ToList();
        }

        private static async Task<List<string>> GetCertificatesAsync()
        {
            // Gets the list of certificates.
            var secrets = await _kv.GetCertificatesAsync(_baseUri).ConfigureAwait(false);

            // Returns the list of certificate names.
            return secrets.Select(p => p.Identifier.Name).ToList();
        }

        public static async Task<List<BackupSecretResult>> BackupSecretsAsync(List<string> secrets)
        {
            // Performs the backup and add the result into the list.
            var results = new List<BackupSecretResult>();

            foreach (var name in secrets)
            {
                var result = await _kv.BackupSecretAsync(_baseUri, name).ConfigureAwait(false);
                results.Add(result);
            }

            // Returns the backup results.
            return results;
        }

        public static async Task<List<BackupKeyResult>> BackupKeysAsync(List<string> secrets)
        {
            // Performs the backup and add the result into the list.
            var results = new List<BackupKeyResult>();

            foreach (var name in secrets)
            {
                var result = await _kv.BackupKeyAsync(_baseUri, name).ConfigureAwait(false);
                results.Add(result);
            }

            // Returns the backup results.
            return results;
        }

        public static async Task<List<BackupCertificateResult>> BackupCertificatesAsync(List<string> secrets)
        {
            // Performs the backup and add the result into the list.
            var results = new List<BackupCertificateResult>();

            foreach (var name in secrets)
            {
                var result = await _kv.BackupCertificateAsync(_baseUri, name).ConfigureAwait(false);
                results.Add(result);
            }

            // Returns the backup results.
            return results;
        }

        public static async Task<bool> UploadAsync<T>(List<T> results, string type)
        {
            // Declares the BlobClient instance.
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var account = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();

            // Gets the Blob container.
            var containerName = Environment.GetEnvironmentVariable("BACKUP_CONTAINER_NAME");
            var container = account.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync().ConfigureAwait(false);

            // Gets the Blob.
            var blobName = $"{DateTimeOffset.UtcNow.ToString("yyyyMMdd")}_{type}.json";
            var blob = container.GetBlockBlobReference(blobName);

            // Serialises the backup result.
            var serialised = JsonConvert.SerializeObject(results);

            // Uploads the backup result to Blob Storage.
            await blob.UploadTextAsync(serialised).ConfigureAwait(false);

            // Returns true, if everything is OK.
            return true;
        }
    }
}