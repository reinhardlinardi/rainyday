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
            if (keyword == null) return Content("<div class=\"news_content\"><h4>Keyword is empty.</h4></div>");
            else
            {
                ReadDataFromFile(); // read data from file
                news_match = new List<NewsMatch>();

                Algorithm algo = new Algorithm();
                algo.StringMatching(keyword, algorithm, news_list, news_match);
                int counter = -1;
                string news_content = "";
                foreach (NewsMatch match in news_match)
                {
                    counter++;
                    if (match.isContent != -1)
                    {
                        news_content = news_content + "<div class=\"news_content\">";
                        if (match.isContent == 0)
                        {
                            //Ketemu di title
                            news_content = news_content + "<h2>" + news_list[counter].title + "</h2>";
                            news_content = news_content + "<h4>" + news_list[counter].link + "</h4>";
                            news_content = news_content + "<p>" + news_list[counter].date + "</p>";
                            news_content = news_content + "<h3>" + news_list[counter].content.Substring(0, 50) + "</h3>";
                        }
                        else
                        {
                            //ketemu di content
                            int content_length = news_list[counter].content.Length;
                            int index_start = news_match[counter].start - 20;
                            int index_end = news_match[counter].end + 20;
                            news_content = news_content + "<h2>" + news_list[counter].title + "</h2>";
                            news_content = news_content + "<h4>" + news_list[counter].link + "</h4>";
                            news_content = news_content + "<p>" + news_list[counter].date + "</p>";

                            if (index_start < 0)
                            {
                                index_start = 0;
                            }

                            if (index_end > content_length)
                            {
                                index_end = content_length;
                            }
                            news_content = news_content + "<h3>" + news_list[counter].content.Substring(0,50/*index_start, index_end*/) + "</h3>";
                        }
                        news_content = news_content + "</div>";
                    }
                }

                return Content(news_content);
            }
        }
    }
}