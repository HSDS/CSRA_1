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
    /// Interaction logic for AcquireScreen.xaml
    /// </summary>
    public partial class AcquireScreen : UserControl, INotifyPropertyChanged
    {
        public ImageData ImageData = new ImageData();

        public AcquireScreen()
        {
            InitializeComponent();
        }

        private void buttonLoadImage_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog() )
            {
                dlg.Filter = "Image files (*.bmp,*.jpg)|*.bmp;*.jpg";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ImageData.Load(dlg.FileName);
                }
            }
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("OKButtonPress");
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("CancelButtonPress");
        }

        private void buttonLiveVideo_Click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("LiveVideoPress");
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
