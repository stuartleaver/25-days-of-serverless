namespace Christmas.ComputerVision.Entities
{
    public class ImageDetails
    {
        public string ImageUrl { get; set; }

        public Description Description { get; set; }

        public ImageDetails()
        {
            Description = new Description();
        }
    }
}