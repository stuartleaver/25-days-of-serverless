using System.Collections.Generic;
using Microsoft.Azure.KeyVault.Models;

namespace Christmas.BackupKeyVault
{
    public class BackupResult
    {
        public bool SecretsUploaded { get; set; }

        public bool KeysUploaded { get; set; }

        public bool CertificatesUploaded { get; set; }

        public List<BackupSecretResult> Secrets { get; set; }

        public List<BackupKeyResult> Keys { get; set; }

        public List<BackupCertificateResult> Certificates { get; set; }
    }
}