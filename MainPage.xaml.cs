using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;  /* Used to store a video stream to a file */
using System.Threading.Tasks;  /* Tasks */ 

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Memento
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        public static MainPage Current;
        MainPage rootPage = MainPage.Current;
        private Windows.Foundation.Collections.IPropertySet appSettings;
        private const String videoKey = "capturedVideo";

        public MainPage()
        {
            this.InitializeComponent();
            appSettings = ApplicationData.Current.LocalSettings.Values;
           
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Reload previously taken video
            if (appSettings.ContainsKey(videoKey))
            {
                object filePath;
                if (appSettings.TryGetValue(videoKey, out filePath) && filePath.ToString() != "")
                {
                    
                    await ReloadVideo(filePath.ToString());
                }
            }
        }

        private async void startRecordingBtn_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                //rootPage.NotifyUser("", NotifyType.StatusMessage);

                // Using Windows.Media.Capture.CameraCaptureUI API to capture a photo
                CameraCaptureUI dialog = new CameraCaptureUI();
                dialog.VideoSettings.Format = CameraCaptureUIVideoFormat.Mp4;

                StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Video);
                if (file != null)
                {
                    IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                    CapturedVideo.SetSource(fileStream, "video/mp4");

                    // Store the file path in Application Data
                    appSettings[videoKey] = file.Path;
                }
                else
                {
                    //rootPage.NotifyUser("No video captured.", NotifyType.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                //rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// This is the click handler for the 'CaptureButton' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CaptureVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //rootPage.NotifyUser("", NotifyType.StatusMessage);

                // Using Windows.Media.Capture.CameraCaptureUI API to capture a photo
                CameraCaptureUI dialog = new CameraCaptureUI();
                dialog.VideoSettings.Format = CameraCaptureUIVideoFormat.Mp4;

                StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Video);
                // I'm not able to record a video right now... How do I start recording? 
                if (file != null)
                {
                    IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                    CapturedVideo.SetSource(fileStream, "video/mp4");

                    // Store the file path in Application Data
                    appSettings[videoKey] = file.Path;
                }
                else
                {
                    //rootPage.NotifyUser("No video captured.", NotifyType.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                //rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
        }
        public void NotifyUser(string strMessage, NotifyType type)
        {
            switch (type)
            {
                // Use the status message style.
                case NotifyType.StatusMessage:
                    //StatusBlock.Style = Resources["StatusStyle"] as Style;
                    break;
                // Use the error message style.
                case NotifyType.ErrorMessage:
                    //StatusBlock.Style = Resources["ErrorStyle"] as Style;
                    break;
            }
            //StatusBlock.Text = strMessage;

            // Collapse the StatusBlock if it has no text to conserve real estate.
            //if (StatusBlock.Text != String.Empty)
            //{
            //    StatusBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //}
            //else
            //{
            //    StatusBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //} 
        }

        /// <summary>
        /// Loads the video from file path
        /// </summary>
        /// <param name="filePath">The path to load the video from</param>
        private async Task ReloadVideo(String filePath)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
                IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                CapturedVideo.SetSource(fileStream, "video/mp4");
                //rootPage.NotifyUser("", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                appSettings.Remove(videoKey);
                //rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
        }

    }

    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };
}
