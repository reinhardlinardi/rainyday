using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System.Web.Script.Serialization;
using RainyDay.Models;

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
            string[] url_list = System.IO.File.ReadAllLines("InputWebsites.txt"); // read all rss url from text file
            
            XmlReader xml_reader;
            XmlDocument xml_doc = new XmlDocument();

            List<News> news_list = new List<News>(); // Create new List of News

            foreach (string url in url_list) // for each url
            {
                xml_reader = XmlReader.Create(url); // read xml from specified url
                xml_doc.Load(xml_reader); // create xml document

                XmlNodeList nodes = xml_doc.SelectNodes("//rss/channel/item"); // get all <item> as node list using XPath

                foreach (XmlNode node in nodes) // for each <item>
                {
                    XmlNode title = node.SelectSingleNode("title"); // get news title using XPath
                    XmlNode pubDate = node.SelectSingleNode("pubDate"); // get news date using XPath
                    XmlNode link = node.SelectSingleNode("link"); // get news link using XPath

                    string news_title = title.InnerText; // get content from title node
                    string news_date = pubDate.InnerText; // get content from pubDate node
                    string news_link = link.InnerText; // get content from link node

                    News news = new News(news_title, news_date, news_link); // create News object
                    news_list.Add(news); // add to news list
                }
            }

            string news_json = new JavaScriptSerializer().Serialize(news_list); // convert List of News to JSON string
            System.IO.File.WriteAllText("News.json", news_json); // write JSON string to JSON file

            return;
        }
    }
}