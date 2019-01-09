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
    public sealed partial class Page19 : Page
    {
        Accelerometer accelerometer = Accelerometer.GetDefault();
        //DisplayInformation displayInformation;

        public Page19()
        {
            this.InitializeComponent();
            pname.Text = "Page 19";

            //DisplayProperties.AutoRotationPreferences = DisplayProperties.NativeOrientation;
            //displayInformation.
            Loaded += OnMainPageLoaded;
            SizeChanged += OnMainPageSizeChanged;
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        async void OnMainPageLoaded(object sender, RoutedEventArgs args)
        {
            if (accelerometer != null)
            {
                accelerometer.ReportInterval = accelerometer.MinimumReportInterval;
                SetBubble(accelerometer.GetCurrentReading());
                accelerometer.ReadingChanged += OnAccelerometerReadingChanged;
            }
            else
            {
                await new MessageDialog("Accelerometer is not available").ShowAsync();
            }
        }

        void OnMainPageSizeChanged(object sender, SizeChangedEventArgs args)
        {
            double size = Math.Min(args.NewSize.Width, args.NewSize.Height);
            outerCircle.Width = size;
            outerCircle.Height = size;
            halfCircle.Width = size / 2;
            halfCircle.Height = size / 2;
        }

        async void OnAccelerometerReadingChanged(Accelerometer sender,
                                                 AccelerometerReadingChangedEventArgs args)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SetBubble(args.Reading);
            });
        }

        void SetBubble(AccelerometerReading accelerometerReading)
        {
            if (accelerometerReading == null)
                return;

            double x = accelerometerReading.AccelerationX;
            double y = accelerometerReading.AccelerationY;

            bubbleTranslate.X = -x * centeredGrid.ActualWidth / 2;
            bubbleTranslate.Y = y * centeredGrid.ActualHeight / 2;
        }
    }
}
