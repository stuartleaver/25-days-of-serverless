using System;
using System.Threading.Tasks;
using Unsplasharp;

namespace PictureChallenge.Api.Services
{
    public class ImageSearchService
    {
        public UnsplasharpClient _client;

        public ImageSearchService(string unsplashAccessKey, string unsplashSecretKey)
        {
            _client = new UnsplasharpClient(unsplashAccessKey, unsplashSecretKey);
        }

        public async Task<Image> GetImageAsync(string query)
        {
            var random = new Random();
            var imageId = random.Next(0, 9);
            
            var results = await _client.SearchPhotos(query, 1, 10);

            var image = new Image
            {
                Id = results[imageId].Id,
                Url = results[imageId].Urls.Regular,
                Alt = string.IsNullOrEmpty(results[imageId].Description) ? query : results[imageId].Description
            };

            return image;
        }
    }
}