using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NaughtyOrNiceMvc.Models;
using Newtonsoft.Json;

namespace NaughtyOrNiceMvc.Controllers
{
    public class AddController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration; 

        public AddController(ILogger<HomeController> logger, IConfiguration configuration)
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
        public async Task<IActionResult> Create([Bind("Who,Message")] NewLetter letter)
        {
            if (ModelState.IsValid)
            {
                var result = await SaveMessageAsync(letter);

                if(result == HttpStatusCode.OK)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(letter);
        }

        private async Task<HttpStatusCode> SaveMessageAsync(NewLetter letter)
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
                request.RequestUri = new Uri(endPoint);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                return response.StatusCode;
            }
        }
    }
}