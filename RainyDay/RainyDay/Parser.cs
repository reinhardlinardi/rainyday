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
        string[] url_list;
        List<News> news_list; 

        public void ReadWebsitesFromFile(string filename)
        {
            url_list = System.IO.File.ReadAllLines(filename);
        }

        public void ParseRSS()
        {
            news_list = new List<News>(); // Create new List of News

            XmlReader xml_reader;
            XmlDocument xml_doc = new XmlDocument();

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

                    News news = new News(news_title, news_date, news_link, ""); // create News object
                    news_list.Add(news); // add to news list
                }
            }
        }

        public void ParseHTML()
        {
            // Website RSS URL
            string viva = "rss.viva.co.id";
            string antara = "www.antaranews.com/rss/";
            string tempo = "www.tempo.co/rss/";
            string detik = "rss.detik.com";

            // Web Client to download HTML
            WebClient webclient = new WebClient();
            webclient.Encoding = System.Text.Encoding.UTF8; // set encoding to UTF-8
            
            foreach (News _news in news_list)
            {
                string news_link = _news.link;
                string page_html = webclient.DownloadString(news_link); // download HTML
                
                // Determine website from URL
                if (Regex.IsMatch(news_link, viva)) // if website is viva
                {
                    string viva_news = "<\\s+span\\s+itemprop\\s+=\\s+\"description\"\\s+><\\s+p\\s+><\\s+strong\\s+>\\s+VIVA.co.id\\s+</\\s+strong\\s+>\\s+–\\s+(.+)<\\s+span>";
                    Match news_match = Regex.Match(page_html, viva_news, RegexOptions.Singleline); // get news

                    string raw_news = news_match.Groups[1].Value; // get 1st subexpression
                    string formatted_news = Regex.Replace(raw_news, "\\s+", " "); // replace all consecutive whitespaces with a single space

                    _news.content = formatted_news;

                    System.Diagnostics.Debug.WriteLine(formatted_news);

                    System.IO.File.AppendAllText("tes.txt",formatted_news);
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