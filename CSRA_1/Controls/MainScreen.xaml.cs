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
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : UserControl, INotifyPropertyChanged
    {
        public MainScreen()
        {
            InitializeComponent();
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

        public void SetVerifyEnable(bool enabled)
        {
            buttonVerify.IsEnabled = enabled;
        }

        private void buttonEnroll_Click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("EnrollButtonPress");
        }

        private void buttonVerify_Click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("VerifyButtonPress");
        }

        private void buttonOptions_Click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("OptionsButtonPress");
        }

        private void buttonCalibrate_Click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("CalibrateButtonPress");
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("ExitButtonPress");
        }
    }
}
