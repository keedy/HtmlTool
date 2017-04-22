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
        }

        private void GetIndexesLink_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            var matchLinks=Regex.Matches(this.TEXTBOX_Links.Text, @"\s*.+(\s+|$)");
            foreach(Match link in matchLinks)
            {
                if(link.Success)
                {
                    var content = new ContentHandle(link.Value.Trim());
                    if (WebHandle.getRootUrl(link.Value.Trim()) == null)
                    {
                        if (MessageBox.Show(link.Value.Trim() + "链接格式错误", "是否继续", MessageBoxButton.YesNo, MessageBoxImage.Question)
                            == MessageBoxResult.Yes)
                            continue;
                        else
                            return;
                    }
                    if(content.WEBPAGE==null)
                    {
                        if (MessageBox.Show(link.Value.Trim()+"无法获取内容", "是否继续", MessageBoxButton.YesNo, MessageBoxImage.Question)
                            == MessageBoxResult.Yes)
                            continue;
                        else
                            return;
                    }
                    var links = SplitHtml.GetLinks(content.contentTree);
                    //TEXTBOX_Links.Clear();
                    foreach(var l in links)
                    {
                        TEXTBOX_View.AppendText
                            ((string.IsNullOrEmpty(WebHandle.getRootUrl(l.Key)) ?
                            content.RootUrl+l.Key : l.Key )+ "\n");
                    }
                }
            }
            this.Cursor = Cursors.Arrow;
        }

        private void GetTXT_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            TEXTBOX_View.Clear();
            var matchLinks = Regex.Matches(this.TEXTBOX_Links.Text, @"\s*.+(\s+|$)");
            foreach (Match link in matchLinks)
            {
                if (link.Success)
                {
                    var content = new ContentHandle(link.Value.Trim());
                    if (WebHandle.getRootUrl(link.Value.Trim()) == null)
                    {
                        if (MessageBox.Show(link.Value.Trim() + "链接格式错误", "是否继续", MessageBoxButton.YesNo, MessageBoxImage.Question)
                            == MessageBoxResult.Yes)
                            continue;
                        else
                            return;
                    }
                    if (content.WEBPAGE == null)
                    {
                        if (MessageBox.Show(link.Value.Trim() + "无法获取内容", "是否继续", MessageBoxButton.YesNo, MessageBoxImage.Question)
                            == MessageBoxResult.Yes)
                            continue;
                        else
                            return;
                    }
                    SplitHtml.RefreshUserSettings();
                    string append=SplitHtml.FindTXTContent(content.contentTree);
                    this.Dispatcher.BeginInvoke(new Action<string>(delegate(string Toappend)
                        {
                            TEXTBOX_View.AppendText(Toappend);
                        }),append);
                    //TEXTBOX_View.AppendText(SplitHtml.FindTXTContent(content.contentTree));
                }
            }
            this.Cursor = Cursors.Arrow;
        }


        private void ViewTree_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            var matchLink = Regex.Match(this.TEXTBOX_Links.Text, @"\s*http(s?):\/\/.+(\s+|$)");
            if (matchLink.Success)
            {
                ViewTreeWin win = new ViewTreeWin(matchLink.Value.Trim());
                win.Owner = this;
                win.Icon = win.Owner.Icon;
                win.Show();
            }
            this.Cursor = Cursors.Arrow;
        }

        private void Save2File_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "Text File(.txt)|*.txt";
            dialog.DefaultExt = "txt";
            dialog.RestoreDirectory = true;
            if(dialog.ShowDialog(this)==true)
            {
                using (var stream = dialog.OpenFile())
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(stream))
                        sw.Write(TEXTBOX_View.Text);
                }
                MessageBox.Show("保存成功" + dialog.FileName);
            }
        }

        private void SetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingWin win = new SettingWin();
            win.ShowDialog();
        }


        //private void RegexEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    RegexCtl ctl = new RegexCtl(links);
        //    ctl.click += delegate()
        //    {
        //        this.links.Text = ctl.OutPut;
        //    };
        //    ctl.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        //    ctl.ShowDialog();
        //}

    }
}
