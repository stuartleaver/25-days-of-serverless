using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wishlist.Models;

namespace WishList.Controllers
{
    public class AddController : Controller
    {
        private readonly ILogger<AddController> _logger;

        private readonly IConfiguration _configuration; 

        public AddController(ILogger<AddController> logger, IConfiguration configuration)
        {
            _logger = logger;

            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Wish,Address,Type")] WishListItemModel item)
        {
            if (ModelState.IsValid)
            {
                var result = await SaveWishListItemAsync(item);

                if(result == HttpStatusCode.OK)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
            //return View(item);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<HttpStatusCode> SaveWishListItemAsync(WishListItemModel letter)
        {
            string endPoint = _configuration.GetValue<string>("AzureFunctionEndPoint");
            
            var requestBody = JsonConvert.SerializeObject(letter);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                // Set the method to Post.
                request.Method = HttpMethod.Post;

                // Construct the URI and add headers.
                request.RequestUri = new Uri($"{endPoint}/api/AddItem");
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                return response.StatusCode;
            }
        }
    }
}
