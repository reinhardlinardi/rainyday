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
                    string raw_news;
                    Match news_match;

                    // get news
                    string viva_news_regex1 = "<span itemprop=\"description\"><p><strong>VIVA\\.co\\.id</strong>\\s*\\S+\\s*(.+)";
                    news_match = Regex.Match(page_html, viva_news_regex1, RegexOptions.Singleline); 
                    raw_news = news_match.Groups[1].Value;

                    string viva_news_regex2 = "^(.+)(?:\\s*\\(.+\\))?</p>\\s*</span>";
                    news_match = Regex.Match(raw_news, viva_news_regex2, RegexOptions.Singleline);
                    raw_news = news_match.Groups[1].Value;

                    raw_news = Regex.Replace(raw_news, "<.+>", "", RegexOptions.Singleline); // remove remaining tag
                    raw_news = Regex.Replace(raw_news, "(?<=\\.)\\s*\\([a-z]+\\)", ""); // remove editor info
                    raw_news = Regex.Replace(raw_news, "(\\n|&nbsp;)", " "); // replace all newline with spaces.
                    raw_news = Regex.Replace(raw_news, "&[^;]+;", ""); // remove all & character
                    
                    string formatted_news = Regex.Replace(raw_news, "\\s+", " "); // replace all consecutive whitespaces with a single space
                    
                    if (formatted_news != "") // if formatted news is not empty
                    {
                        _news.content = formatted_news;
                        news_list.Add(_news); // add to final news list
                    }
                }
                else if (Regex.IsMatch(news_link, antara)) // if website is antara
                {
                    string raw_news;
                    Match news_match;

                    string antara_regex = "<div id=\"content_news\".+\\(ANTARA News\\)\\s*\\S+\\s*(.+)<p class=\"mt10\">";
                    news_match = Regex.Match(page_html, antara_regex, RegexOptions.Singleline);
                    raw_news = news_match.Groups[1].Value;

                    raw_news = Regex.Replace(raw_news, "<[^>]+>", "", RegexOptions.Singleline); // remove remaining tag
                    raw_news = Regex.Replace(raw_news, "(\\n|&nbsp;)", " "); // replace all newline with spaces
                    raw_news = Regex.Replace(raw_news, "\\(T\\..+\\)$", "");
                    raw_news = Regex.Replace(raw_news, "&[^;]+;", ""); // remove all & character

                    string formatted_news = Regex.Replace(raw_news, "\\s+", " "); // replace all consecutive whitespaces with a single space

                    if (formatted_news != "") // if formatted news is not empty
                    {
                        _news.content = formatted_news;
                        news_list.Add(_news); // add to final news list
                    }
                }
                else if (Regex.IsMatch(news_link, tempo)) // if website is tempo
                {
                    string raw_news;
                    Match news_match;

                    string tempo_regex = "<!-- end block display -->(.+)<!-- end artikel -->";
                    news_match = Regex.Match(page_html, tempo_regex, RegexOptions.Singleline);
                    raw_news = news_match.Groups[1].Value;

                    raw_news = Regex.Replace(raw_news, "<[^>]+>", "", RegexOptions.Singleline); // remove remaining tag
                    raw_news = Regex.Replace(raw_news, "TEMPO[^-]+-\\s*", ""); // remove header
                    raw_news = Regex.Replace(raw_news, "(\\n|&nbsp;)", " "); // replace all newline with spaces
                    raw_news = Regex.Replace(raw_news, "(?<=\\.)(\\s*[A-Z]+)*\\s*$", ""); // remove editor
                    raw_news = Regex.Replace(raw_news, "&[^;]+;", ""); // remove all & character

                    string formatted_news = Regex.Replace(raw_news, "\\s+", " "); // replace all consecutive whitespaces with a single space

                    if (formatted_news != "") // if formatted news is not empty
                    {
                        _news.content = formatted_news;
                        news_list.Add(_news); // add to final news list
                    }
                }
                else if (Regex.IsMatch(news_link, detik)) // if website is detik
                {
                    string detik_regex2 = "(.+)$";
                    news_match = Regex.Match(raw_news, detik_regex2);
                    raw_news = news_match.Groups[1].Value;

                    raw_news = Regex.Replace(raw_news, "<.+>", "", RegexOptions.Singleline); // remove remaining tag
                    raw_news = Regex.Replace(raw_news, "\\n", " "); // replace all newline with spaces.

                    string formatted_news = Regex.Replace(raw_news, "\\s+", " "); // replace all consecutive whitespaces with a single space

                    if (formatted_news != "") // if formatted news is not empty
                    {
                        _news.content = formatted_news;
                        news_list.Add(_news); // add to final news list
                    }
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