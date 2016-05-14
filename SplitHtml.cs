using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace HtmlTool
{
    public class SplitHtml
    {
        static List<string> OpenHeads;
        static SplitHtml()
        {
            OpenHeads = new List<string>() { "meta", "br","!--","link","!DOCTYPE" };
        }
        //static string PosFirstHead(int start,out int end,string htmlInput)
        //{

        //}
        //static string PosFirstEnd(int start,out int end,string htmlInput)
        //{

        //}
        static bool IsHead(int headStart,string htmlInput,string headName)
        {
            if (htmlInput[headStart] != '<')
                return false;
            for (int i = 1; i <= headName.Length; i++)
            {
                if(htmlInput[headStart+i]!=headName[i-1])
                {
                    return false;
                }
            }
            if(htmlInput[headStart+headName.Length+1]!=' '&&
                htmlInput[headStart+headName.Length+1]!='>')
            {
                return false;
            }
            return true;
        }
        static bool IsEnd(int endStart,string htmlInput,string endName)
        {
            string end = "</" + endName + ">";

            for(int i=0;i<end.Length;i++)
            {
                if(htmlInput[endStart+i].ToString().ToLower()!=end[i].ToString())
                {
                    return false;
                }
            }
            return true;
        }
        static string PosFirstHead(int start, out int end, string htmlInput,string head)
        {
            if(htmlInput==null)
            {
                throw new Exception("输入网页内容为空！请检查网址格式，或者你在耍我？！");
            }
            var index=htmlInput.IndexOf("<" + head + " ",start);
            var index1 = htmlInput.IndexOf("<" + head + ">", start);
            if(index==-1&&index1==-1)
            {
                end = index;
                return null;
            }
            if (index1 == -1 || (index!=-1&&index < index1))
            {
                end = htmlInput.IndexOf('>', index + head.Length + 1);
                return htmlInput.Substring(index, end - index + 1);
            }
            else
            {
                end = index1 + head.Length + 1;
                return "<" + head + ">";
            }
        }
        static int PosFirstEnd(int start, string htmlInput,string head)
        {
            var index = htmlInput.IndexOf("</" + head + ">",start);
            if(index==-1)
            {
                index = htmlInput.IndexOf("</" + head.ToUpper() + ">", start);
                if (index == -1)
                    return index;
                else
                    return index + head.Length + 2;
            }
            return index + head.Length + 2;
        }
        public static string CatchFirstTag(int start,out int end,string htmlInput,string tag)
        {
            //注释
            if (tag == "!--")
            {
                var comment = new Regex(@"<!-- .+-->").Match(htmlInput, start);
                if(comment.Success)
                {
                    end = comment.Index + comment.Length - 1;
                    return comment.Value;
                }
                else
                {
                    end = -1;
                    return null;
                }
            }
            //匹配第一个标签头
            var head=PosFirstHead(start, out start, htmlInput, tag);
            if(start==-1)
            {
                end = -1;
                return null;
            }
            //脚本
            if (tag == "script")
            {
                end = PosFirstEnd(start + 1, htmlInput, tag);
                if(end!=-1)
                {
                    return htmlInput.Substring(start - head.Length + 1, end - start + head.Length);
                }
                else
                {
                    return null;
                }
            }
            //匹配位置对应的标签尾
            int index = start+1;
            int level = 0;
            while (index != -1 && index < htmlInput.Length - 1)
            {
                if (htmlInput[index] != '<')
                {
                    index += 1;
                    continue;
                }
                if (IsHead(index, htmlInput, tag))
                {
                    if (index + PosFirstHead(index, out end, htmlInput, tag).Length - 1 == end)
                    {
                        level += 1;
                        index = end;
                        continue;
                    }
                }
                if (IsEnd(index, htmlInput, tag))
                {
                    if (level == 0)
                    {
                        end = PosFirstEnd(index, htmlInput, tag);
                        return htmlInput.Substring(start - head.Length + 1, end - start + head.Length);
                    }
                    else
                    {
                        index += tag.Length + 2;
                        level -= 1;
                    }
                }
                index += 1;
                
            }
            //返回标签头
            {
                end = start;
                return head;
            }
        }
        public static Tag MakeFirstTag(int start,out int end,string htmlInput,string tag)
        {
            Tag ret;
            //注释
            if (tag == "!--")
            {
                var comment = new Regex(@"<!-- .+-->").Match(htmlInput, start);
                if (comment.Success)
                {
                    end = comment.Index + comment.Length - 1;
                    return new Tag()
                    {
                        contentL = comment.Value,
                        contentR=comment.Value,
                        head="!--",
                        position=comment.Index
                    };
                }
                else
                {
                    end = -1;
                    return null;
                }
            }
            //匹配第一个标签头
            var head = PosFirstHead(start, out start, htmlInput, tag);
            if (start == -1)
            {
                end = -1;
                return null;
            }
            //脚本
            if (tag == "script")
            {
                end = PosFirstEnd(start + 1, htmlInput, tag);
                if (end != -1)
                {
                    ret = SplitNoCloseTag(head);
                    ret.contentL = ret.contentR = htmlInput.Substring(start, end - start - 7);
                    ret.position = start - head.Length+1;
                    return ret;
                }
                else
                {
                    return null;
                }
            }
            //匹配位置对应的标签尾
            int index = start + 1;
            int level = 0;
            while (index != -1 && index < htmlInput.Length - 1)
            {
                if (htmlInput[index] != '<')
                {
                    index += 1;
                    continue;
                }
                if (IsHead(index, htmlInput, tag))
                {
                    if (index + PosFirstHead(index, out end, htmlInput, tag).Length - 1 == end)
                    {
                        level += 1;
                        index = end;
                        continue;
                    }
                }
                if (IsEnd(index, htmlInput, tag))
                {
                    if (level == 0)
                    {
                        end = PosFirstEnd(index, htmlInput, tag);
                        ret= SplitTag(htmlInput.Substring(start - head.Length + 1, end - start + head.Length));
                        ret.position = start - head.Length + 1;
                        return ret;
                    }
                    else
                    {
                        index += tag.Length + 2;
                        level -= 1;
                    }
                }
                index += 1;
            }
            //返回标签头
            {
                end = start;
                if (head[head.Length - 2] == '/')
                {
                    ret= SplitSpecialTag(head);
                }
                else
                {
                    ret= SplitNoCloseTag(head);
                }
                ret.position = start - head.Length + 1;
                return ret;
            }

        }
        //public static string CatchFirstTag(int start, out int end, string htmlInput)
        //{

        //}
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
            int firstLeftIndex = tagContent.IndexOf('<');
            int firstRightIndex = tagContent.IndexOf('>');
            int lastLeftIndex = tagContent.LastIndexOf('<');
            int lastRightIndex = tagContent.LastIndexOf('>');
            string lastname = tagContent.Substring(lastLeftIndex + 2, lastRightIndex - lastLeftIndex - 2);
            string head = tagContent.Substring(firstLeftIndex + 1, firstRightIndex - firstLeftIndex - 1);
            string content = tagContent.Substring(firstRightIndex + 1, lastLeftIndex - firstRightIndex - 1);
            var splittedHead = head.Split(new char[] { ' ' }, 2);
            var splittedAttris = splittedHead.Count() < 2 ? new string[] { } : Regex.Split(splittedHead[1], @"(?<="")\s+");
            if (splittedHead[0].ToLower() != lastname.ToLower())
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
                tag.contentL = start == 0 ? null : content.Substring(0, start);
                tag.contentR = end == content.Count() - 1 ? null : content.Substring(end + 1, content.Count() - end - 2);
                tag.TagsInTag = splittedTags(content.Substring(start, end - start + 1));
            }
            return tag;
        }
        static string MacthOpen(int start, out int end, string html)
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
        /// 分解当前级别并列标签体
        /// </summary>
        /// <param name="tags">网页内容</param>
        /// <returns>分解的标签集</returns>
        static Tag[] splittedTags(string tags)
        {
            List<Tag> ret = new List<Tag>();
            int end;
            var head = MacthOpen(0, out end, tags);
            if (head == null) return ret.ToArray();
            var close = MakeFirstTag(0, out end, tags,head);
            if (close == null) return ret.ToArray();
            ret.Add(close);
            while (end != tags.Count() && end != -1)
            {
                int next;
                head = MacthOpen(end, out next, tags);
                if (head == null) break;
                close = MakeFirstTag(end, out end, tags,head);
                if (close == null) break;
                ret.Add(close);
            }
            return ret.ToArray();
        }
        //static Tag SplitTag(string htmlContent, int startIndex,string head)
        //{
        //    if (!IsHead(startIndex, htmlContent, head)) return new Tag();
        //    int end = -1;
        //    var headContent=PosFirstHead(startIndex, out end,htmlContent, head);
        //    if (end == -1) throw new Exception("网页内容不包括标签" + head);
        //    var tag = SplitNoCloseTag(headContent);
        //    tag.TagsInTag = SplittedTags(htmlContent, end);
        //    return tag;
        //}
        //static Tag[] SplittedTags(string htmlContent, int startIndex)
        //{

        //}
    }
    /// <summary>
    /// 
    /// </summary>
    public class Tag
    {
        public Tag() { }
        public int position { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string head { get; set; }
        public Tag[] TagsInTag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contentL { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contentR { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> attris { get; set; }
    }

}
