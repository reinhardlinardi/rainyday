using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RainyDay.Models
{
    public class News
    {
        public string title { get; set; } // news title
        public string date { get; set; } // news date
        public string link { get; set; } // news url
        public string content { get; set; } // news content

        // Constructor
        public News(string _title, string _date, string _link, string _content)
        {
            title = _title;
            date = _date;
            link = _link;
            content = _content;
        }
    }
}