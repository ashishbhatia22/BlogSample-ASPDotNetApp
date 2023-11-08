﻿using BlogSample_ASPDotNetApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace BlogSample_ASPDotNetApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILoggerManager _logger;
        private Meter _meter;


        private static ActivitySource? MyActivitySource { get; set; }

        public HomeController(ILoggerManager logger, Meter meter)
        {
            _meter = meter;
            _logger = logger;
        }

        private readonly HttpClient httpClient = new HttpClient();
        [HttpGet]
        [Route("/outgoing-http-call")]

        public string OutgoingHttp()
        {
            //Custom Traces
            MyActivitySource = new ActivitySource("OutgoingHttpCall", "1");
            using var activity = MyActivitySource.StartActivity("VisitHome", ActivityKind.Server); // this will be translated to a X-Ray Segment
            activity?.SetTag("http.method", "GET");
            activity?.SetTag("http.url", "http://www.sample-app.com/outgoing-http-call");
            activity?.SetTag("http.page", "HomeIndex");

            
            var requestCounter = _meter.CreateCounter<long>("Requests");
            var requestHistogram = _meter.CreateHistogram<long>("RequestCounter");
            requestCounter.Add(1);
            requestHistogram.Record(new Random().Next(0,100));
            
            _ = httpClient.GetAsync("https://aws.amazon.com").Result;
            return "Successfully invoked the Http Call to aws.amazon.com";
        }

        public IActionResult Index()
        {
            var requestCounter = _meter.CreateCounter<long>("home-request");
            var requestHistogram = _meter.CreateHistogram<long>("home-request-counter");
            requestCounter.Add(1);
            requestHistogram.Record(new Random().Next(0,100));
            
            _logger.LogDebug("A user has visited the sample site.");
            return View();
        }

        public IActionResult Privacy()
        {
            var requestCounter = _meter.CreateCounter<long>("privacy-request");
            var requestHistogram = _meter.CreateHistogram<long>("privacy-request-counter");
            requestCounter.Add(1);
            requestHistogram.Record(new Random().Next(0,100));
            
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