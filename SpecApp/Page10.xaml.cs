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
    public sealed partial class Page10 : Page
    {
        public Page10()
        {
            this.InitializeComponent();
            pname.Text = "Page 10";

            Loaded += (sender, args) =>
            {
                polyline.Points.Add(new Point(drawingGrid.ActualWidth / 2,
                                              drawingGrid.ActualHeight / 2));
            };
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        void OnDialValueChanged(object sender, RangeBaseValueChangedEventArgs args)
        {
            Dial dial = sender as Dial;
            RotateTransform rotate = dial.RenderTransform as RotateTransform;
            rotate.Angle = args.NewValue;

            double xFraction = (horzDial.Value - horzDial.Minimum) /
                                    (horzDial.Maximum - horzDial.Minimum);

            double yFraction = (vertDial.Value - vertDial.Minimum) /
                                    (vertDial.Maximum - vertDial.Minimum);

            double x = xFraction * drawingGrid.ActualWidth;
            double y = yFraction * drawingGrid.ActualHeight;
            polyline.Points.Add(new Point(x, y));
        }

        void OnClearButtonClick(object sender, RoutedEventArgs args)
        {
            polyline.Points.Clear();
        }
    }
}
