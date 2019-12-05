namespace NaughtyOrNiceMvc.Models
{
    public class Letter
    {
        public string Who { get; set; }

        public string Message { get; set; }

        public string TranslatedMessage { get; set; }

        public double Sentiment { get; set; }
    }
}