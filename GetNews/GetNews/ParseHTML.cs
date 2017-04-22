using System;
using System.Collections.Generic;
using System.Xml;
using System.ServiceModel.Syndication;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Parser
{
    class Parser
    {
        public static List<SyndicationFeed> feeds;
        public static List<News> newslist;
        public static List<string> htmlurls;
        static void Main(string[] args)
        {
            newslist = new List<News>();
            feeds = new List<SyndicationFeed>();
            htmlurls = new List<string>();

            List<string> urls = new List<string>();
            urls.Add("http://rss.detik.com/index.php");
            urls.Add("http://rss.vivanews.com/get/all"); 
            //urls.Add("http://www.antaranews.com/rss/terkini"); 
            //urls.Add("https://www.tempo.co/rss/terkini");  

            Parser parser = new Parser();

            /* XML Parser and HTML Parser*/
            parser.XMLParser(urls);
            parser.HTMLParser(htmlurls);

            foreach (var elmt in newslist)
            {
                Console.WriteLine(elmt.title);
                Console.WriteLine(elmt.summary);
                Console.WriteLine(elmt.imagelink);
                Console.WriteLine(elmt.link);
                Console.WriteLine(elmt.description);
                Console.WriteLine("\n");
            }

            Console.ReadKey();
        }
        public void HTMLParser(List<string> urls)
        {
            int i = -1;
            HtmlWeb web = new HtmlWeb();
            foreach (string url in htmlurls)
            {
                HtmlDocument document = web.Load(url);
                string result = "";

                i++;
                if (Regex.IsMatch(url, ".*detik.com"))
                {
                    /* Removing Comment */
                    var commentnodes = document.DocumentNode.SelectNodes("//comment()");
                    if (commentnodes != null)
                    {
                        foreach (HtmlNode comment in commentnodes)
                        {
                            comment.ParentNode.RemoveChild(comment);
                        }
                    }

                    /* Removing Script */
                    var scriptnodes = document.DocumentNode.SelectNodes("//script");
                    if (scriptnodes != null)
                    {
                        foreach (HtmlNode script in scriptnodes)
                        {
                            script.ParentNode.RemoveChild(script);
                        }
                    }

                    var divnodes = document.DocumentNode.SelectNodes("//div[@id]");
                    if (divnodes != null)
                    {
                        foreach (HtmlNode divNode in document.DocumentNode.SelectNodes("//div[@id]"))
                        {
                            HtmlAttribute attribute = divNode.Attributes["id"];
                            if (attribute.Value == "detikdetailtext")
                            {
                                result = divNode.InnerText;
                            }
                        }
                    }
                }
                else if (Regex.IsMatch(url, ".*viva.co.id"))
                {

                    foreach (HtmlNode divNode in document.DocumentNode.SelectNodes("//span[@itemprop]"))
                    {
                        HtmlAttribute attribute = divNode.Attributes["itemprop"];
                        if (attribute.Value == "description")
                        {
                            result = divNode.InnerText;
                            break;
                        }
                    }
                }
                else if (Regex.IsMatch(url, ".*antaranews.com"))
                {
                    Console.WriteLine("Antara");
                    /* Removing Comment */
                    var commentnodes = document.DocumentNode.SelectNodes("//comment()");
                    if (commentnodes != null)
                    {
                        foreach (HtmlNode comment in commentnodes)
                        {
                            comment.ParentNode.RemoveChild(comment);
                        }
                    }

                    /* Removing Script */
                    var scriptnodes = document.DocumentNode.SelectNodes("//script");
                    if (scriptnodes != null)
                    {
                        foreach (HtmlNode script in scriptnodes)
                        {
                            script.ParentNode.RemoveChild(script);
                        }
                    }
                    foreach (HtmlNode divNode in document.DocumentNode.SelectNodes("//div[@id]"))
                    {
                        HtmlAttribute attribute = divNode.Attributes["id"];
                        if (attribute.Value == "content_news")
                        {
                            result = divNode.InnerText;
                            break;
                        }
                    }
                    
                    /* Reformating */
                    int indexof;
                    indexof = result.IndexOf("     ");
                    while (indexof != -1)
                    {
                        result = result.Remove(indexof, 4);
                        indexof = result.IndexOf("     ");
                    }
                }

                Console.WriteLine(i);
                string description = Regex.Replace(result, @"\r|\n|\t", "");
                description = description.Trim();

                newslist[i].description = description;
            }
        }
        public void XMLParser(List<string> urls)
        {
            foreach (string url in urls)
            {
                XmlReader x = XmlReader.Create(url);
                feeds.Add(SyndicationFeed.Load(x));
                x.Close();
            }
            foreach (SyndicationFeed elmt in feeds)
            {
                foreach (SyndicationItem item in elmt.Items)
                {
                    News news = new News();

                    if (item != null)
                    {
                        news.title = item.Title.Text;

                        htmlurls.Add(item.Links[0].Uri.ToString());
                        news.link = item.Links[0].Uri.ToString();
                        news.imagelink = item.Links.Count > 1 ? item.Links[1].Uri.ToString() : "";
                        news.summary = item.Summary.Text;
                        int awal, akhir;
                        awal = news.summary.IndexOf('<');
                        akhir = news.summary.IndexOf('>');
                        if (awal != -1)
                        {
                            news.summary = news.summary.Remove(awal, akhir - awal + 1);
                        }
                        newslist.Add(news);
                    }
                }
            }
        }
    }
    class News
    {
        public string title;
        public string summary;
        public string imagelink;
        public string link;
        public string description;
    }

}
