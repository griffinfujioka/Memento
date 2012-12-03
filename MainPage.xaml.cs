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
using System.Net.Http;          /* For http handlers */
using System.Net.Http.Headers;  /* For ProductInfoHeaderValue class */ 
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

        HttpClient httpClient;

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

            // HttpClient functionality can be extended by plugging multiple handlers together and providing
            // HttpClient with the configured handler pipeline.
            HttpMessageHandler handler = new HttpClientHandler();
            handler = new PlugInHandler(handler); // Adds a custom header to every request and response message.            
            httpClient = new HttpClient(handler);


            // The following line sets a "User-Agent" request header as a default header on the HttpClient instance.
            // Default headers will be sent with every request sent from this HttpClient instance.
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Sample", "v8"));

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

        private async void sendBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MultipartFormDataContent form = new MultipartFormDataContent();
                string filepath = "C:\\Users\\Griffin\\AppData\\Local\\Packages\\1957dd34-ee02-42da-a878-e11efa152641_f7f6khtztvxxm\\TempState\\video004.mp4";
                StorageFile file = await StorageFile.GetFileFromPathAsync(filepath);
                var stream = await file.OpenReadAsync();
                //IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                StreamContent streamContent = new StreamContent(stream.AsStream(), 1024); 
                form.Add(streamContent, "video", file.Path);
                string address = "http://momento.wadec.com/upload";
                HttpResponseMessage response = await httpClient.PostAsync(address, form);

                //await Helpers.DisplayTextResult(response, OutputField);

                //rootPage.NotifyUser("Completed", NotifyType.StatusMessage);
            }
            catch (HttpRequestException hre)
            {
                //rootPage.NotifyUser("Error", NotifyType.ErrorMessage);
                //OutputField.Text = hre.ToString();
            }
            catch (TaskCanceledException)
            {
                //rootPage.NotifyUser("Request canceled.", NotifyType.ErrorMessage);
            }
        }

    }

    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };
}
