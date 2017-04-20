using System;
using System.Text.RegularExpressions;

public class Algorithm
{
	private static string text;
    private static string pattern;
    private static string expr;

    public Algorithm()
	{
        text = System.IO.File.ReadAllText("input.txt");
        Console.WriteLine("String sumber: " + text);
        Console.Write("String yang ingin dicari: ");
        pattern = Console.ReadLine();
        Console.WriteLine("1. KMP");
        Console.WriteLine("2. Booyer Moore");
        Console.WriteLine("3. Regex");
        Console.Write("Input: ");
        int choice = Convert.ToInt32(Console.ReadLine());
        int posn;
        if (choice == 1)
        {
            posn = kmpMatch();
            if (posn == -1)
                Console.WriteLine("Pattern not found");
            else
                Console.WriteLine("Pattern starts at position " + posn);
                Console.WriteLine("Pattern ends at position " + (posn + pattern.Length - 1));
        }
        else if (choice == 2)
        {
            posn = bmMatch();
            if (posn == -1)
                Console.WriteLine("Pattern not found");
            else
                Console.WriteLine("Pattern starts at position " + posn);
                Console.WriteLine("Pattern ends at position " + (posn + pattern.Length - 1));
        }
        else if (choice == 3)
        {   
            buildRegex();
            posn = showMatch();
            if (posn == -1)
                Console.WriteLine("Pattern not found");
        }
        Console.ReadKey();
	}

    public static int[] computeFail()
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

    public static int kmpMatch() 
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
    
    public static int[] buildLast()
    {
        int[] last = new int[128];
        for (int i = 0; i < 128; i++)
            last[i] = -1;
        for (int i = 0; i < pattern.Length; i++)
            last[pattern[i]] = i;
        return last;
    }

    public static int bmMatch()
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

    private static int showMatch()
    {
        Regex r = new Regex(@expr, RegexOptions.IgnoreCase);
        Match m = r.Match(text);
        if (m.Success)
        {
            //Console.WriteLine(m);
            Capture c = m.Groups[0].Captures[0];
            Console.WriteLine("Real Text: " + c);
            Console.WriteLine("Pattern starts at position " + c.Index);
            Console.WriteLine("Pattern ends at position " + (c.Index + c.Length - 1));
            return 1;
        }
        else {
            return -1;
        }
    }

    private static void buildRegex()
    {   
        expr = System.Text.RegularExpressions.Regex.Replace(pattern,@"\s+","(?:\\s+[^\\s]+)*\\s+");
    }

    static void Main(string[] args) 
    {
        Algorithm kmp = new Algorithm();
    }
}