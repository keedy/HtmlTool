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
using System.Windows.Shapes;

namespace HtmlTool
{
    /// <summary>
    /// Interaction logic for ViewTreeWin.xaml
    /// </summary>
    public partial class ViewTreeWin : Window
    {
        public ViewTreeWin(string link)
        {
            InitializeComponent();
            var content = new ContentHandle(link);
            if (WebHandle.getRootUrl(link) == null)
            {
                MessageBox.Show(link + "链接格式错误", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }
            if (content.WEBPAGE == null)
            {
                MessageBox.Show(link + "无法获取内容", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }
            TextBlock_title.Text = link;
            ShowHTMLTree(content.contentTree);
        }
        public void ShowHTMLTree(TAGBlock HtmlTag)
        {
            TreeView_WebPage.Items.Clear();
            ProcessHTMLTag(HtmlTag, null);
        }
        void ProcessHTMLTag(TAGBlock HtmlTag,
            TreeViewItem previewItem)
        {
            if (HtmlTag == null) return;
            TreeViewItem item = new TreeViewItem();
            item.Header = HtmlTag.Name;
            item.IsExpanded = true;
            if (previewItem == null)
            {
                TreeView_WebPage.Items.Add(item);
            }
            else
            {
                previewItem.Items.Add(item);
                if (HtmlTag.content != null)
                {
                    //string header = null;
                    //HtmlTag.content.ToList().ForEach(x => header += x+"\n");
                    item.Items.Add(new TreeViewItem() { Header = HtmlTag.content });
                }
            }
            if (HtmlTag.FirstInside != null)
            {
                ProcessHTMLTag(HtmlTag.FirstInside, item);
            }
            if (HtmlTag.NextBlock != null)
            {
                ProcessHTMLTag(HtmlTag.NextBlock, previewItem);
            }
        }

    }
}
