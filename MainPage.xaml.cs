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
using System.Text.RegularExpressions; 


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Memento
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Memento.Common.LayoutAwarePage
    {
        #region Variable declarations 
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        public static MainPage Current;
        
        MainPage rootPage = MainPage.Current;
        private Windows.Foundation.Collections.IPropertySet appSettings;
        private const String videoKey = "capturedVideo";
        private const String fileKey = "filePath";
        private const String usernameKey = "Username";
        private const String passwordKey = "Password"; 
        public static string filePath; 
        HttpClient httpClient;
        #endregion 

        #region Default constructor
        public MainPage()
        {
            this.InitializeComponent();
            appSettings = ApplicationData.Current.LocalSettings.Values;

            var bounds = Window.Current.Bounds;
            var height = bounds.Height;
            var width = bounds.Width;
            VidGrid.Width = .7 * width;     // VidGrid takes up the left half of the page
            VideoInformationGrid.Width = .35 * width; 



            // If the user is not logged in 
            if (!appSettings.ContainsKey(usernameKey) || !appSettings.ContainsKey(passwordKey))
            {
                loginPopUp.IsOpen = true;
                usernameTxtBox.Focus(Windows.UI.Xaml.FocusState.Keyboard); 
                logoutBtn.Visibility = Visibility.Collapsed;
                accoutnBtn.Visibility = Visibility.Collapsed;
                videosBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                loginPopUp.IsOpen = false;
                logoutBtn.Visibility = Visibility.Visible;
                accoutnBtn.Visibility = Visibility.Visible;
                videosBtn.Visibility = Visibility.Visible; 
            }

       

            List<DummiePerson> list = new List<DummiePerson>()
            {
                new DummiePerson{Image_Name="Made some good progress", Image="Person1.jpg", Description="I finally figured out how to calculate congruency complexity!"},
                new DummiePerson{Image_Name="Signing off!", Image="Person2.jpg", Description="Thank you all for the awesome experience!"},
                new DummiePerson{Image_Name="Just woke up", Image="Person3.jpg", Description="I had such a good time with my friends tonight."},
                new DummiePerson{Image_Name="Talk about a bad hair day...", Image="Person4.jpg", Description="You know what's awesome about summer? Long hair."},
                new DummiePerson{Image_Name="Haircuts... ", Image="Person6.jpg", Description="I don't like haircuts one bit. That's right, I said. it. "},
            };
            DataContext = list;
            groupedItemsViewSource.Source = list;

            OpenVideo(); 
        
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

            if (!appSettings.ContainsKey(usernameKey) || !appSettings.ContainsKey(passwordKey))
            {
                loginPopUp.IsOpen = true;               // so prompt the user to login 
                usernameTxtBox.Focus(Windows.UI.Xaml.FocusState.Keyboard);
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

        #region OpenVideo
        private async void OpenVideo()
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
                    appSettings[fileKey] = file.Path; 
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
                // TODO: Figure out how to resume a paused video
                CapturedVideo.SetSource(fileStream, "video/mp4");
                 
            }
            catch (Exception ex)
            {
                //appSettings.Remove(videoKey);
            }
        }
        #endregion 

        #region Play button click 
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
            else
            {
                //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("There is no video file to play.");
                //await dialog.ShowAsync();
            }

        }
        #endregion 

        #region New video click 
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
        #endregion 

        #region Upload button click 
        private async void uploadBtn_Click_1(object sender, RoutedEventArgs e)
        {
           /* video_metadataPopup.IsOpen = true;
            while (video_metadataPopup.IsOpen) ; */
            ShowVideoDetailsDialog();
            titleTxtBox.Focus(Windows.UI.Xaml.FocusState.Keyboard);
            
        }
        #endregion 

        #region Discard button click 
        private async void discardButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (appSettings.ContainsKey(videoKey))
            {
                appSettings.Remove(videoKey);
                CapturedVideo.Source = null;
            }
            else
            {
                //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("There is no video file to discard.");
                //await dialog.ShowAsync();
            }
        }
        #endregion 

        #region My videos app bar button click 
        private void my_videosBtn_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(VideosPage));
        }
        #endregion 

        #region Submit login credentials button click
        private void submitLoginBtn_Click_1(object sender, RoutedEventArgs e)
        {
            // TODO: Verify login credentials against user database 
            appSettings[usernameKey] = usernameTxtBox.Text;        
            appSettings[passwordKey] = passwordTxtBox.Password; 
            loginPopUp.IsOpen = false; 
            // if(login is valid)
            logoutBtn.Content = "Log out";
            logoutBtn.Visibility = Visibility.Visible;
            accoutnBtn.Visibility = Visibility.Visible;
            videosBtn.Visibility = Visibility.Visible;
            signupBtn.Visibility = Visibility.Collapsed; 
            BottomAppBar.IsOpen = true; 
        }
        #endregion 

        #region Submit video button click
        private async void submit_videoBtn_Click_1(object sender, RoutedEventArgs e)
        {
            video_metadataPopup.IsOpen = false;

            try
            {
                if (appSettings.ContainsKey(videoKey))
                {
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    // BUG HERE: If the video was recorded previously, this breaks. 
                    // That's because videos are only being stored temporarily right now. 
                    StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
                    var stream = await file.OpenReadAsync();
                    StreamContent streamContent = new StreamContent(stream.AsStream(), 1024);
                    form.Add(streamContent, "video", file.Path);
                    string address = "http://momento.wadec.com/upload";
                    //HttpResponseMessage response = await httpClient.PostAsync(address, form);

                    var output = string.Format("Your video was sent successfully!\nView it online at momento.wadec.com");
                    output += "\nShare your video:\n\tTwitter\n\tFacebook\n\tYouTube";
                    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(output);
                    await dialog.ShowAsync();
                }
                else
                {
                    //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("There is no video file to upload.");
                    //await dialog.ShowAsync();
                }

            }
            catch (HttpRequestException hre)
            {
            }
            catch (TaskCanceledException)
            {
            }
        }
        #endregion

        #region ShowVideoDetailsDialog()
        public int ShowVideoDetailsDialog()
        {
            video_metadataPopup.IsOpen = true;

            return 1; 
        }
        #endregion

        #region Logout button click
        private async void logoutBtn_Click_1(object sender, RoutedEventArgs e)
        {
            if (logoutBtn.Content.ToString() == "Login")    // If the button is showing Login, the user is logged out
            {
                loginPopUp.IsOpen = true;               // so prompt the user to login 
                usernameTxtBox.Focus(Windows.UI.Xaml.FocusState.Keyboard); 
            }
            else
            {
                appSettings.Remove(usernameKey);    // Clear the username key from appSettings
                appSettings.Remove(passwordKey);    // Clear the password key from appSettings
                logoutBtn.Content = "Login";        // Change button text to display logout 

                usernameTxtBox.Text = "";       // Clear out the username textbox
                passwordTxtBox.Password = "";   // Clear out the password textbox
                accoutnBtn.Visibility = Visibility.Collapsed;
                videosBtn.Visibility = Visibility.Collapsed;
                signupBtn.Visibility = Visibility.Visible; 
                BottomAppBar.IsOpen = false; 
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("You are now logged out.");
                await dialog.ShowAsync();
            }
        }
        #endregion 

        #region Stop button click
        private void stopBtn_Click_1(object sender, RoutedEventArgs e)
        {
            CapturedVideo.Stop(); 
        }
        #endregion 

        #region Pause button click 
        private void pauseBtn_Click_1(object sender, RoutedEventArgs e)
        {
            CapturedVideo.Pause(); 
        }
        #endregion 

        #region Account button click 
        private void accoutnBtn_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AccountPage));
        }
        #endregion 
    }

    #region NotifyType enum 
    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };
    #endregion 
}
