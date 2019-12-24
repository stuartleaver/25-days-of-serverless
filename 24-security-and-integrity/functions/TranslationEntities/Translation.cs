namespace Christmas.Sweden.TranslationEntities
{
    public class Translation
    {
        public string Text { get; set; }

        public TextResult Transliteration { get; set; }

        public string To { get; set; }

        public Alignment Alignment { get; set; }

        public SentenceLength SentLen { get; set; }
    }
}