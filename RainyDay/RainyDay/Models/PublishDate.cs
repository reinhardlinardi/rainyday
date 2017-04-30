using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RainyDay.Models
{
    public class PublishDate
    {
        public int year { get; set;} // year of publish date
        public int month { get; set; } // month of publish date
        public int date { get; set; } // date of publish date
        public int hour { get; set; } // hour of publish date
        public int minute { get; set; } // minute of publish date
        public int second { get; set; } // second of publish date

        public PublishDate(int _year, int _month, int _date, int _hour, int _minute, int _second)
        {
            year = _year;
            month = _month;
            date = _date;
            hour = _hour;
            minute = _minute;
            second = _second;
        }

        public static int ComparePublishDate(PublishDate a, PublishDate b) // is a later than b
        {
            if (a.year < b.year) return 1;
            else if (a.year > b.year) return -1;
            else
            {
                if (a.month < b.month) return 1;
                else if (a.month > b.month) return -1;
                else
                {
                    if (a.date < b.date) return 1;
                    else if (a.date > b.date) return -1;
                    else
                    {
                        if (a.hour < b.hour) return 1;
                        else if (a.hour > b.hour) return -1;
                        else
                        {
                            if (a.minute < b.minute) return 1;
                            else if (a.minute > b.minute) return -1;
                            else
                            {
                                if (a.second < b.second) return 1;
                                else if (a.second > b.second) return -1;
                                else return 0;
                            }
                        }
                    }
                }
            }
        }
    }
}