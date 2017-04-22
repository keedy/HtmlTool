using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for SettingWin.xaml
    /// </summary>
    public partial class SettingWin : Window
    {
        public SettingWin()
        {
            InitializeComponent();
            //AllTagAttached=Enum.GetValues(typeof(TagAttached)).Cast<TagAttached>().ToList();
            CustomAppSettings.Default.Reload();
            this.DataGrid_ContentSetting.ItemsSource = CustomAppSettings.Default.contentSettings;
            
        }
        //List<TagAttached> AllTagAttached;

        private void Btn_SaveContentSettings_Click(object sender, RoutedEventArgs e)
        {
            CustomAppSettings.Default.contentSettings = new BindingList<ContentSetting>(CustomAppSettings.Default.contentSettings);
            CustomAppSettings.Default.Save();
            MessageBox.Show("保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
