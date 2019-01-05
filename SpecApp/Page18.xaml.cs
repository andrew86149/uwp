using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpecApp
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class Page18 : Page
    {
        Accelerometer accelerometer = Accelerometer.GetDefault();
        SimpleOrientationSensor simpleOrientationSensor = SimpleOrientationSensor.GetDefault();

        public Page18()
        {
            this.InitializeComponent();
            pname.Text = "Page 18";

            //Loaded += (sender, args) =>
            //{
            //    (this.Resources["storyboard"] as Storyboard).Begin();
            //};
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void AppBarButton_Click1(object sender, RoutedEventArgs e)
        {
            (this.Resources["storyboard"] as Storyboard).Begin();
        }

        async private void AppBarButton_Click2(object sender, RoutedEventArgs e)
        {
            if (accelerometer == null)
                await new MessageDialog("Cannot start Accelerometer").ShowAsync();

            if (simpleOrientationSensor == null)
                await new MessageDialog("Cannot start SimpleOrientationSensor").ShowAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            if (accelerometer != null)
            {
                SetAccelerometerText(accelerometer.GetCurrentReading());
                accelerometer.ReadingChanged += OnAccelerometerReadingChanged;
            }

            if (simpleOrientationSensor != null)
            {
                SetSimpleOrientationText(simpleOrientationSensor.GetCurrentOrientation());
                simpleOrientationSensor.OrientationChanged += OnSimpleOrientationChanged;
            }
            base.OnNavigatedTo(args);
        }

        // Detach event handlers
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            if (accelerometer != null)
                accelerometer.ReadingChanged -= OnAccelerometerReadingChanged;

            if (simpleOrientationSensor != null)
                simpleOrientationSensor.OrientationChanged -= OnSimpleOrientationChanged;

            base.OnNavigatedFrom(args);
        }

        // Accelerometer handler
        async void OnAccelerometerReadingChanged(Accelerometer sender,
                                                 AccelerometerReadingChangedEventArgs args)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SetAccelerometerText(args.Reading);
            });
        }

        void SetAccelerometerText(AccelerometerReading accelerometerReading)
        {
            if (accelerometerReading == null)
                return;

            accelerometerX.Text = accelerometerReading.AccelerationX.ToString("F2");
            accelerometerY.Text = accelerometerReading.AccelerationY.ToString("F2");
            accelerometerZ.Text = accelerometerReading.AccelerationZ.ToString("F2");
            magnitude.Text =
                Math.Sqrt(Math.Pow(accelerometerReading.AccelerationX, 2) +
                          Math.Pow(accelerometerReading.AccelerationY, 2) +
                          Math.Pow(accelerometerReading.AccelerationZ, 2)).ToString("F2");
        }

        // SimpleOrientationSensor handler
        async void OnSimpleOrientationChanged(SimpleOrientationSensor sender,
                                              SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SetSimpleOrientationText(args.Orientation);
            });
        }

        void SetSimpleOrientationText(SimpleOrientation simpleOrientation)
        {
            this.simpleOrientation.Text = simpleOrientation.ToString();
        }
    }
}
