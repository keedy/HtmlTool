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
        public SplitHtml(string htmlOrgCode)
        {
            this.HTMLCode=htmlOrgCode;
        }
        static bool IsHead(int headStart,string htmlInput,string headName)
        {
            if (htmlInput[headStart] != '<')
                return false;
            for (int i = 1; i <= headName.Length; i++)
            {
                if(htmlInput[headStart+i].ToString().ToLower()!=headName[i-1].ToString().ToLower())
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
                if(htmlInput[endStart+i].ToString().ToLower()!=end[i].ToString().ToLower())
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
            var index=htmlInput.ToLower().IndexOf("<" + head.ToLower() + " ",start);
            var index1 = htmlInput.ToLower().IndexOf("<" + head.ToLower() + ">", start);
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
            var index = htmlInput.ToLower().IndexOf("</" + head.ToLower() + ">",start);
            if(index==-1)
            {
                index = htmlInput.ToLower().IndexOf("</" + head.ToLower() + ">", start);
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
                var comment = new Regex(@"<!-- (.+)-->").Match(htmlInput, start);
                if(comment.Success)
                {
                    end = comment.Index + comment.Length - 1;
                    return comment.Groups[1].Value;
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
                    return htmlInput.Substring(start +1, end - start -9);
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
                        return htmlInput.Substring(start + 1, end - start - tag.Length - 3);
                    }
                    else
                    {
                        index += tag.Length + 2;
                        level -= 1;
                    }
                }
                index += 1;
                
            }
            ////返回标签头
            //{
            //    end = start;
            //    return head;
            //}
            end = start;
            return null;
        }
        string HTMLCode;
        public TAGBlock SplitTo()
        {
            return SplitTo(this.HTMLCode);
        }
        public static TAGBlock SplitTo(string htmlOrgCode)
        {
            var TAGs=Regex.Matches(htmlOrgCode, @"<[^<>]+>", RegexOptions.Multiline);
            TAGBlock startNode=new TAGBlock();
            TAGBlock current = startNode;
            int end = 0;
            if(TAGs.Count==0)
            {
                TAGBlock.SetNULL(startNode, htmlOrgCode);
                return startNode;
            }
            for(int i=0;i<TAGs.Count;i++)
            {
                if (TAGs[i].Index < end)
                    continue;
                if(TAGs[i].Index>end)
                {
                    TAGBlock.SetNULL
                        (current, htmlOrgCode.Substring
                        (end + 1, TAGs[i].Index - end - 1));
                    current.NextBlock = new TAGBlock();
                    current = current.NextBlock;
                }
                var type = TAG.GetType(TAGs[i].Value);
                var name = TAG.GetName(TAGs[i].Value);
                if (type == TAG.TAGType.PUREEND)
                {
                    //throw new Exception("结构错误！");
                    end = TAGs[i].Index + TAGs[i].Length - 1;
                    continue;
                }
                else if (type == TAG.TAGType.FULLTAG)
                {
                    current.head = name;
                    current.NextBlock = new TAGBlock();
                    current = current.NextBlock;
                    end = TAGs[i].Index + TAGs[i].Length - 1;
                }
                else
                {
                    current.head = name;
                    var InsideContent = CatchFirstTag(TAGs[i].Index, out end, htmlOrgCode, name);
                    if (InsideContent != null)
                    {
                        current.FirstInside = SplitTo(InsideContent);
                        current.NextBlock = new TAGBlock();
                        current = current.NextBlock;
                    }
                    else
                    {
                        current.NextBlock = new TAGBlock();
                        current = current.NextBlock;
                        end = TAGs[i].Index + TAGs[i].Length - 1;
                    }
                }
            }
            if(end<htmlOrgCode.Length-1)
            {
                TAGBlock.SetNULL
                    (current, htmlOrgCode.Substring(end + 1, htmlOrgCode.Length - end - 2));
            }
            return startNode;
        }
    }
    public class TAGBlock
    {
        public static void SetNULL(TAGBlock block,string content)
        {
            if (block == null)
                block = new TAGBlock();
            block.head = "NULL";
            block.content = content;
        }
        public TAGBlock() { }
        public string head { get; set; }
        public string content { get; set; }
        public TAGBlock NextBlock { get; set; }
        public TAGBlock FirstInside { get; set; }
    }
    public class TAG
    {
        public enum TAGType
        {
            PUREHEAD,
            PUREEND,
            FULLTAG
        }
        public static TAGType GetType(string InputTAG)
        {
            if (!InputTAG[0].Equals('<') ||
                !InputTAG[InputTAG.Length - 1].Equals('>'))
                throw new Exception("Not a TAG!");
            if (InputTAG[1].Equals('/'))
                return TAGType.PUREEND;
            else if (InputTAG[InputTAG.Length - 2].Equals('/'))
                return TAGType.FULLTAG;
            else return TAGType.PUREHEAD;
        }
        public static string GetName(string InputTAG)
        {
            return InputTAG.Split
                (new char[] { '<', '>', '/', ' ' }, 
                StringSplitOptions.RemoveEmptyEntries)[0];
        }
    }

}
