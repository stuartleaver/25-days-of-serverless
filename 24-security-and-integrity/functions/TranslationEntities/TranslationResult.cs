namespace Christmas.Sweden.TranslationEntities
{
    public class TranslationResult
    {
        public DetectedLanguage DetectedLanguage { get; set; }

        public TextResult SourceText { get; set; }

        public Translation[] Translations { get; set; }
    }
}