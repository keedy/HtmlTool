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
        public void Init(TAGBlock[] tags)
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
            }

        }
        TAGBlock[] MyTags;
        string RootUrl = null;
        public void ShowHTMLTree(TAGBlock HtmlTag)
        {
            HtmlTree.Items.Clear();
            ProcessHTMLTag(HtmlTag, null);
        }
        void ProcessHTMLTag(TAGBlock HtmlTag,
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
                if (HtmlTag.content != null)
                {
                    string header = null;
                    HtmlTag.content.ToList().ForEach(x => header += x+"\n");
                    item.Items.Add(new TreeViewItem() { Header = header });
                }
            }
            if (HtmlTag.FirstInside != null)
            {
                ProcessHTMLTag(HtmlTag.FirstInside, item);
            }
            if(HtmlTag.NextBlock!=null)
            {
                ProcessHTMLTag(HtmlTag.NextBlock, previewItem);
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
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
            throw new Exception("未完待续");
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
            List<TAGBlock> tags = new List<TAGBlock>();
            foreach (var link in splitted)
            {
                this.Cursor = Cursors.Wait;
                try
                {
                    var page =  SplitHtml.SplitTo(handle.GetHTMLPage
                        (RootUrl == null ? link : RootUrl + link));
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
            //if (editTree.SelectedItem == null)
            //    return;
            //var tagPattern = (editTree.SelectedItem as TreeViewItem).Tag as TagPattern;
            //ContextMenu menu = new ContextMenu();
            //if (tagPattern.IsIndex)
            //{
            //    MenuItem item = new MenuItem();
            //    item.Header = "取消目录设置";
            //    item.Click += delegate(object sender1, RoutedEventArgs e1)
            //    {
            //        tagPattern.IsIndex = false;
            //        (editTree.SelectedItem as TreeViewItem).Foreground = Brushes.Black;
            //    };
            //    menu.Items.Add(item);
            //}
            //else
            //{
            //    MenuItem item = new MenuItem();
            //    item.Header = "设置为目录";
            //    item.Click += delegate(object sender1, RoutedEventArgs e1)
            //    {
            //        tagPattern.IsIndex = true;
            //        (editTree.SelectedItem as TreeViewItem).Foreground = Brushes.Purple;
            //    };
            //    menu.Items.Add(item);
            //}
            //this.ContextMenu = menu;

        }
    }
}
