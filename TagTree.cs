using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows;

namespace HtmlTool
{
    class TagTree:TextBox
    {
        public TAGBlock[] MyTag;
        const string tagPattern = @"(?<tag>^(?<front>([^<>]+\.){0,})(?<behind>[^<>]+)$)";
        protected override void OnPreviewMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.MyTag == null)
            {
                base.OnPreviewMouseRightButtonDown(e);
                return;
            }
            this.ContextMenu = GetObjectMenu( MyTag );
        }
        protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (this.MyTag == null) return;
            if (e.Key == Key.OemPeriod)
            {
                var PartTree = new Regex(@"(?<partTree>([^<>]+\.)+$)");
                string TagPtr = PartTree.Match(this.Text.Substring(0, this.SelectionStart)).Value;
                if (TagPtr.Length > 0)
                {
                    this.SelectionLength = Regex.Match(Text.Substring(SelectionStart), @"^(?<partTree>([^<>]+\.)+)").Length;
                    this.ContextMenu = GetObjectMenu(GetTagObject
                        (this.MyTag , TagPtr.Split('.').Where(x => x.Length > 0).ToArray()));
                    if(this.ContextMenu.HasItems)
                    {
                        this.ContextMenu.IsOpen = true;
                    }
                }

            }
        }
        ContextMenu GetObjectMenu(TAGBlock[] objs)
        {
            ContextMenu menu = new ContextMenu();
            List<string> menuString = new List<string>();
            foreach(var tag in objs)
            {
                //if(tag.contentL!=null&&tag.contentL.Length>0)
                //{
                //    menuString.Add("contentL");
                //}
                //if(tag.contentR!=null&&tag.contentR.Length>0)
                //{
                //    menuString.Add("contentR");
                //}
                if (tag.content != null)
                    menuString.Add("content");
            }
            menuString=menuString.Distinct().ToList();
            foreach(var s in menuString)
            {
                MenuItem item = new MenuItem();
                item.Header = s;
                item.Click += delegate(object o, RoutedEventArgs e)
                {
                    string txt = item.Header.ToString();
                    SelectedText = txt;
                    SelectionStart += txt.Length;
                    SelectionLength = 0;
                };
                menu.Items.Add(item);
            }
            return menu;
        }
        TAGBlock[] GetTagObject(TAGBlock[] nodes, string Ptr)
        {
            List<TAGBlock> TagsIn = new List<TAGBlock>();
            List<TAGBlock> TagsOut = new List<TAGBlock>();
            TagsIn = nodes.ToList();
            foreach(var tag in TagsIn)
            {
            }
            return TagsOut.ToArray();
        }
        TAGBlock[] GetTagObject(TAGBlock[] nodes, string[] ptrs)
        {
            TAGBlock[] temp=nodes;
            foreach(var ptr in ptrs)
            {
                temp = GetTagObject(temp, ptr);
                if (temp.Count() == 0) break;
            }
            return temp;
        }
        public TagPattern CurrentTags()
        {
            var TagMatch = Regex.Match(this.Text, tagPattern);
            if (!TagMatch.Success) return null;
            TagPattern pattern = new TagPattern();
            foreach (var tag in MyTag)
            {
                pattern.TAGS.Add(GetTagObject
                    (new TAGBlock[] { tag },
                    TagMatch.Groups["front"].Value.Split('.').Where(x => x.Length > 0).ToArray()));
            }
            if (pattern.TAGS == null)
            {
                return null;
            }
            if (pattern.TAGS.Count() == 0)
            {
                return null;
            }
            pattern.contentPtr = TagMatch.Groups["behind"].Value;
            pattern.PatternString = this.Text;
            return pattern;
        }
    }
    class TagPattern
    {
        public TagPattern()
        {
            TAGS = new List<TAGBlock[]>();
        }
        public List<TAGBlock[]> TAGS;
        public string contentPtr;
        public string PatternString;
        public bool IsIndex { get; set; }

    }
}
