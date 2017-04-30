using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using RainyDay.Models;

namespace RainyDay
{
    public class NewsSorter : IComparer<News>
    {
        public int MonthToInt(string month)
        {
            int value = 0;

            switch (month)
            {
                case "Jan":
                    value = 1;
                    break;

                case "Feb":
                    value = 2;
                    break;

                case "Mar":
                    value = 3;
                    break;

                case "Apr":
                    value = 4;
                    break;

                case "May":
                    value = 5;
                    break;

                case "Jun":
                    value = 6;
                    break;

                case "Jul":
                    value = 7;
                    break;

                case "Aug":
                    value = 8;
                    break;

                case "Sep":
                    value = 9;
                    break;

                case "Oct":
                    value = 10;
                    break;

                case "Nov":
                    value = 11;
                    break;

                case "Dec":
                    value = 12;
                    break;
            }

            return value;
        }

        public int Compare(News a, News b)
        {
            // RSS Date : Sun, 30 Apr 2017 00:14:01 + 0700
            string publish_time_regex = "^\\S+\\s+(\\d{2})\\s+(\\w{3})\\s+(\\d{4})\\s+(\\d{2}):(\\d{2}):(\\d{2})";
            Match matcher;

            matcher = Regex.Match(a.date, publish_time_regex);
            int a_date= Convert.ToInt32(matcher.Groups[1].Value);
            int a_month = MonthToInt(matcher.Groups[2].Value);
            int a_year = Convert.ToInt32(matcher.Groups[3].Value);
            int a_hour = Convert.ToInt32(matcher.Groups[4].Value);
            int a_minute = Convert.ToInt32(matcher.Groups[5].Value);
            int a_second = Convert.ToInt32(matcher.Groups[6].Value);

            PublishDate a_publish_date = new PublishDate(a_year, a_month, a_date, a_hour, a_minute, a_second);

            matcher = Regex.Match(b.date, publish_time_regex);
            int b_date = Convert.ToInt32(matcher.Groups[1].Value);
            int b_month = MonthToInt(matcher.Groups[2].Value);
            int b_year = Convert.ToInt32(matcher.Groups[3].Value);
            int b_hour = Convert.ToInt32(matcher.Groups[4].Value);
            int b_minute = Convert.ToInt32(matcher.Groups[5].Value);
            int b_second = Convert.ToInt32(matcher.Groups[6].Value);

            PublishDate b_publish_date = new PublishDate(b_year, b_month, b_date, b_hour, b_minute, b_second);

            return PublishDate.ComparePublishDate(a_publish_date, b_publish_date);
        }
    }
}