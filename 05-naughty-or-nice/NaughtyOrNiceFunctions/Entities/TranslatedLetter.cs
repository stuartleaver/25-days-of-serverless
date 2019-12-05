namespace NaughtyOrNice.Entities
{
    public class TranslatedLetter : Letter
    {
        public string FromLanguage { get; set; }
        
        public string ToLanguage { get; set; }

        public float TranslationConfidenceScore { get; set; }

        public string TranslatedMessage { get; set; }
    }
}