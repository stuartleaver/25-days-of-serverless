using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wishlist.Models;

namespace WishList.Controllers
{
    public class ListController : Controller
    {
        private readonly ILogger<ListController> _logger;

        private readonly IConfiguration _configuration; 

        public ListController(ILogger<ListController> logger, IConfiguration configuration)
        {
            _logger = logger;

            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            return View(await GetWishesAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<List<WishListItemModel>> GetWishesAsync()
        {
            string endPoint = _configuration.GetValue<string>("AzureFunctionEndPoint");

            using (var client = new HttpClient())
            {
                // Construct the URI and add headers.
                var response = await client.GetAsync($"{endPoint}/api/QueryItems");

                var result = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<WishListItemModel>>(result);
            }
        }
    }
}
