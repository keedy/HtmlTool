using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for RegexCtl.xaml
    /// </summary>
    public partial class RegexCtl : Window
    {
        public RegexCtl(string input)
        {
            InitializeComponent();
            this.Input = input;
        }
        public event Action click;
        string Input;
        public string OutPut;
        private void match_Click(object sender, RoutedEventArgs e)
        {
            foreach(Match mt in Regex.Matches(Input, RegexTextBox.Text))
            {
                OutPut += mt.Value+"\n";
            }
            if(click!=null)
            {
                click();
            }
        }

        private void replace_Click(object sender, RoutedEventArgs e)
        {
            var replaceStr = RegexTextBox.Text.Split(',');
            OutPut = Regex.Replace(Input,replaceStr[0],new MatchEvaluator(delegate(Match match)
            {
                return replaceStr.Count() > 1 ? replaceStr[1] : "";
            }));
            if (click != null)
            {
                click();
            }
        }
    }
}
