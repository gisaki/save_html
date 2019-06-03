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

            vm_ = new ViewModel();
            this.DataContext = vm_;
        }

        async private void Button_Click_Get(object sender, RoutedEventArgs e)
        {
            UrlItem url = ComboBoxUrl.SelectedItem as UrlItem;
            string filename = TextBoxSaveTo.Text;

            await Task.Run(() => HttpGetProc(url, filename));
        }


        private void HttpGetProc(UrlItem url, string filename)
        {
            if (url == null)
            {
                vm_.addLog("error, No URL selected.");
                return;
            }

            vm_.addLog("request, " + url.Name_ + ", " + url.Path_);
            // MessageBox.Show(url.Name_ + "\n" + url.Path_);

            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url.Path_);
            req.Credentials = new System.Net.NetworkCredential(url.Username_, url.Password_);

            System.Net.HttpWebResponse res = null;
            try
            {
                res = (System.Net.HttpWebResponse)req.GetResponse();
                System.IO.Stream st = res.GetResponseStream();
                System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                byte[] buf = new byte[1024];
                while (true)
                {
                    int readsize = st.Read(buf, 0, buf.Length);
                    if (readsize == 0)
                    {
                        break;
                    }
                    fs.Write(buf, 0, readsize);
                }
                fs.Close();
                st.Close();
                vm_.addLog("done");
            }
            catch (System.Net.WebException ex)
            {
                if (ex.Status == System.Net.WebExceptionStatus.ProtocolError)
                {
                    System.Net.HttpWebResponse errres = (System.Net.HttpWebResponse)ex.Response;
                    //MessageBox.Show(errres.ResponseUri + "\n" + errres.StatusCode + "\n" + errres.StatusDescription);
                    vm_.addLog("error, " + errres.ResponseUri);
                    vm_.addLog("error, " + errres.StatusCode);
                    vm_.addLog("error, " + errres.StatusDescription);
                }
                else
                {
                    //MessageBox.Show(ex.Message);
                    vm_.addLog("error, " + ex.Message);
                }
            }
            finally
            {
                if (res != null)
                {
                    res.Close();
                }
            }
        }

        private void TextBox_TextChanged_Log(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            textbox.ScrollToEnd();
        }
    }
}
