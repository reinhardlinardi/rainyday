using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RainyDay.Models;
using RainyDay;

namespace RainyDay.Controllers
{
    public class HomeController : Controller
    {
        /* View methods */
        
        public IActionResult Index() // for Index.cshtml page
        {
            return View(); // return ViewResult
        }
        
        public IActionResult Search() // for Search.cshtml page
        {
            return View(); // return ViewResult
        }
        
        /* Ajax request handlers */
        
        [HttpPost]
        public void StartUpdate() // update RSS Feed
        {
            Parser parser = new Parser();

            parser.ReadWebsitesFromFile("InputWebsites.txt");
            parser.ParseRSS();
            parser.ParseHTML();
            parser.WriteJSONToFile("News.json");

            return;
        }

        [HttpPost]
        public IActionResult SearchKeyword(string keyword, string algorithm)
        {
            return Content("Keyword = " + keyword + ", Algorithm = " + algorithm);
        }
    }
}