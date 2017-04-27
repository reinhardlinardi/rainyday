using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web.Script.Serialization;
using RainyDay.Models;
using RainyDay;

namespace RainyDay.Controllers
{
    public class HomeController : Controller
    {
        List<News> news_list;
        List<NewsMatch> news_match;

        /* View methods */

        public IActionResult Index() // for Index.cshtml page
        {
            return View(); // return ViewResult
        }
        
        public IActionResult Search() // for Search.cshtml page
        {
            return View(); // return ViewResult
        }
        
        public IActionResult About() //for About.cshtml page
        {
            return View();
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

        public void ReadDataFromFile()
        {
            news_list = new List<News>();

            string news_json = System.IO.File.ReadAllText("News.json"); // get json of string
            news_list = new JavaScriptSerializer().Deserialize<List<News>>(news_json); // convert to List of News
        }

        [HttpPost]
        public IActionResult SearchKeyword(string keyword, string algorithm)
        {
            ReadDataFromFile(); // read data from file
            news_match = new List<NewsMatch>();

            Algorithm algo = new Algorithm();
            algo.StringMatching(keyword, algorithm, news_list, news_match);

            return Content("Keyword = " + keyword + ", Algorithm = " + algorithm);
        }
    }
}