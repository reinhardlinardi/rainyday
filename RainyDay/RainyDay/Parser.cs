using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using RainyDay.Models;

namespace RainyDay
{
    public class Parser
    {
        string[] url_list; // list of RSS URL
        List<News> raw_news_list; // initial news list
        List<News> news_list; // final news list

        public void ReadWebsitesFromFile(string filename)
        {
            url_list = System.IO.File.ReadAllLines(filename);
        }

        public void ParseRSS()
        {
            raw_news_list = new List<News>(); // Create new List of News

            XmlReader xml_reader;
            XmlDocument xml_doc = new XmlDocument();
            int max = 20; // maximum number of news

            foreach (string url in url_list) // for each URL
            {
                try
                {
                    xml_reader = XmlReader.Create(url); // read xml from specified URL
                }
                catch(WebException w)
                {
                    continue; // skip URL
                }

                xml_doc.Load(xml_reader); // create xml document
                XmlNodeList nodes = xml_doc.SelectNodes("//rss/channel/item"); // get all <item> as node list using XPath

                int counter = 0;

                foreach (XmlNode node in nodes) // for each <item>
                {
                    counter++;

                    if (counter <= 20)
                    {
                        XmlNode title = node.SelectSingleNode("title"); // get news title using XPath
                        XmlNode pubDate = node.SelectSingleNode("pubDate"); // get news date using XPath
                        XmlNode link = node.SelectSingleNode("link"); // get news link using XPath

                        string news_title = title.InnerText; // get content from title node
                        string news_date = pubDate.InnerText; // get content from pubDate node
                        string news_link = link.InnerText; // get content from link node

                        News news = new News(news_title, news_date, news_link, ""); // create News object
                        raw_news_list.Add(news); // add to news list
                    }
                    else break;
                }
            }
        }

        public void ParseHTML()
        {
            news_list = new List<News>();

            // Website RSS URL
            string viva = "viva\\.co\\.id";
            string antara = "antaranews\\.com";
            string tempo = "www\\.tempo\\.co";
            string detik = "rss\\.detik\\.com";

            // Web Client to download HTML
            WebClient webclient = new WebClient();
            webclient.Encoding = System.Text.Encoding.UTF8; // set encoding to UTF-8
            string page_html;
            
            foreach (News _news in raw_news_list)
            {
                string news_link = _news.link;

                try
                {
                    page_html = webclient.DownloadString(news_link); // download HTML
                }
                catch(WebException w)
                {
                    continue; // skip item
                }

                // Determine website from URL
                if (Regex.IsMatch(news_link, viva)) // if website is viva
                {
                    string raw_news;
                    Match news_match;

                    // get news
                    string viva_news_regex1 = "<span itemprop=\"description\"><p><strong>VIVA.co.id</strong>\\s*–\\s*(.+)";
                    news_match = Regex.Match(page_html, viva_news_regex1, RegexOptions.Singleline); 
                    raw_news = news_match.Groups[1].Value;

                    string viva_news_regex2 = "^(.+)</p>\\s*</span>";
                    news_match = Regex.Match(raw_news, viva_news_regex2, RegexOptions.Singleline);
                    raw_news = news_match.Groups[1].Value;

                    raw_news = Regex.Replace(raw_news, "<.+>", "", RegexOptions.Singleline); // remove remaining tag
                    raw_news = Regex.Replace(raw_news, "\n", " ", RegexOptions.Singleline); //replace all newline with spaces.

                    string formatted_news = Regex.Replace(raw_news, "\\s+", " "); // replace all consecutive whitespaces with a single space

                    _news.content = formatted_news;
                    news_list.Add(_news); // add to final news list

                    System.IO.File.AppendAllText("tes.txt",formatted_news);
                    System.IO.File.AppendAllText("tes.txt", "\n");
                }
                else if (Regex.IsMatch(news_link, antara)) // if website is antara
                {
                }
                else if (Regex.IsMatch(news_link, tempo)) // if website is tempo
                {

                }
                else if (Regex.IsMatch(news_link, detik)) // if website is detik
                {

                }
            }
        }

        public void WriteJSONToFile(string filename)
        {
            string news_json = new JavaScriptSerializer().Serialize(news_list); // convert List of News to JSON string
            System.IO.File.WriteAllText(filename, news_json); // write JSON string to JSON file
        }
    } 
}