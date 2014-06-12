using CSRA_1.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace CSRA_1.Controls
{
    /// <summary>
    /// Interaction logic for VerifyScreen.xaml
    /// </summary>
    public partial class VerifyScreen : UserControl, INotifyPropertyChanged
    {
        UserData userData = new UserData();
        public UserData UserData
        {
            get { return userData; }
            set
            {
                userData = value;
                UserText = userData.ToString();
            }
        }

        string userText = "";

        public string UserText
        {
            get { return userData.ToString(); }
            set
            {
                userText = value;
                OnPropertyChanged("UserText");
            }
        }
        public VerifyScreen()
        {
            InitializeComponent();

            textBoxUserText.DataContext = this;
        }

        private void buttonAcquire_Click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("AcquireButtonPress");
        }

        private void buttonDone_Click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("DoneButtonPress");
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

    }
}
