using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using System.IO;
using System.ComponentModel;
using System.Windows.Input;

namespace SaveHtml
{

    public class ViewModel : INotifyPropertyChanged
    {
        // ViewModel
        public ViewModel()
        {
            bool loaded = LoadInitialData();
            if (!loaded)
            {
                this.UrlData_ = new ObservableCollection<UrlItem>();
                this.UrlData_.Add(new UrlItem { Name_ = "name1", Path_ = "path1" });
                this.UrlData_.Add(new UrlItem { Name_ = "name2", Path_ = "path2" });
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // Command
        private ICommand httpRequestCommand_ = null;
        public ICommand HttpRequestCommand_
        {
            get
            {
                if (this.httpRequestCommand_ == null)
                {
                    this.httpRequestCommand_ = new HttpRequestCommand(this);
                }
                return this.httpRequestCommand_;
            }
        }

        // SaveTo
        private string saveTo_ = "./Log.txt";
        public string SaveTo_
        {
            get { return this.saveTo_; }
            set
            {
                this.saveTo_ = value;
                NotifyPropertyChanged("SaveTo_");
            }
        }
        public bool OverwriteExistingFile_ { get; set; } 

        // Logs
        public string Logs_ { get; private set; }
        public void addLog(string logtext)
        {
            string logline = "[" + DateTime.Now + "]" + logtext;
            this.Logs_ += (logline + "\n");

            NotifyPropertyChanged("Logs_");
        }

        // URL
        public ObservableCollection<UrlItem> UrlData_ { get; private set; }
        private bool LoadInitialData()
        {
            try
            {
                StreamReader sr = new StreamReader("./Url.txt");
                this.UrlData_ = new ObservableCollection<UrlItem>();
                while (sr.EndOfStream == false)
                {
                    string line1 = sr.ReadLine();
                    string line2 = sr.ReadLine();
                    string line3 = sr.ReadLine();
                    string line4 = sr.ReadLine();
                    this.UrlData_.Add(new UrlItem { Name_ = line1, Path_ = line2, Username_ = line3, Password_ = line4 });
                }
            }
            catch (FileNotFoundException ex)
            {
                return false;
            }
            return true;
        }

        // Progress
        private System.Windows.Visibility progressVisibility_ = System.Windows.Visibility.Hidden;
        public System.Windows.Visibility ProgressVisibility_
        {
            get { return this.progressVisibility_; }
            set
            {
                this.progressVisibility_ = value;
                NotifyPropertyChanged("ProgressVisibility_");
            }
        }
    }

    public class UrlItem
    {
        public string Name_ { get; set; }
        public string Path_ { get; set; }
        public string Username_ { get; set; }
        public string Password_ { get; set; }
    }
}
