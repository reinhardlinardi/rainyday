using System;
using System.Xml;

namespace GetNews
{
    class Program
    {
        /*static void Main(string[] args)
        {
            string[] urls = System.IO.File.ReadAllLines("NewsWebsites.txt");

            foreach (string url in urls)
            {
                Console.WriteLine(url);
                Console.WriteLine();

                XmlReader xml_reader = XmlReader.Create(url);
                SyndicationFeed feed = SyndicationFeed.Load(xml_reader);

                xml_reader.Close();
                int cnt = 0;

                foreach(SyndicationItem item in feed.Items)
                {
                    cnt++;

                    string subject = item.Title.Text;
                    string published = item.PublishDate.ToString();
                    string link = item.Links.First().Uri.ToString();

                    Console.WriteLine("Berita #" + cnt + ":");
                    Console.WriteLine("Subject : " + subject);
                    Console.WriteLine("Published : " + published);
                    Console.WriteLine("Link : " + link);
                    Console.WriteLine();
                }
            }
            
            //Insert Algorithm here
            Algorithm kmp = new Algorithm();

            
            //Console.WriteLine("Press any key to continue");
            //Console.ReadKey();
        }*/
    }
}