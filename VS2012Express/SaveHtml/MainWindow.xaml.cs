using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;

namespace SaveHtml
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel vm_;
        public MainWindow()
        {
            InitializeComponent();

            this.vm_ = new ViewModel();
            this.DataContext =this.vm_;
        }

        private void TextBox_TextChanged_Log(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            textbox.ScrollToEnd();
        }

        // Drag Dropは手軽に処理するためにここに記述
        private void TextBox_PreviewDragOver_SaveTo(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
        }
        private void TextBox_Drop_SaveTo(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null) { return; }
            foreach (string s in files)
            {
                this.vm_.SaveTo_ = s;
                break; // 先頭のみ処理
            }
        }

    }
}
