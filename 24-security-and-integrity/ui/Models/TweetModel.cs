using Microsoft.WindowsAzure.Storage.Table;

namespace Christmas.Sweden.Models
{
    public class TweetModel : TableEntity
    {
        public string OriginalText { get; set; }

        public string TranslatedText { get; set; }

        public string OriginalLanguage { get; set; }

        public string TranslatedLanguage { get; set; }

        public string Username { get; set; }

        public string SentTimestamp { get; set; }

        public double Sentiment { get; set; }

        public string Url => $"https://twitter.com/{Username}/status/{Sentiment}";

        public TweetModel() { }

        public TweetModel(string originalText, string translatedText, string originalLanguage, string translatedLanguage, string username, string sentTimestamp, double sentiment)
        {
            OriginalText = originalText;

            TranslatedText = translatedText;

            OriginalLanguage = originalLanguage;

            TranslatedLanguage = translatedLanguage;

            Username = username;

            SentTimestamp = sentTimestamp;

            Sentiment = sentiment;
        }
    }
}