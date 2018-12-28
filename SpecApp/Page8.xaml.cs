using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
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
    public sealed partial class Page8 : Page
    {
        bool manualChange = false;

        public Page8()
        {
            this.InitializeComponent();
            pname.Text = "Page 8";

            Loaded += async (sender, args) =>
            {
                Geolocator geolocator = new Geolocator();

                // Might not have permission!
                try
                {
                    Geoposition position = await geolocator.GetGeopositionAsync();

                    if (!manualChange)
                    {
                        double x = (position.Coordinate.Longitude + 180) / 360;
                        double y = (90 - position.Coordinate.Latitude) / 180;
                        xySlider.Value = new Point(x, y);
                    }
                }
                catch
                {
                }
            };
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        void OnXYSliderValueChanged(object sender, Point point)
        {
            double longitude = 360 * point.X - 180;
            double latitude = 90 - 180 * point.Y;
            label.Text = String.Format("Longitude: {0:F0} Latitude: {1:F0}",
                                       longitude, latitude);
            manualChange = true;
        }
    }
}
