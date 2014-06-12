using CSRA_1.Controls;
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

namespace CSRA_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CSRAData csraData = new CSRAData();
        UserData tempEnrollUserData = new UserData();

        public MainWindow()
        {
            InitializeComponent();

            MyInit();
        }

        void MyInit()
        {
            CSRAData tempData = CSRAData.Load();
            if (tempData != null)
            {
                csraData = tempData;
            }

            border.Child = new MainScreen();
            (border.Child as MainScreen).PropertyChanged += mainScreen_PropertyChanged;
            (border.Child as MainScreen).SetVerifyEnable(csraData.HasEnrollmentData());
        }

        void mainScreen_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "EnrollButtonPress":
                    LogFile.Write("MainScreen: Enroll button press");
                    border.Child = new EnrollScreen();
                    (border.Child as EnrollScreen).PropertyChanged += enrollScreen_PropertyChanged;
                    (border.Child as EnrollScreen).userData = new UserData(csraData.UserData);
                    break;

                case "VerifyButtonPress":
                    LogFile.Write("MainScreen: Verify button press");
                    border.Child = new VerifyScreen();
                    (border.Child as VerifyScreen).PropertyChanged += verifyScreen_PropertyChanged;
                    (border.Child as VerifyScreen).UserData = csraData.UserData;
                    break;

                case "OptionsButtonPress":
                    LogFile.Write("MainScreen: Options button press");
                    border.Child = new OptionsScreen();
                    (border.Child as OptionsScreen).PropertyChanged += optionsScreen_PropertyChanged;
                    (border.Child as OptionsScreen).options = new Options(csraData.Options);
                    break;

                case "CalibrateButtonPress":
                    LogFile.Write("MainScreen: Calibrate button press");
                    border.Child = new CalibrationScreen();
                    (border.Child as CalibrationScreen).PropertyChanged += calibrationScreen_PropertyChanged;
                    break;

                case "ExitButtonPress":
                    LogFile.Write("MainScreen: Exit button press");
                    Close();
                    break;

                default:
                    break;
            }
        }


        void enrollScreen_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "AcquireButtonPress":
                    tempEnrollUserData = (border.Child as EnrollScreen).userData;
                    border.Child = new AcquireScreen();
                    (border.Child as AcquireScreen).PropertyChanged += enrollAcquireScreen_PropertyChanged;
                    break;

                case "OKButtonPress":
                    MessageBox.Show("OK");
                    csraData.UserData = (border.Child as EnrollScreen).userData;
                    border.Child = new MainScreen();
                    (border.Child as MainScreen).PropertyChanged += mainScreen_PropertyChanged;
                    (border.Child as MainScreen).SetVerifyEnable(csraData.HasEnrollmentData());

                    // process here
                    break;

                case "CancelButtonPress":
                    MessageBox.Show("Cancel");
                    border.Child = new MainScreen();
                    (border.Child as MainScreen).PropertyChanged += mainScreen_PropertyChanged;
                    (border.Child as MainScreen).SetVerifyEnable(csraData.HasEnrollmentData());

                    break;

                default:
                    break;
            }
        }

        void enrollAcquireScreen_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "OKButtonPress":
                    MessageBox.Show("OK");
                    border.Child = new EnrollScreen();
                    (border.Child as EnrollScreen).PropertyChanged += enrollScreen_PropertyChanged;
                    (border.Child as EnrollScreen).userData = tempEnrollUserData;
                    // save image
                    break;

                case "CancelButtonPress":
                    MessageBox.Show("Cancel");
                    border.Child = new EnrollScreen();
                    (border.Child as EnrollScreen).PropertyChanged += enrollScreen_PropertyChanged;
                    (border.Child as EnrollScreen).userData = tempEnrollUserData;
                    break;

                case "LiveVideoPress":
                    border.Child = new VideoScreen();
                    (border.Child as VideoScreen).PropertyChanged += enrollLiveVideoScreen_PropertyChanged;
                   // (border.Child as VideoScreen).userData = tempEnrollUserData;
                    break;
                   
                default:
                    break;
            }
        }

        void enrollLiveVideoScreen_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "OKButtonPress":
                  //  MessageBox.Show("OK");
                    break;

                case "CancelButtonPress":
                 //   MessageBox.Show("Cancel");
                    break;

                default:
                    break;
            }

            border.Child = new AcquireScreen();
            (border.Child as AcquireScreen).PropertyChanged += enrollAcquireScreen_PropertyChanged;
        }

        void verifyScreen_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "AcquireButtonPress":
                    border.Child = new AcquireScreen();
                    (border.Child as AcquireScreen).PropertyChanged += verifyAcquireScreen_PropertyChanged;
                    break;

                case "DoneButtonPress":
                    MessageBox.Show("Done");
                    border.Child = new MainScreen();
                    (border.Child as MainScreen).PropertyChanged += mainScreen_PropertyChanged;
                    (border.Child as MainScreen).SetVerifyEnable(csraData.HasEnrollmentData());

                    break;

                default:
                    break;
            }
        }

        void verifyAcquireScreen_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "OKButtonPress":
                    MessageBox.Show("OK");
                    border.Child = new VerifyScreen();
                    (border.Child as VerifyScreen).PropertyChanged += verifyScreen_PropertyChanged;
                    (border.Child as VerifyScreen).UserData = csraData.UserData;
                    // save image
                    break;

                case "CancelButtonPress":
                    MessageBox.Show("Cancel");
                    border.Child = new VerifyScreen();
                    (border.Child as VerifyScreen).PropertyChanged += verifyScreen_PropertyChanged;
                    (border.Child as VerifyScreen).UserData = csraData.UserData;
                    break;

                default:
                    break;
            }
        }

        void optionsScreen_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "OKButtonPress":
                    csraData.Options = (border.Child as OptionsScreen).options;
                    border.Child = new MainScreen();
                    (border.Child as MainScreen).PropertyChanged += mainScreen_PropertyChanged;
                    (border.Child as MainScreen).SetVerifyEnable(csraData.HasEnrollmentData());

                    break;

                case "CancelButtonPress":
                    border.Child = new MainScreen();
                    (border.Child as MainScreen).PropertyChanged += mainScreen_PropertyChanged;
                    (border.Child as MainScreen).SetVerifyEnable(csraData.HasEnrollmentData());

                    break;

                default:
                    break;
            }
        }

        void calibrationScreen_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "OKButtonPress":
                    MessageBox.Show("OK");
                    border.Child = new MainScreen();
                    (border.Child as MainScreen).PropertyChanged += mainScreen_PropertyChanged;
                    (border.Child as MainScreen).SetVerifyEnable(csraData.HasEnrollmentData());

                    break;

                case "CancelButtonPress":
                    MessageBox.Show("Cancel");
                    border.Child = new MainScreen();
                    (border.Child as MainScreen).PropertyChanged += mainScreen_PropertyChanged;
                    (border.Child as MainScreen).SetVerifyEnable(csraData.HasEnrollmentData());

                    break;

                default:
                    break;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            LogFile.Write("MainScreen: Window closing");
            LogFile.Close();
            CSRAData.Save(csraData);
        }
    }
}
