using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

namespace SaveHtml
{
    public class HttpRequestCommand : ICommand
    {
        private ViewModel vm_;

        // 処理中？
        private bool isBusy_ = false;
        public bool IsBusy
        {
            get { return isBusy_; }
            set
            {
                isBusy_ = value;
                RaiseCanExecuteChanged();
            }
        }

        // コンストラクタ
        public HttpRequestCommand(ViewModel vm)
        {
            this.vm_ = vm;
        }

        public event EventHandler CanExecuteChanged
        {
            // これでInvalidateRequerySuggested()はすべてのコマンドに対して作用するようになる
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        private void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
        public bool CanExecute(object parameter)
        {
            return !this.IsBusy;
        }

        async public void Execute(object parameter)
        {
            UrlItem url = parameter as UrlItem;
            string filename = this.vm_.SaveTo_;

            // 上書きチェック
            if ((System.IO.File.Exists(filename)) && (!this.vm_.OverwriteExistingFile_))
            {
                System.Windows.MessageBoxResult result =
                    System.Windows.MessageBox.Show(
                        "There is already a file with the same name.\nOverwrite local file ?\n\n" + filename,
                        "Save File",
                        System.Windows.MessageBoxButton.YesNoCancel,
                        System.Windows.MessageBoxImage.Question
                    );
                if (result != System.Windows.MessageBoxResult.Yes)
                {
                    return;
                }
            }

            //System.Windows.MessageBox.Show("HttpRequestCommand, Execute, \n" + url + "\n" + filename);

            this.IsBusy = true;
            vm_.ProgressVisibility_ = System.Windows.Visibility.Visible;
            await Task.Run(() => HttpGetProc(url, filename));
            vm_.ProgressVisibility_ = System.Windows.Visibility.Hidden;
            this.IsBusy = false;
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
            req.Credentials = new System.Net.NetworkCredential(url.Username_, url.Password_); // あってもなくても

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
            catch (System.IO.IOException ex)
            {
                vm_.addLog("error, " + ex.Message);
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
            catch (Exception ex)
            {
                vm_.addLog("error, " + ex.Message);
            }
            finally
            {
                if (res != null)
                {
                    res.Close();
                }
            }
        }
    }
}
