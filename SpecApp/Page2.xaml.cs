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
    public sealed partial class Page2 : Page
    {
        DispatcherTimer timer;

        public Page2()
        {
            this.InitializeComponent();
            pname.Text = "Page 2";

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            timer.Tick += OnTimerTick;
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        void OnClearButtonClick(object sender, RoutedEventArgs args)
        {
            logger.Clear();
        }

        void OnCaptureToggleButtonChecked(object sender, RoutedEventArgs args)
        {
            ToggleButton toggle = sender as ToggleButton;
            logger.CaptureOnPress = toggle.IsChecked.Value;
        }

        void OnReleaseCapturesButtonClick(object sender, RoutedEventArgs args)
        {
            timer.Start();
        }

        void OnTimerTick(object sender, object args)
        {
            logger.ReleasePointerCaptures();
            timer.Stop();
        }
    }
}
