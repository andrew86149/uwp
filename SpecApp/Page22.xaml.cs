using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Popups;
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
    public sealed partial class Page22 : Page
    {
        Compass compass = Compass.GetDefault();

        public Page22()
        {
            this.InitializeComponent();
            pname.Text = "Page 22";

            DisplayProperties.AutoRotationPreferences = DisplayProperties.NativeOrientation;
            Loaded += OnMainPageLoaded;
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        async void OnMainPageLoaded(object sender, RoutedEventArgs args)
        {
            if (compass != null)
            {
                ShowCompassValues(compass.GetCurrentReading());
                compass.ReportInterval = compass.MinimumReportInterval;
                compass.ReadingChanged += OnCompassReadingChanged;
            }
            else
            {
                await new MessageDialog("Compass is not available").ShowAsync();
            }
        }

        async void OnCompassReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ShowCompassValues(args.Reading);
            });
        }

        void ShowCompassValues(CompassReading compassReading)
        {
            if (compassReading == null)
                return;

            magNorthRotate.Angle = -compassReading.HeadingMagneticNorth;

            if (compassReading.HeadingTrueNorth.HasValue)
            {
                trueNorthPath.Visibility = Visibility.Visible;
                trueNorthRotate.Angle = -compassReading.HeadingTrueNorth.Value;
            }
            else
            {
                trueNorthPath.Visibility = Visibility.Collapsed;
            }
        }
    }
}
