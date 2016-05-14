using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace HtmlTool
{
    public class SplittedHtml
    {
        static List<string> OpenHeads;
        static SplittedHtml()
        {
            OpenHeads = new List<string>() { "meta", "br","!--","link","!DOCTYPE" };
        }
        /// <summary>
        /// 抓取html文档的特定标签
        /// </summary>
        /// <param name="html">html文档内容</param>
        /// <param name="tag">标签名</param>
        /// <returns>返回文档里所有符合的标签内容块</returns>
        public static string[] FindTags(string html,string tag)
        {
            List<string> tags = new List<string>();
            int nextIndex = -1;
            string myTag=null;
            myTag = MacthClose(tag, 0, out nextIndex, html);
            while (myTag != null)
            {
                tags.Add(myTag);
                myTag = MacthClose(tag, nextIndex, out nextIndex, html);
            }
            return tags.ToArray();
        }
        /// <summary>
        /// 查找网页内容中的所有一级并列标签内容
        /// </summary>
        /// <param name="html">网页内容</param>
        /// <returns>一级并列标签块</returns>
        static string[] FindTags(string html)
        {
            List<string> tags=new List<string>();
            int end=-1;
            while (end < html.Count())
            {
                string TagName = MacthOpen(0, out end, html);
                if (TagName != null)
                {
                    string close = MacthClose(TagName, end, out end, html);
                    if (close == null) break;
                    tags.Add(close);

                }
            }
            return tags.ToArray();
        }
        /// <summary>
        /// 查找网页的第一个一级标签
        /// </summary>
        /// <param name="html">网页内容</param>
        /// <returns></returns>
        public static string FindTag(string html)
        {
            int end=-1;
            string TagName=MacthOpen(0,out end,html);
            if(TagName!=null)
            {
                return MacthClose(TagName, end, out end, html);
            }
            return null;
        }
        /// <summary>
        /// 匹配一个标签块
        /// </summary>
        /// <param name="tag">要匹配的标签名</param>
        /// <param name="start">开始位置 标签头的左标签</param>
        /// <param name="end">匹配结束位置</param>
        /// <param name="html">要搜索的字符串内容</param>
        /// <returns>返回整个标签块</returns>
        static string MacthClose(string tag,int start, out int end,string html)
        {
            //debug
            List<string> add = new List<string>();
            List<string> haha = new List<string>();

            end = -1;
            int level = 0;
            Regex TagRegex = new Regex(@"<" + tag + @"(\s+[^<>]+)*>");
            string TagClose = @"</" + tag + @">";
            Regex TagCloseRegex = new Regex(@"^</" + tag + @">");
            Match match = TagRegex.Match(html, start);
            if (tag == "script")
            {
                if (match.Success)
                {
                    var script = new Regex(TagClose).Match(html, match.Index + match.Length);
                    if (script.Success)
                    {
                        end = script.Index + script.Length;
                        string it = html.Substring(match.Index, script.Index + script.Length - match.Index);
                        return it;
                    }
                }
            }
            if(!match.Success)
            {
                var TagSpecial = new Regex(@"<" + tag + @"(\s+[^<>]+)*/>");
                var special = TagSpecial.Match(html, start);
                if(special.Success)
                {
                    end = special.Index + special.Length;
                    return special.Value;
                }
                else
                {
                    return null;
                }
            }
            for (int index = match.Index + match.Length; index < html.Length; index++)
            {
                if (html[index] == '<')
                {
                    if (html[index + 1] == '/')
                    {
                        //var close = TagCloseRegex.Match(html, index);
                        var close = new Regex(@"</([^<>/]+)>").Match(html, index);
                        if (close.Success)
                        {

                        }
                        if (close.Groups[1].Value == tag)
                        {
                            var a1=add.GroupBy(x => x).Select(x=>x).OrderBy(x=>x.Key).ToList();
                            var h1 = haha.GroupBy(x => x).Select(x=>x).OrderBy(x=>x.Key).ToList();
                            foreach (var a in a1)
                            {
                                if (!h1.Contains(a))
                                {

                                }
                            }
                        }
                        if (close.Groups[1].Value==tag && level == 0)
                        {
                            string it = html.Substring(match.Index, close.Index + close.Length - match.Index);
                            end = close.Index + close.Length;
                            return it;
                        }
                        else
                        {
                            level -= 1; haha.Add(close.Groups[1].Value);
                        }
                    }
                    else
                    {
                        int nextIndex=-1;
                        var open=MacthOpen(index, out nextIndex, html);
                        if (open != null)
                        {
                            var head=open.Split(' ')[0];
                            if (head == "script")
                            {
                                var script = new Regex(@"</script>").Match(html, nextIndex);
                                if (script.Success)
                                {
                                    index = script.Index + script.Length-1;
                                    continue;
                                }
                            }
                            if (head == "!--")
                            {
                                index = nextIndex-1;
                                continue;
                            }
                            if (!OpenHeads.Contains(head))
                            {
                                level += 1; add.Add(head);
                                continue;
                            }

                            //index = nextIndex - 1;
                            //open = MacthClose(head, nextIndex, out nextIndex, html);
                            //if (open != null)
                            //{
                            //    level += 1;
                            //}
                        }
                    }
                }

            }
            //如果没有匹配
            {
               // MessageBox.Show("add:" + add + "\n jianshao:" + haha);
                end = match.Index + match.Length;
                return match.Value;
            }
        }
        /// <summary>
        /// 匹配并分解一个标签体
        /// </summary>
        /// <param name="tag">标签头</param>
        /// <param name="start">匹配开始位置</param>
        /// <param name="end">匹配结束位置 匹配失败设为-1</param>
        /// <param name="html">网页主体</param>
        /// <returns>返回分解的标签体</returns>
        public static Tag MacthTag(string tag, int start, out int end, string html)
        {
            end = -1;
            int level = 0;
            Regex TagRegex = new Regex(@"<" + tag + @"(\s+[^<>]+)*(?<!/)>");
            Regex TagCloseRegex = new Regex(@"</" + tag + @">");
            Match match = TagRegex.Match(html, start);
            var test = html[start];
            if (!match.Success)
            {
                var TagSpecial = new Regex(@"<" + tag + @"(\s+[^<>]+)*/>");
                var special = TagSpecial.Match(html, start);
                if (special.Success)
                {
                    end = special.Index + special.Length;
                    return SplitSpecialTag(special.Value);
                }
                else
                {
                    return null;
                }
            }
            for (int index = match.Index + match.Length; index < html.Length; index++)
            {
                if (html[index] == '<')
                {
                    if (html[index + 1] == '/')
                    {
                        var close = TagCloseRegex.Match(html, index);
                        if (close.Success && level == 0)
                        {
                            string it = html.Substring(match.Index, close.Index + close.Length - match.Index);
                            end = close.Index + close.Length;
                            return SplitTag(it);
                        }
                        else
                        {
                            level -= 1;
                        }
                    }
                    else
                    {
                        int nextIndex = -1;
                        var open = MacthOpen(index, out nextIndex, html);
                        if (open != null)
                        {
                            var head = open.Split(' ')[0];
                            if (!OpenHeads.Contains(head))
                            {
                                level += 1;
                                continue;
                            }

                            //index = nextIndex - 1;
                            //open = MacthClose(head,nextIndex, out nextIndex, html);
                            //if (open != null)
                            //{
                            //    level += 1;
                            //}
                        }
                    }
                }

            }
            //如果没有匹配
            {
                end = match.Index + match.Length;
                return SplitNoCloseTag(match.Value);
            }
        }
        /// <summary>
        /// 匹配最开始的标签头 
        /// </summary>
        /// <param name="start">开始搜索处的字符索引</param>
        /// <param name="end">匹配结尾处的索引</param>
        /// <param name="html">要搜索的内容</param>
        /// <returns>返回切分好的标签头，第一个为标签名称，其后为属性</returns>
        static string MacthOpen(int start,out int end,string html)
        {
            Regex TagRegex = new Regex(@"<([^<>/\s]+)(\s+[^<>]+)*>");
            var match = TagRegex.Match(html, start);
            if (match.Success)
            {
                end = match.Index + match.Length;
                return match.Groups[1].Value;
            }
            else
            {
                //注释
                TagRegex = new Regex(@"<!-- .*-->");
                match = TagRegex.Match(html, start);
                if (match.Success)
                {
                    end = match.Index + match.Length;
                    return match.Groups[1].Value;
                }
            }
            end = -1;
            return null;
        }
        /// <summary>
        /// 分解没有尾标签，也没有尾斜杠标志的标签体<标签>
        /// </summary>
        /// <param name="tagContent">标签块</param>
        /// <returns>分解标签类</returns>
        static Tag SplitNoCloseTag(string tagContent)
        {
            Tag ret = new Tag();
            int firstLeftIndex = tagContent.IndexOf('<');
            int firstRightIndex = tagContent.IndexOf('>');
            string head = tagContent.Substring(firstLeftIndex + 1, firstRightIndex - firstLeftIndex - 1);
            var splittedHead = head.Split(new char[] { ' ' }, 2);
            var splittedAttris = splittedHead.Count() < 2 ? new string[] { } : Regex.Split(splittedHead[1], @"(?<="")\s+");
            ret.attris = new Dictionary<string, string>();
            ret.head = splittedHead[0];
            for (int i = 0; i < splittedAttris.Count(); i++)
            {
                var attri = splittedAttris[i].Split(new char[] { '=' }, 2);
                if (attri.Count() == 2)
                {
                    ret.attris.Add(attri[0], attri[1]);
                }
            }
            return ret;

        }
        /// <summary>
        /// 分解没有尾标签，有尾斜杠标志的标签体<标签/>
        /// </summary>
        /// <param name="tagContent">标签块</param>
        /// <returns>分解的标签类</returns>
        static Tag SplitSpecialTag(string tagContent)
        {
            Tag ret = new Tag();
            int firstLeftIndex = tagContent.IndexOf('<');
            int firstRightIndex = tagContent.IndexOf('>');
            string head = tagContent.Substring(firstLeftIndex + 1, firstRightIndex - firstLeftIndex - 2);
            var splittedHead = head.Split(new char[] { ' ' }, 2);
            var splittedAttris = splittedHead.Count() < 2 ? new string[] { } : Regex.Split(splittedHead[1], @"(?<="")\s+");
            ret.attris = new Dictionary<string, string>();
            ret.head = splittedHead[0];
            for (int i = 0; i < splittedAttris.Count(); i++)
            {
                var attri = splittedAttris[i].Split(new char[] { '=' }, 2);
                if (attri.Count() == 2)
                {
                    ret.attris.Add(attri[0], attri[1]);
                }
            }
            return ret;
        }
        /// <summary>
        /// 分解正常标签块
        /// </summary>
        /// <param name="tagContent">标签体</param>
        /// <returns>分解的标签体</returns>
        static Tag SplitTag(string tagContent)
        {
            if (tagContent == null) { return new Tag(); }
            int firstLeftIndex=tagContent.IndexOf('<');
            int firstRightIndex = tagContent.IndexOf('>');
            int lastLeftIndex = tagContent.LastIndexOf('<');
            int lastRightIndex = tagContent.LastIndexOf('>');
            string lastname = tagContent.Substring(lastLeftIndex + 2, lastRightIndex - lastLeftIndex - 2);
            string head = tagContent.Substring(firstLeftIndex+1, firstRightIndex-firstLeftIndex-1);
            string content = tagContent.Substring(firstRightIndex + 1, lastLeftIndex - firstRightIndex - 1);
            var splittedHead = head.Split(new char[] { ' ' }, 2);
            var splittedAttris=splittedHead.Count()<2?new string[]{}:Regex.Split(splittedHead[1], @"(?<="")\s+");
            if (splittedHead[0] != lastname) 
            { MessageBox.Show("标签名首尾不一致！"); return null; }
            Tag tag = new Tag();
            tag.attris = new Dictionary<string, string>();
            tag.head = splittedHead[0];
            for (int i = 0; i < splittedAttris.Count(); i++)
            {
                var attri = splittedAttris[i].Split(new char[] { '=' }, 2);
                if (attri.Count() == 2)
                {
                    tag.attris.Add(attri[0], attri[1]);
                }
            }
            if (!content.Contains('<'))
            {
                tag.contentL = tag.contentR = content;
                return tag;
            }
            else
            {
                int start = content.IndexOf('<');
                int end = content.LastIndexOf('>');
                tag.contentL =start==0?null: content.Substring(0, start);
                tag.contentR =end==content.Count()-1?null: content.Substring(end + 1, content.Count() - end - 2);
                tag.TagsInTag = splittedTags(content.Substring(start, end - start+1));
            }
            return tag;
        }
        /// <summary>
        /// 分解当前级别并列标签体
        /// </summary>
        /// <param name="tags">网页内容</param>
        /// <returns>分解的标签集</returns>
        static Tag[] splittedTags(string tags)
        {
            List<Tag> ret=new List<Tag>();
            int end;
            var head = MacthOpen(0, out end, tags);
            if (head == null) return ret.ToArray();
            var close = MacthTag(head, 0, out end, tags);
            if (close == null) return ret.ToArray();
            ret.Add(close);
            while (end != tags.Count()&&end!=-1)
            {
                int next;
                head = MacthOpen(end, out next, tags);
                if (head == null) break;
                close = MacthTag(head, end, out end, tags);
                if (close == null) break;
                ret.Add(close);
            }
            return ret.ToArray();
        }
    }
    ///// <summary>
    ///// 
    ///// </summary>
    //public class Tag
    //{
    //    public Tag() { }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string head { get; set; }
    //    public Tag[] TagsInTag{get;set;}
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string contentL{get;set;}
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string contentR{get;set;}
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public Dictionary<string,string> attris{get;set;}

    //}
}
