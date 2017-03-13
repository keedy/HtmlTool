using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlTool
{
    public class ContentHandle
    {
        public ContentHandle(string link)
        {
            this.LINK = link;
            this.WEBPAGE=WebHandle.GetHTMLPage(link, null);
            if (this.WEBPAGE != null)
            {
                this.contentTree = SplitHtml.SplitTo(this.WEBPAGE);
            }                
            this.RootUrl = WebHandle.getRootUrl(this.LINK);
        }
        string LINK;
        public string RootUrl
        {
            get;
            private set;
        }
        private string _WEBPAGE;
        public string WEBPAGE
        {
            get
            {
                return _WEBPAGE;
            }
            private set
            {
                _WEBPAGE = value;
            }
        }
        private TAGBlock _contentTree;
        public TAGBlock contentTree
        {
            get
            {
                return _contentTree;
            }
            private set
            {
                _contentTree = value;
            }
        }
    }
}
