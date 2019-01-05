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
    public sealed partial class Page0 : Page
    {
        SimpleOrientationSensor simpleOrientationSensor = SimpleOrientationSensor.GetDefault();
        DisplayInformation displayInformation;

        public Page0()
        {
            this.InitializeComponent();
            pname.Text = "Page 0";

            this.SizeChanged += OnMainPageSizeChanged;
            displayInformation = DisplayInformation.GetForCurrentView();
            displayInformation.DpiChanged += OnDpiChanged;

            Loaded += (sender, args) =>
            {
                UpdateDisplay();
            };

            if (simpleOrientationSensor != null)
            {
                SetOrientationSensorText(simpleOrientationSensor.GetCurrentOrientation());
                simpleOrientationSensor.OrientationChanged += OnSimpleOrientationChanged;
            }
        }

        async private void OnSimpleOrientationChanged(SimpleOrientationSensor sender, 
            SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SetOrientationSensorText(args.Orientation);
            });
        }

        private void SetOrientationSensorText(SimpleOrientation simpleOrientation)
        {
            orientationSensorTextBlock.Text = simpleOrientation.ToString();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void OnDpiChanged(DisplayInformation sender, object args)
        {
            UpdateDisplay();
        }
        void OnMainPageSizeChanged(object sender, SizeChangedEventArgs args)
        {
            UpdateDisplay();
        }

        void OnLogicalDpiChanged(object sender)
        {
            UpdateDisplay();
        }

        void UpdateDisplay()
        {
            displayInformation = DisplayInformation.GetForCurrentView();
            double logicalDpi = displayInformation.LogicalDpi;

            int pixelWidth = (int)Math.Round(logicalDpi * this.ActualWidth / 96);
            int pixelHeight = (int)Math.Round(logicalDpi * this.ActualHeight / 96);

            textBlock.Text =
                String.Format("Window size = {0} x {1}\r\n" +
                              "ResolutionScale = {2}\r\n" +
                              "Logical DPI = {3}\r\n" +
                              "Pixel size = {4} x {5}",
                              this.ActualWidth, this.ActualHeight,
                              displayInformation.ResolutionScale,
                              displayInformation.LogicalDpi,
                              pixelWidth, pixelHeight);
            displayOrientationTextBlock.Text = displayInformation.CurrentOrientation.ToString();
        }
    }
}
