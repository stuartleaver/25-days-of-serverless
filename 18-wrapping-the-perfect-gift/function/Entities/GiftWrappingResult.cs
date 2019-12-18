using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Christmas.GiftWrappingAnalyser.Entities
{
    public class GiftWrappingResult : TableEntity
    {
        public string FileName { get; set; }

        public Uri FileUri { get; set; }

        public bool IsBoxed { get; set; }

        public bool IsGiftWrapped { get; set; }

        public bool HasRibbon { get; set; }

        public bool IsPresent { get; set; }

        public bool IsPerfectlyGiftWrapped { get; set; }

        public GiftWrappingResult(string fileName, Uri fileUri, bool isBoxed, bool isGiftWrapped, bool hasRibbon, bool isPresent)
        {
            PartitionKey = "giftwrappingresult";

            RowKey = fileName;

            FileName = fileName;

            FileUri = fileUri;

            IsBoxed = isBoxed;

            IsGiftWrapped = isGiftWrapped;

            HasRibbon = hasRibbon;

            IsPresent = isPresent;

            IsPerfectlyGiftWrapped = isBoxed && isGiftWrapped && hasRibbon && isPresent;
        }
    }
}