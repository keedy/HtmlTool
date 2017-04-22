using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlTool
{
    public enum TagAttached
    {
        提取,
        排除,
        标题,
    }
    public static class DataResource
    {
        public static Dictionary<string,string> Marks=new Dictionary<string,string>(
        ) { { "h1", "#" }, { "h2", "##" }, { "h3", "###" }, { "h4", "####" }, { "h5", "#####" }, { "h6", "######" } };
        public static string GetMarkFormatted(string MarkKey)
        {
            if(Marks.ContainsKey(MarkKey))
            {
                return string.Format("\n\n{0}\t\t", Marks[MarkKey]);
            }
            return string.Empty;
        }
    }
    public class ContentSetting:System.ComponentModel.INotifyPropertyChanged
    {
        private TagAttached _ContentTagAttached;
        public TagAttached ContentTagAttached
        {
            get { return _ContentTagAttached; }
            set { _ContentTagAttached = value;
            OnPropertyChanged("ContentTagAttached");
            }
        }
        private string _TagName;
        public string TagName
        {
            get { return _TagName; }
            set
            {
                _TagName = value;
            OnPropertyChanged("TagName");
            }
        }
        private string _MarkKey;
        public string MarkKey
        {
            get { return _MarkKey; }
            set { _MarkKey = value;
            OnPropertyChanged("MarkKey");
            }
        }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            if(this.PropertyChanged!=null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(property));
            }
        }
    }
}
