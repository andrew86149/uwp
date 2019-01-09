using MyToolkit.Multimedia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpecApp
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class PageVideo : Page
    {
        public PageVideo()
        {
            this.InitializeComponent();
            pname.Text = "Page Video";

            //Loaded += OnPageVideoLoaded;
        }

        //async private void OnPageVideoLoaded(object sender, RoutedEventArgs e)
        //{
            //string youtubeId = "mnjbHvYhl-w";//"OgbmOjon0Mw";
            //string youtubeId = str;
            
        //}

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string str = e.Parameter as string;
            string youtubeId = str;
            //pname.Text = str;
            try
            {
                YouTubeUri url = await YouTube.GetVideoUriAsync(youtubeId, YouTubeQuality.Quality360P);
                //txtblk.Text = "Uri = " + url.Uri.ToString();
                mediaElt.Source = url.Uri;
                mediaElt.AreTransportControlsEnabled = true;
                mediaElt.MaxHeight = 320;
                //mediaElt.IsMuted = true;
                mediaElt.Play();
            }
            catch (Exception) { }
            base.OnNavigatedTo(e);
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
