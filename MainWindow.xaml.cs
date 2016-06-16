using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace HtmlTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SameLevel.IsThreeState = false;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        public void Init(Tag[] tags)
        {
            this.MyTags = tags;
            if (tags.Count() == 0)
            {
                MessageBox.Show("空数据！");
            }
            else
            {
                ShowHTMLTree(tags[0]);
                this.AddTagBox.MyTag = tags;
                //InitEditTree();
            }

        }
        Tag[] MyTags;
        string RootUrl = null;
        public void ShowHTMLTree(Tag HtmlTag)
        {
            HtmlTree.Items.Clear();
            ProcessHTMLTag(HtmlTag, null);
        }
        void ProcessHTMLTag(Tag HtmlTag,
            TreeViewItem previewItem)
        {
            if (HtmlTag == null) return;
            TreeViewItem item = new TreeViewItem();
            item.Header = HtmlTag.head;
            item.IsExpanded = true;
            if (previewItem == null)
            {
                HtmlTree.Items.Add(item);
            }
            else
            {
                previewItem.Items.Add(item);
                if (HtmlTag.contentL != null)
                    item.Items.Add(new TreeViewItem() { Header = HtmlTag.contentL });
                if (HtmlTag.contentR != null)
                    item.Items.Add(new TreeViewItem() { Header = HtmlTag.contentR });
                //if(HtmlTag.attris!=null)
                //{
                //    foreach(var attri in HtmlTag.attris)
                //    {
                //        previewItem.Items.Add(new TreeViewItem() { Header = attri.Key });
                //    }

                //}
            }
            if (HtmlTag.TagsInTag != null)
            {
                foreach (var tag in HtmlTag.TagsInTag)
                {
                    ProcessHTMLTag(tag, item);
                }
            }
        }
        void ProcessTagPattern(TagPattern HtmlTagPattern,
            TreeViewItem previewItem)
        {
            if (HtmlTagPattern == null) return;
            TreeViewItem item = new TreeViewItem();
            item.Header = HtmlTagPattern.PatternString;
            item.Tag = HtmlTagPattern;
            item.IsExpanded = true;
            if (previewItem == null)
            {
                editTree.Items.Add(item);
            }
            else
            {
                previewItem.Items.Add(item);
            }
        }
        void InitEditTree()
        {
            int count = 0;
            TreeViewItem preItem = null;
            while (count < 3)
            {
                TreeViewItem item = new TreeViewItem();
                TagTree t1 = new TagTree();
                t1.Text = "目录" + (count + 1).ToString();
                t1.Height = 20;
                t1.Width = 75;
                t1.MyTag = MyTags;
                item.Header = t1;
                item.IsExpanded = true;
                if (preItem == null)
                {
                    editTree.Items.Add(item);
                }
                else
                {
                    preItem.Items.Add(item);
                }
                preItem = item;
                count += 1;
            }

        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AddTagBox.Text.Length == 0)
            {
                if (HtmlTree.SelectedItem != null)
                {
                    List<string> borrow = new List<string>();
                    var item = HtmlTree.SelectedItem as TreeViewItem;
                    while (item != null)
                    {
                        borrow.Add(item.Header.ToString());
                        item = item.Parent == null ? null : item.Parent as TreeViewItem;
                    }
                    for (int i = borrow.Count - 2; i >= 0; i--)
                    {
                        AddTagBox.AppendText(borrow[i] + ".");
                    }
                    return;
                }
            }

            bool sameLevel = (bool)SameLevel.IsChecked;
            var tagPattern = AddTagBox.CurrentTags();
            if (tagPattern != null)
            {
                if (editTree.SelectedItem == null)
                {
                    ProcessTagPattern(tagPattern, null);
                    return;
                }
                if (sameLevel)
                {
                    var pItem = (editTree.SelectedItem as TreeViewItem).Parent;
                    ProcessTagPattern(tagPattern, pItem == null ? null : pItem as TreeViewItem);
                }
                else
                {
                    ProcessTagPattern(tagPattern, (editTree.SelectedItem as TreeViewItem));
                }
            }
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (editTree.SelectedItem == null) return;
            editTree.Items.Remove(editTree.SelectedItem);
        }

        private void deleteAllBtn_Click(object sender, RoutedEventArgs e)
        {
            editTree.Items.Clear();
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Path.Combine(filePath.Text, fileNameTB.Text);
            System.IO.File.WriteAllLines(path, GetEditTreeString(editTree.Items), Encoding.UTF8);
        }
        string[] GetEditTreeString(ItemCollection Items)
        {
            List<string> ret = new List<string>();
            if (Items.Count == 0) return ret.ToArray();
            List<TagPattern> patterns = new List<TagPattern>();
            foreach (var item in Items)
            {
                var p = (item as TreeViewItem).Tag as TagPattern;
                patterns.Add(p);
            }
            for (int i = 0; i < patterns[0].TAGS.Count; i++)
            {
                foreach (var p in patterns)
                {
                    if (p.contentPtr == "contentL")
                    {
                        foreach (var tag in p.TAGS[i])
                        {
                            if (p.IsIndex)
                            {
                                ret.Add("\r\n\r\n##　　" + tag.contentL + "\n");
                            }
                            else
                            {
                                ret.Add(tag.contentL + "\n");
                            }
                        }

                    }
                    else
                    {
                        if (p.contentPtr == "contentR")
                        {
                            foreach (var tag in p.TAGS[i])
                            {
                                ret.Add(tag.contentR + "\n");
                            }
                        }
                        else
                        {
                            foreach (var tag in p.TAGS[i])
                            {
                                if (!tag.attris.ContainsKey(p.contentPtr))
                                    continue;
                                ret.Add(tag.attris[p.contentPtr] + "\n");
                            }
                        }
                    }

                }
                //ret.AddRange(GetEditTreeString((item as TreeViewItem).Items));

            }
            return ret.ToArray();
        }
        public string[] GetEditTreeString()
        {
            return GetEditTreeString(editTree.Items);
        }
        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            Window win = new Window();
            ScrollViewer view = new ScrollViewer();

            TextBlock block = new TextBlock();
            foreach (var txt in GetEditTreeString(editTree.Items))
            {
                block.Text += txt;
            }
            view.Content = block;
            win.Content = view;
            win.Height = 250;
            win.Width = 300;
            win.Show();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var splitted = Regex.Split(links.Text.Trim(), @"\s");
            WebHandle handle = new WebHandle();
            List<Tag> tags = new List<Tag>();
            foreach (var link in splitted)
            {
                this.Cursor = Cursors.Wait;
                try
                {
                    int end = -1;
                    //var html = SplittedHtml.FindTags(handle.GetHTMLPage
                    //    (RootUrl==null?link:RootUrl+link), "html")[0];
                    //var page = SplittedHtml.MacthTag("html", 0, out end, html);
                    var page = SplitHtml.MakeFirstTag(0, out end, handle.GetHTMLPage
                        (RootUrl == null ? link : RootUrl + link), "html");
                    if (page != null) tags.Add(page);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);

                }
                finally
                {
                    this.Cursor = Cursors.Hand;
                }
            }
            Init(tags.ToArray());
        }



        private void RegexEdit_Click(object sender, RoutedEventArgs e)
        {
            RegexCtl ctl = new RegexCtl(links.Text);
            ctl.click += delegate()
            {
                this.links.Text = ctl.OutPut;
            };
            ctl.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ctl.ShowDialog();
        }

        private void editTree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (editTree.SelectedItem == null)
                return;
            var tagPattern = (editTree.SelectedItem as TreeViewItem).Tag as TagPattern;
            ContextMenu menu = new ContextMenu();
            if (tagPattern.IsIndex)
            {
                MenuItem item = new MenuItem();
                item.Header = "取消目录设置";
                item.Click += delegate(object sender1, RoutedEventArgs e1)
                {
                    tagPattern.IsIndex = false;
                    (editTree.SelectedItem as TreeViewItem).Foreground = Brushes.Black;
                };
                menu.Items.Add(item);
            }
            else
            {
                MenuItem item = new MenuItem();
                item.Header = "设置为目录";
                item.Click += delegate(object sender1, RoutedEventArgs e1)
                {
                    tagPattern.IsIndex = true;
                    (editTree.SelectedItem as TreeViewItem).Foreground = Brushes.Purple;
                };
                menu.Items.Add(item);
            }
            this.ContextMenu = menu;

        }
    }
}
