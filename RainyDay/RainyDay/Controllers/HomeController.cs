using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using RainyDay.Models;
using RainyDay;

namespace RainyDay.Controllers
{
    public class HomeController : Controller
    {
        /* Global variables */
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

        public void ReadDataFromFile()
        {
            news_list = new List<News>(); // create new List of News

            string news_json = System.IO.File.ReadAllText("News.json"); // read news from JSON
            news_list = new JavaScriptSerializer().Deserialize<List<News>>(news_json); // convert to List of News
        }

        public string FormatDate(string date)
        {
            //Sun, 30 Apr 2017 00:14:01 + 0700
            Match matcher;
            string formatted_date = "";

            string day_pattern = "^(\\w{3})";
            matcher = Regex.Match(date, day_pattern);
            string day = matcher.Groups[1].Value;

            switch(day)
            {
                case "Mon":
                    formatted_date += "Monday";
                    break;

                case "Tue":
                    formatted_date += "Tuesday";
                    break;

                case "Wed":
                    formatted_date += "Wednesday";
                    break;

                case "Thu":
                    formatted_date += "Thursday";
                    break;

                case "Fri":
                    formatted_date += "Friday";
                    break;

                case "Sat":
                    formatted_date += "Saturday";
                    break;

                case "Sun":
                    formatted_date += "Sunday";
                    break;
            }

            string date_pattern = ",\\s+(\\d+)\\s+(\\w{3})\\s+(\\d+)";
            matcher = Regex.Match(date, date_pattern);
            formatted_date += ", " + matcher.Groups[1].Value + " ";

            string month = matcher.Groups[2].Value;

            switch(month)
            {
                case "Jan":
                    formatted_date += "January";
                    break;

                case "Feb":
                    formatted_date += "February";
                    break;

                case "Mar":
                    formatted_date += "March";
                    break;

                case "Apr":
                    formatted_date += "April";
                    break;

                case "May":
                    formatted_date += "May";
                    break;

                case "Jun":
                    formatted_date += "June";
                    break;

                case "Jul":
                    formatted_date += "July";
                    break;

                case "Aug":
                    formatted_date += "August";
                    break;

                case "Sep":
                    formatted_date += "September";
                    break;

                case "Oct":
                    formatted_date += "October";
                    break;

                case "Nov":
                    formatted_date += "November";
                    break;

                case "Dec":
                    formatted_date += "December";
                    break;
            }

            formatted_date += " " + matcher.Groups[3].Value + " ";

            string time_pattern = "(\\d{2}):(\\d{2}):\\d{2}";
            matcher = Regex.Match(date, time_pattern);
            formatted_date += matcher.Groups[1].Value + ":" + matcher.Groups[2].Value;

            return formatted_date;
        }

        [HttpPost]
        public IActionResult SearchKeyword(string keyword, string algorithm)
        {
            ReadDataFromFile(); // read data from file
            news_match = new List<NewsMatch>(); // create List of NewsMatch

            Algorithm algo = new Algorithm();
            algo.StringMatching(keyword, algorithm, news_list, news_match); // search keyword using specified algorithm

            int index = -1; // index of news_list and news_match
            string news_html = ""; // HTML code of search result

            foreach (NewsMatch match in news_match) // for every NewsMatch in news_match
            {
                index++; // increment index

                if (match.isContent != -1) // if keyword found
                {
                    news_html += "\n<div class=\"panel panel-default\">"; // add Bootstrap panel

                    // add news title and date in panel heading
                    news_html += "\n<div class=\"panel-heading\">" + news_list[index].title;
                    for (int i = 1; i <= 12; i++) news_html += "&nbsp;";
                    news_html += FormatDate(news_list[index].date) + "</div>";

                    news_html += "\n<div class=\"panel-body\">"; // add panel body
                    news_html += "\n<div class=\"news-image\"><img src=\"" + news_list[index].image + "\" alt=\"(No Image)\"></div>"; // add image link
                    news_html += "\n<div><p>"; // add news content

                    if (match.isContent == 0) // if keyword found in title
                    {
                        int length = news_list[index].content.Length; // news content length
                        int display_length = 400; // how many characters to be displayed (default 400)

                        if (length < 400) display_length = length; // if news content < 200 characters, display all
                        news_html += news_list[index].content.Substring(0, display_length); // display first 300 characters
                        if (display_length == 400) news_html += "..."; // if news truncated, add triple dots
                    }
                    else // else, keyword found in content
                    {
                        int length = news_list[index].content.Length; // news content length
                        int display_start = news_match[index].start - 150; // display start
                        int display_end = news_match[index].end + 150; // display end

                        news_html += "..."; // add triple dots

                        if (display_start < 0) display_start = 0; // if 50 prefix characters is truncated, set display start to 0
                        if (display_end > length-1) display_end = length-1; // if 50 suffix characters is truncated, set display end to 0

                        news_html += news_list[index].content.Substring(display_start, display_end - display_start + 1); // add news content
                        news_html += "..."; // add triple dots
                    }

                    news_html += "</p></div>"; // end tag of news content
                    news_html += "\n</div>"; // end tag of panel body
                    news_html += "\n</div>"; // end tag of Bootstrap panel
                }
            }

            return Content(news_html); // return HTML code to be displayed by browser
        }
    }
}