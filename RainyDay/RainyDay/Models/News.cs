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

        // Constructor
        public News(string _title, string _date, string _link)
        {
            title = _title;
            date = _date;
            link = _link;
        }
    }
}