using BlogSample_ASPDotNetApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlogSample_ASPDotNetApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static ActivitySource MyActivitySource { get; set; }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private readonly HttpClient httpClient = new HttpClient();
        [HttpGet]
        [Route("/outgoing-http-call")]

        public string OutgoingHttp()
        {
            //Custom Traces
            MyActivitySource = new ActivitySource("test", "test");
            using var activity = MyActivitySource.StartActivity("VisitHome", ActivityKind.Server); // this will be translated to a X-Ray Segment
            activity?.SetTag("http.method", "GET");
            activity?.SetTag("http.url", "http://www.bookstorecore.com/Books");
            activity?.SetTag("http.page", "BooksIndex");

            _ = httpClient.GetAsync("https://aws.amazon.com").Result;
            return "success";
        }

        public IActionResult Index()
        {
            _logger.LogDebug("A user has visited the site.");
            return View();
        }

        public IActionResult Privacy()
        {
            _logger.LogDebug("The Privacy link was clicked by the user");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}