using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace NaughtyOrNice.Entities
{
    public class AnalysedLetter : TableEntity
    {
        public string Who { get; set; }

        public string Message { get; set; }

        public string FromLanguage { get; set; }

        public string ToLanguage { get; set; }

        public float TranslationConfidenceScore { get; set; }

        public string TranslatedMessage { get; set; }

        public double Sentiment { get; set; }

        public AnalysedLetter(string who, string message, string fromLanguage, string toLanguage, float translationConfidenceScore, string translatedMessage, double sentiment)
        {
            PartitionKey = who.Replace(" ", "");

            RowKey = Guid.NewGuid().ToString();

            Who = who;

            Message = message;

            FromLanguage = fromLanguage;

            ToLanguage = toLanguage;

            TranslationConfidenceScore = translationConfidenceScore;

            TranslatedMessage = translatedMessage;

            Sentiment = sentiment;
        }
    }
}