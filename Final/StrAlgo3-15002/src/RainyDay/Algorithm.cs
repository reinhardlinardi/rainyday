using System;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using RainyDay.Models;

namespace RainyDay
{
    public class Algorithm
    {
	    public string text;
        public string pattern;
        public string expr;
        /** Melakukan pengecekan News di dalam news_list dan memasukkan hasil pengecekan ke dalam news_match. 
          */
        public void StringMatching(string keyword, string algorithm, List<News> news_list, List<NewsMatch> news_match)
	    {
            foreach (News news in news_list)
            {
                text = news.content;
                pattern = keyword;
                NewsMatch berita = new NewsMatch();

                if (algorithm.Equals("KMP"))
                {
                    berita.start = kmpMatch(); // pencocokan isi berita
                    if (berita.start == -1) // isi not match
                    {
                        text = news.title;
                        berita.start = kmpMatch(); // pencocokan isi judul
                        if (berita.start > -1) // judul match
                        {
                            berita.isContent = 0;
                            berita.end = (berita.start + pattern.Length - 1);
                        }
                    }
                    else
                    {
                        berita.end = (berita.start + pattern.Length - 1);
                        berita.isContent = 1;
                    }
                }
                else if (algorithm.Equals("Boyer-Moore"))
                {
                    berita.start = bmMatch(); // pencocokan isi berita
                    if (berita.start == -1) // isi not match
                    {
                        text = news.title;
                        berita.start = bmMatch(); // pencocokan isi judul
                        if (berita.start > -1) // judul match
                        {
                            berita.isContent = 0;
                            berita.end = (berita.start + pattern.Length - 1);
                        }
                    }
                    else
                    {
                        berita.end = (berita.start + pattern.Length - 1);
                        berita.isContent = 1;
                    }
                }
                else
                {
                    int[] arrInt = new int[2];
                    buildRegex();
                    showMatch(news_match,arrInt); // pencocokan isi berita
                    if (arrInt[0] == -1) // isi not match
                    {
                        text = news.title;
                        buildRegex();
                        showMatch(news_match, arrInt); // pencocokan isi judul
                        if (arrInt[0] > -1) // judul match
                        {
                            berita.start = arrInt[0];
                            berita.end = arrInt[1];
                            berita.isContent = 0;
                        }
                    }
                    else
                    {
                        berita.start = arrInt[0];
                        berita.end = arrInt[1];
                        berita.isContent = 1;
                    }
                }
                news_match.Add(berita);
            }
	    }

        public int[] computeFail()
        {
            int[] fail = new int [pattern.Length];
            fail[0] = 0;
            int m = pattern.Length;
            int j = 0;
            int i = 1;
            while (i < m)
            {
                if (pattern[j] == pattern[i])
                {
                    fail[i] = j + 1;
                    i++;
                    j++;
                }
                else if (j > 0)
                    j = fail[j - 1];
                else
                {
                    fail[i] = 0;
                    i++;
                }
            }
            return fail;
        }

        /** Melakukan pencocokan pattern dengan string text menggunakan KMP. 
          */
        public int kmpMatch() 
        {
            int n = text.Length;
            int m = pattern.Length;
            int[] fail = computeFail();
            int i = 0;
            int j = 0;
            while (i < n)
            {
                if (pattern[j] == text[i])
                {
                    if (j == m - 1)
                        return i - m + 1;
                    i++;
                    j++;
                }
                else if (j > 0)
                    j = fail[j - 1];
                else
                    i++;
            }
            return -1;
        }   
    
        public int[] buildLast()
        {
            int[] last = new int[128];
            for (int i = 0; i < 128; i++)
                last[i] = -1;
            for (int i = 0; i < pattern.Length; i++)
                last[pattern[i]] = i;
            return last;
        }

        /** Melakukan pencocokan pattern dengan string text menggunakan Boyer Moore. 
          */
        public int bmMatch()
        {
            int[] last = buildLast();
            int n = text.Length;
            int m = pattern.Length;
            int i = m - 1;
            if (i > n - 1)
                return -1;
            int j = m - 1;
            do
            {
                if (pattern[j] == text[i])
                {
                    if (j == 0)
                        return i;
                    else
                    {
                        i--;
                        j--;
                    }
                }
                else
                {
                    int lo = last[text[i]];
                    i = i + m - Math.Min(j, 1+lo);
                    j = m - 1;
                } 
            }
            while (i <= n - 1);
            return -1;
        }

        /** Melakukan pencocokan pattern dengan string text menggunakan Regex. 
          */
        public void showMatch(List<NewsMatch> news_match, int[] arrInt)
        {
            Regex r = new Regex(@expr, RegexOptions.IgnoreCase);
            Match m = r.Match(text);
            if (m.Success)
            {
                Capture c = m.Groups[0].Captures[0];
                arrInt[0] = (c.Index);
                arrInt[1] = (c.Index + c.Length - 1);
            }
            else
            {
                arrInt[0] = -1;
                arrInt[1] = -1;
            }
        }

        /** Membangun regex untuk melakukan pencarian pattern di dalam text. 
          */
        public void buildRegex()
        {   
            expr = Regex.Replace(pattern,@"\s+","(?:\\s+[^\\s]+)*\\s+");
        }
    }
}