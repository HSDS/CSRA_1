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
    /// Interaction logic for EnrollScreen.xaml
    /// </summary>
    public partial class EnrollScreen : UserControl, INotifyPropertyChanged
    {
        public UserData userData = new UserData();

        public string UserName
        {
            get { return userData.Name; }
            set
            {
                userData.Name = value;
                OnPropertyChanged("UserName");
            }
        }

        public string UserPIN
        {
            get { return userData.PIN; }
            set
            {
                userData.PIN = value;
                OnPropertyChanged("UserPIN");
            }
        }

        public EnrollScreen()
        {
            InitializeComponent();

            textBoxName.DataContext = this;
            textBoxPIN.DataContext = this;
        }

        private void buttonAcquire_Click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("AcquireButtonPress");
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("OKButtonPress");
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("CancelButtonPress");
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
