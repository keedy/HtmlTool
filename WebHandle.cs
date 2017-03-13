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
        static WebHandle()
        {
            client = new WebClient();

            // Add a user agent header in case the 
            // requested URI contains a query.

            client.Headers.Add
                ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

        }
        static WebClient client;
        public static string GetHTMLPage(string url,string charSet)
        {
            try
            {
                var myDataBuffer = client.DownloadData(url);
                var strWebData = Encoding.Default.GetString(myDataBuffer);
                //获取网页字符编码描述信息
                Match charSetMatch = Regex.Match(strWebData, "<meta([^<]*)charset=\"?([^<]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string webCharSet = charSetMatch.Groups[2].Value;
                if (charSet == null || charSet == "")
                {
                //如果未获取到编码，则设置默认编码
                if (webCharSet == null || webCharSet == "")
                {
                charSet = "UTF-8";
                }
                else
                {
                charSet = webCharSet;
                }
                }
                if (charSet != null && charSet != "" && Encoding.GetEncoding(charSet) != Encoding.Default)
                {
                strWebData = Encoding.GetEncoding(charSet).GetString(myDataBuffer);
                }
                return strWebData;
            }
            catch { return null; }
        }
       public static string GetHTMLPage(string url)
       {
           try
           {
               Stream data = client.OpenRead(url);
               StreamReader reader = new StreamReader(data, true);
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
