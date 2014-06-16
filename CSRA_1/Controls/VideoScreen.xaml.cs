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
using Microsoft.Win32;
using Camera_NET;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;

namespace CSRA_1.Controls
{
    /// <summary>
    /// Interaction logic for OptionsScreen.xaml
    /// </summary>
    public partial class VideoScreen : UserControl, INotifyPropertyChanged
    {
        private Camera_NET.CameraControl cameraControl;
        private Camera_NET.CameraChoice _CameraChoice;
        public Bitmap _SnapShotBMP;

        /*-------------------------------------------------------------------------------------------------------*/
        public VideoScreen()
        {
            InitializeComponent();
            cameraControl = new Camera_NET.CameraControl();
            _CameraChoice = new CameraChoice();
            InitializeDirectShow();
        }

        /*-------------------------------------------------------------------------------------------------------*/
        private void InitializeDirectShow()
        {
            // Register OnPaint callback for directshow.
            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
            cameraControl.Paint += new System.Windows.Forms.PaintEventHandler(pictureBox_Paint);
            host.Child = cameraControl;
            grid1.Children.Add(host);

            SetCurrentCamera();
        }

        /*-------------------------------------------------------------------------------------------------------*/
        private void SetCurrentCamera()
        {
            // Get List of devices (cameras)
            _CameraChoice.UpdateDeviceList();

            // To get an example of camera and resolution change look at other code samples 
            if (_CameraChoice.Devices.Count > 0)
            {
                // Device moniker. It's like device id or handle.
                // Run first camera if we have one
                var camera_moniker = _CameraChoice.Devices[0].Mon;

                // Set selected camera to camera control with default resolution
                cameraControl.SetCamera(camera_moniker, null);

                FillCameraList();
            }
        }

        /*-------------------------------------------------------------------------------------------------------*/
        private void pictureBox_Paint(object sender, System.Windows.Forms.PaintEventArgs args)
        {
        }

        /*-------------------------------------------------------------------------------------------------------*/
        private void Window_Closing(Object sender, RoutedEventArgs e)
        {
            cameraControl.CloseCamera();
        }

        /*-------------------------------------------------------------------------------------------------------*/
        private void buttonDone_Click(object sender, RoutedEventArgs e)
        {
            cameraControl.CloseCamera();
            OnPropertyChanged("DoneButtonPress");
        }

        /*-------------------------------------------------------------------------------------------------------*/
        private void FillCameraList()
        {
            comboBoxCameraList.Items.Clear();
            _CameraChoice.UpdateDeviceList();

            foreach (var camera_device in _CameraChoice.Devices)
            {
                string str = camera_device.Name.ToString();
                comboBoxCameraList.Items.Add(str);
            }
            comboBoxCameraList.SelectedIndex = 0;
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

        /*-------------------------------------------------------------------------------------------------------*/
        private void comboBoxCameraList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxCameraList.SelectedIndex < 0)
            {
                cameraControl.CloseCamera();
            }
            else
            {
                // Set camera
                SetCamera(_CameraChoice.Devices[comboBoxCameraList.SelectedIndex].Mon, null);
            }
        }

        /*-------------------------------------------------------------------------------------------------------*/
        private void SetCamera(IMoniker camera_moniker, Resolution resolution)
        {
            try
            {
                cameraControl.SetCamera(camera_moniker, resolution);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, @"Error while running camera");
            }

            if (!cameraControl.CameraCreated)
                return;

            cameraControl.MixerEnabled = true;
        }

        /*-------------------------------------------------------------------------------------------------------*/
        private void BtnSnapShot_Click(object sender, RoutedEventArgs e)
        {
            if (BtnSnapShot.Content.ToString() == "Snap Shot")
                SnapAndDisplay();
            else
                HideStillAndStream();
        }

        /*-------------------------------------------------------------------------------------------------------*/
        private void SnapAndDisplay()
        {
            if (!cameraControl.CameraCreated)
                return;

            try
            {
                _SnapShotBMP = cameraControl.SnapshotSourceImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error while getting a snapshot");
            }

            cameraControl.CloseCamera();
            BtnSnapShot.Content = "Stream";

            if (_SnapShotBMP == null)
                return;

            pictureBoxScreenshot.Source = CreateBitmapSourceFromBitmap(_SnapShotBMP);
            pictureBoxScreenshot.Visibility = Visibility.Visible;
        }

        /*-------------------------------------------------------------------------------------------------------*/
        private void HideStillAndStream()
        {
            SetCurrentCamera();
            BtnSnapShot.Content = "Snap Shot";
            pictureBoxScreenshot.Visibility = Visibility.Hidden;
        }

        /*-------------------------------------------------------------------------------------------------------*/
        public static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        /*-------------------------------------------------------------------------------------------------------*/
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "Bitmap Image|*.bmp" };
            if (dialog.ShowDialog() == true)
            {
                BitmapUtility.SaveImage(_SnapShotBMP, dialog.FileName);
            }
        }
    }
}
