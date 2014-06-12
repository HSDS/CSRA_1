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
    /// Interaction logic for CalibrationScreen.xaml
    /// </summary>
    public partial class CalibrationScreen : UserControl, INotifyPropertyChanged
    {
        public CalibrationScreen()
        {
            InitializeComponent();
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
