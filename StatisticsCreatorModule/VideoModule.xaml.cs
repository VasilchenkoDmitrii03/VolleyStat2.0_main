using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using Microsoft.Web.WebView2.Core;
using System.Text.Json;

namespace StatisticsCreatorModule
{
    /// <summary>
    /// Interaction logic for VideoModule.xaml
    /// </summary>
    
    public delegate void TimeCodeChanged(object sender, TimeCodeEventArgs e);
    public partial class VideoModule : UserControl
    {
        public VideoModule()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await YouTubeWebView.EnsureCoreWebView2Async(null);
            YouTubeWebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            string htmlString = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <script src=""https://www.youtube.com/iframe_api""></script>
                    <script>
                        var player;
                        function onYouTubeIframeAPIReady() {
                            player = new YT.Player('player', {
                                height: '390',
                                width: '640',
                                videoId: 'ks6yJSDPBAY',
                                events: {
                                    'onReady': onPlayerReady,
                                    'onStateChange': onPlayerStateChange
                                }
                            });
                        }

                        function onPlayerReady(event) {
                            // Видео готово к воспроизведению
                        }

                        function onPlayerStateChange(event) {
                            if (event.data == YT.PlayerState.PLAYING) {
                                setInterval(() => {
                                    window.chrome.webview.postMessage({ type: 'currentTime', time: player.getCurrentTime() });
                                }, 250);
                            }
                        }

                        function getCurrentTime() {
                            return player.getCurrentTime();
                        }

                        function seekTo(time) {
                            player.seekTo(time, true);
                        }
                    </script>
                </head>
                <body>
                    <div id=""player""></div>
                </body>
                </html>";
            YouTubeWebView.NavigateToString(htmlString);
        }
        public TimeCodeChanged TimeCodeChanged;

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var message = e.WebMessageAsJson;
            using JsonDocument doc = JsonDocument.Parse(message);
            JsonElement root = doc.RootElement;

            var time = root.GetProperty("time");
            
            

            if (message != null)
            {
                var data = JsonSerializer.Deserialize<WebMessage>(message);
                CurrentTime = (double)time.GetDouble();
                TimeCodeChanged(this, new TimeCodeEventArgs(CurrentTime));
            }
        }
        double CurrentTime = 0;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(CurrentTime.ToString());
        }


        #region Themese module
        private void LoadTheme()
        {
            ResourceDictionary themeDict = new ResourceDictionary();
            // Определяем, какая тема загружена в приложении
            if (Application.Current.Resources.MergedDictionaries[0].Source.ToString().Contains("LightTheme"))
            {
                themeDict.Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative);
            }
            else
            {
                themeDict.Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative);
            }

            this.Resources.MergedDictionaries.Add(themeDict);
        }
        public void UpdateTheme()
        {
            this.Resources.MergedDictionaries.Clear();
            LoadTheme();
        }
        #endregion
    }

    public class TimeCodeEventArgs : EventArgs
    {
        public double currentTimeCode;
        public TimeCodeEventArgs(double t)
        {
            currentTimeCode = t;
        }
    }

    public class WebMessage
    {
        public string Type { get; set; }
        public double Time { get; set; }
    }
}
