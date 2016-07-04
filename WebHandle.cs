using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace HtmlTool
{
    public class WebHandle
    {
        public WebHandle()
        {
            client = new WebClient();

            // Add a user agent header in case the 
            // requested URI contains a query.

            client.Headers.Add
                ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

        }
        WebClient client;
       public  string GetHTMLPage(string url)
        {
            try
            {
                Stream data = client.OpenRead(url);
                StreamReader reader = new StreamReader(data,true);
                string s = reader.ReadToEnd();
                data.Close();
                reader.Close();
                return s;
            }
            catch { return null; }
        }
       public static string getRootUrl(string url)
        {
            return Regex.Match(url, @"(http[s]{0,1}:\/\/[^/]+)\/").Groups[1].Value;
        }

    }
}
