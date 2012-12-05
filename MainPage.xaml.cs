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
using System.Threading.Tasks;   /* Tasks */
using Memento.Common; 

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Memento
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Memento.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        public static MainPage Current;
        
        MainPage rootPage = MainPage.Current;
        private Windows.Foundation.Collections.IPropertySet appSettings;
        private const String videoKey = "capturedVideo";
        public static string filePath; 


        HttpClient httpClient;

        #region Default constructor
        public MainPage()
        {
            this.InitializeComponent();
            appSettings = ApplicationData.Current.LocalSettings.Values;
           
        }
        #endregion 

        #region OnNavigatedTo
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
        #endregion 

        #region CaptureVideo_Click
        /// <summary>
        /// This is the click handler for the 'CaptureButton' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CaptureVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Using Windows.Media.Capture.CameraCaptureUI API to capture a photo
                CameraCaptureUI dialog = new CameraCaptureUI();
                dialog.VideoSettings.Format = CameraCaptureUIVideoFormat.Mp4;

                StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Video);

                if (file != null)
                {
                    IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                    CapturedVideo.SetSource(fileStream, "video/mp4");

                    // Store the file path in Application Data
                    // Each time you Capture a video file.Path is a different, randomly generated path. 
                    appSettings[videoKey] = file.Path;
                    filePath = file.Path;       // Set the global variable so when you record a video, that's that video that will send 
                    
                        
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion 

        #region NotifyUser
        /// <summary>
        ///  This function will be used to notify the user of the status of their video. 
        ///         i.e., Video transmission success or failure
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="type"></param>
        public void NotifyUser(string strMessage, NotifyType type)
        {
        }
        #endregion 

        #region ReloadVideo
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
            }
            catch (Exception ex)
            {
                appSettings.Remove(videoKey);
            }
        }
        #endregion 

        #region Send button click 
        private async void sendBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MultipartFormDataContent form = new MultipartFormDataContent(); 
                StorageFile file = await StorageFile.GetFileFromPathAsync(MainPage.filePath);
                var stream = await file.OpenReadAsync();
                StreamContent streamContent = new StreamContent(stream.AsStream(), 1024);
                form.Add(streamContent, "video", file.Path);
                string address = "http://momento.wadec.com/upload";
                //HttpResponseMessage response = await httpClient.PostAsync(address, form);
                progressRing.Visibility = Visibility.Visible;

                var output = string.Format("Your video was sent successfully!\nClick here to view it online: [ Link ]\nShare your video:\n\tTwitter\n\tFacebook\n\tYouTube"); 
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(output);
                await dialog.ShowAsync();
                progressRing.Visibility = Visibility.Collapsed; 
            }
            catch (HttpRequestException hre)
            {
            }
            catch (TaskCanceledException)
            {
            }
        }
        #endregion 

        private void discardBtn_Click(object sender, RoutedEventArgs e)
        {
            appSettings.Remove(videoKey);
        }

        private async void playBtn_Click_1(object sender, RoutedEventArgs e)
        {
            if (appSettings.ContainsKey(videoKey))
            {
                object filePath;
                if (appSettings.TryGetValue(videoKey, out filePath) && filePath.ToString() != "")
                {

                    await ReloadVideo(filePath.ToString());
                }
            }
        }

        private async void newvideoBtn_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Using Windows.Media.Capture.CameraCaptureUI API to capture a photo
                CameraCaptureUI dialog = new CameraCaptureUI();
                dialog.VideoSettings.Format = CameraCaptureUIVideoFormat.Mp4;

                StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Video);

                if (file != null)
                {
                    IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                    CapturedVideo.SetSource(fileStream, "video/mp4");

                    // Store the file path in Application Data
                    // Each time you Capture a video file.Path is a different, randomly generated path. 
                    appSettings[videoKey] = file.Path;
                    filePath = file.Path;       // Set the global variable so when you record a video, that's that video that will send 


                }
            }
            catch (Exception ex)
            {
            }
        }

        private async void uploadBtn_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                MultipartFormDataContent form = new MultipartFormDataContent();
                StorageFile file = await StorageFile.GetFileFromPathAsync(MainPage.filePath);
                var stream = await file.OpenReadAsync();
                StreamContent streamContent = new StreamContent(stream.AsStream(), 1024);
                form.Add(streamContent, "video", file.Path);
                string address = "http://momento.wadec.com/upload";
                //HttpResponseMessage response = await httpClient.PostAsync(address, form);
                progressRing.Visibility = Visibility.Visible;

                var output = string.Format("Your video was sent successfully!\nClick here to view it online: [ Link ]\nShare your video:\n\tTwitter\n\tFacebook\n\tYouTube");
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(output);
                await dialog.ShowAsync();
                progressRing.Visibility = Visibility.Collapsed;
            }
            catch (HttpRequestException hre)
            {
            }
            catch (TaskCanceledException)
            {
            }
        }

        private void discardButton_Click_1(object sender, RoutedEventArgs e)
        {
            appSettings.Remove(videoKey);
            CapturedVideo.Source = null; 
        }

    }

    #region NotifyType enum 
    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };
    #endregion 
}
