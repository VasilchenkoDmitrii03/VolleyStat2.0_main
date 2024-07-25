using System;
using System.Collections.Generic;
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
using Microsoft.Web.WebView2.Core;

namespace StatisticsCreatorModule
{
    /// <summary>
    /// Interaction logic for VideoModule.xaml
    /// </summary>
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
            LoadYouTubeVideo("oG0e7xVufZk"); // Замените на ваш видео ID
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (e.TryGetWebMessageAsString() == "playerReady")
            {
                // Плеер готов, можно управлять видео
            }
        }

        private void LoadYouTubeVideo(string videoId)
        {
            string embedHtml = $@"
                <html>
                <head>
                    <meta http-equiv='X-UA-Compatible' content='IE=11' />
                    <script src='https://www.youtube.com/iframe_api'></script>
                    <script>
                        var player;
                        function onYouTubeIframeAPIReady() {{
                            player = new YT.Player('player', {{
                                height: '100%',
                                width: '100%',
                                videoId: '{videoId}',
                                events: {{
                                    'onReady': onPlayerReady
                                }}
                            }});
                        }}
                        function onPlayerReady(event) {{
                            window.chrome.webview.postMessage('playerReady');
                        }}
                        function seekTo(seconds) {{
                            if (player) {{
                                player.seekTo(seconds, true);
                            }}
                        }}
                        function getCurrentTime() {{
                            return player.getCurrentTime();
                        }}
                    </script>
                </head>
                <body style='margin:0px;padding:0px;overflow:hidden'>
                    <div id='player'></div>
                </body>
                </html>";

            YouTubeWebView.NavigateToString(embedHtml);
        }

        private void SeekTo(int seconds)
        {
            YouTubeWebView.CoreWebView2.ExecuteScriptAsync($"seekTo({seconds});");
        }

        private async void getCurrentTimeCode()
        {
            string currentTime = await YouTubeWebView.CoreWebView2.ExecuteScriptAsync($"getCurrentTime()");
            currentTime = currentTime.Trim('"'); // Remove quotes from the JSON response
            timeCode = Convert.ToDouble(currentTime);
            
        }
        double timeCode = -1;
        private double getTimeCode()
        {
            getCurrentTimeCode();
            while (timeCode == -1)
            {
                Thread.Sleep(10);
            }
            return timeCode;
        }
        private void Button_Click_10s(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(getTimeCode().ToString());
            //SeekTo(10); // Перейти к 10 секунде видео
        }

        private void Button_Click_30s(object sender, RoutedEventArgs e)
        {
            SeekTo(30); // Перейти к 30 секунде видео
        }
    }
}
