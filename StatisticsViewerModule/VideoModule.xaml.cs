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
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.WinForms;

namespace StatisticsViewerModule
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
            OnTimeCodeChanged += (o, e) => { };
            //StartVideo("");
        }

        public void StartVideo(string url)
        {
            InitializeWebView2(url);
        }
        private async void InitializeWebView2(string url)
        {
            // Инициализация WebView2
            await YouTubeWebView.EnsureCoreWebView2Async(null);

            // Установка URL для воспроизведения видео с YouTube
            YouTubeWebView.CoreWebView2.WebMessageReceived += WebView2Control_WebMessageReceived;
            YouTubeWebView.NavigationCompleted += WebView2Control_NavigationCompleted;
            if(url == "") YouTubeWebView.CoreWebView2.Navigate("https://youtu.be/f1cpF2xF3Mw");
            else YouTubeWebView.CoreWebView2.Navigate(url);
            
        }
        double currentTimeCode = 0;
        private void WebView2Control_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            // Получаем сообщение от JavaScript
            string message = e.WebMessageAsJson;
            if (double.TryParse(message, out double currentTime))
            {
                // Обновляем текстовый блок с текущим временем
                currentTimeCode = currentTime;
                OnTimeCodeChanged(this, new TimeCodeEventArgs(currentTime));
            }
        }
        private async void WebView2Control_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                // Успешная загрузка страницы, можно запускать скрипт
                string script = @"
                    setInterval(function() {
                        let video = document.querySelector('video');
                        if (video) {
                            window.chrome.webview.postMessage(video.currentTime);
                        }
                    }, 300);"; // Обновление каждые 100 мс

                await YouTubeWebView.CoreWebView2.ExecuteScriptAsync(script);
            }
            else
            {
                MessageBox.Show("Failed to load the page.");
            }
        }
        public async void SetYouTubeTimeCode(double timeInSeconds)
        {
            string script = $"document.getElementById('movie_player').seekTo({timeInSeconds}, true);";
            await YouTubeWebView.ExecuteScriptAsync(script);
        }
        public event TimeCodeChanged OnTimeCodeChanged;
        public double GetTimeCode()
        {
            return currentTimeCode;
        }
        public void SetTimeCode()
        {

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
