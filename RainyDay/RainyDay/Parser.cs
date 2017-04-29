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
            int max = 15; // maximum number of news per website

            foreach (string url in url_list) // for each URL
            {
                try
                {
                    xml_reader = XmlReader.Create(url); // read xml from specified URL
                    xml_doc.Load(xml_reader); // create xml document
                }
                catch (Exception e)
                {
                    continue; // skip URL
                }

                XmlNodeList nodes = xml_doc.SelectNodes("//rss/channel/item"); // get all <item> as node list using XPath
                int counter = 0;

                foreach (XmlNode node in nodes) // for each <item>
                {
                    counter++;

                    if (counter <= max) // limit news
                    {
                        XmlNode title = node.SelectSingleNode("title"); // get news title using XPath
                        XmlNode pubDate = node.SelectSingleNode("pubDate"); // get news date using XPath
                        XmlNode link = node.SelectSingleNode("link"); // get news link using XPath

                        string news_title = title.InnerText; // get content from title node
                        string news_date = pubDate.InnerText; // get content from pubDate node
                        string news_link = link.InnerText; // get content from link node
                        string image_link = ""; // news may not contain images

                        string viva_regex = "rss\\.viva\\.co\\.id";
                        string antara_regex = "antaranews\\.com/rss";
                        string tempo_regex = "tempo\\.co/rss";
                        string detik_regex = "rss\\.detik\\.com/";

                        if(Regex.IsMatch(url,viva_regex) || Regex.IsMatch(url,detik_regex))
                        {
                            XmlNode image = node.SelectSingleNode("enclosure"); // get image url using XPath
                            if(image != null) image_link = image.Attributes["url"].Value; // get image link from url attribute in <enclosure> tag
                        }
                        else if(Regex.IsMatch(url,tempo_regex))
                        {
                            XmlNode image = node.SelectSingleNode("image"); // get image url using XPath
                            if (image != null) image_link = image.InnerText; // get image link from <image> tag
                        }
                        else if(Regex.IsMatch(url,antara_regex))
                        {
                            XmlNode image = node.SelectSingleNode("description");// get image url using XPath
                            if (image != null)
                            {
                                string description = image.InnerText; // get text from <description> tag

                                string antara_image_regex = "img src=\"(.+)\"";
                                Match antara_image = Regex.Match(description, antara_image_regex);
                                image_link = antara_image.Groups[1].Value; // get image url from img src="..."
                            }
                        }

                        News news = new News(news_title, news_date, news_link, "", image_link); // create News object
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
            string detik = "detik\\.com";

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
                catch (Exception e)
                {
                    continue; // skip item
                }

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

                    raw_news = Regex.Replace(raw_news, "<[^>]+>", "", RegexOptions.Singleline); // remove remaining tag
                    raw_news = Regex.Replace(raw_news, "(?<=\\.)\\s*\\([a-z]+\\)", ""); // remove editor info
                    raw_news = Regex.Replace(raw_news, "(\\n|&nbsp;)", " "); // replace all newline with spaces.
                    raw_news = Regex.Replace(raw_news, "&[^;]+;", ""); // remove all & character
                    raw_news = Regex.Replace(raw_news, "^\\s+", ""); // remove trailing spaces at beginning of line 
                    raw_news = Regex.Replace(raw_news, "\\s+$", ""); // remove trailing spaces at end of line

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
                    raw_news = Regex.Replace(raw_news, "\\s*(Baca juga:.+)*$", ""); // remove unnecessary article
                    raw_news = Regex.Replace(raw_news, "^\\s+", ""); // remove trailing spaces at beginning of line 
                    raw_news = Regex.Replace(raw_news, "\\s+$", ""); // remove trailing spaces at end of line

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
                    raw_news = Regex.Replace(raw_news, "(?<=\\.|\"|\\|\\?| !)\\s*(\\s*[A-Z\\|\\.]+)*\\s*(Baca:[^\\.]+)*$", ""); // remove editor
                    raw_news = Regex.Replace(raw_news, "&[^;]+;", ""); // remove all & character
                    raw_news = Regex.Replace(raw_news, "^\\s+", ""); // remove trailing spaces at beginning of line 
                    raw_news = Regex.Replace(raw_news, "\\s+$", ""); // remove trailing spaces at end of line

                    string formatted_news = Regex.Replace(raw_news, "\\s+", " "); // replace all consecutive whitespaces with a single space

                    if (formatted_news != "") // if formatted news is not empty
                    {
                        _news.content = formatted_news;
                        news_list.Add(_news); // add to final news list
                    }
                }
                else if (Regex.IsMatch(news_link, detik)) // if website is detik
                {
                    string raw_news;
                    Match news_match;

                    // get news
                    string detik_regex = "<!-- e:pic detail -->\\s*<b>[^<]+</b>\\s*\\S*\\s*(.+)<strong>.+<!-- POLONG -->";
                    news_match = Regex.Match(page_html, detik_regex, RegexOptions.Singleline);
                    raw_news = news_match.Groups[1].Value;

                    raw_news = Regex.Replace(raw_news, "<[^>]+>", "", RegexOptions.Singleline); // remove remaining tag
                    raw_news = Regex.Replace(raw_news, "\\n", " "); // replace all newline with spaces
                    raw_news = Regex.Replace(raw_news, "^\\s+", ""); // remove trailing spaces at beginning of line 
                    raw_news = Regex.Replace(raw_news, "\\s+$", ""); // remove trailing spaces at end of line

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
            bool news_exist = news_list.Any(); // return true if list contains any element

            if (news_exist) // if list contains any element, update news (prevents data lost)
            {
                string news_json = new JavaScriptSerializer().Serialize(news_list); // convert List of News to JSON string
                System.IO.File.WriteAllText(filename, news_json); // write JSON string to JSON file
            }
        }
    }
}