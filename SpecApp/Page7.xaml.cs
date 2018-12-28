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
    public sealed partial class Page7 : Page
    {
        int xDirection;
        int yDirection;

        public Page7()
        {
            this.InitializeComponent();
            pname.Text = "Page 7";
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        void OnEllipseManipulationStarted(object sender, ManipulationStartedRoutedEventArgs args)
        {
            // Initialize directions
            xDirection = 1;
            yDirection = 1;
        }

        void OnEllipseManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs args)
        {
            // Find new position of ellipse regardless of edges
            double x = Canvas.GetLeft(ellipse) + xDirection * args.Delta.Translation.X;
            double y = Canvas.GetTop(ellipse) + yDirection * args.Delta.Translation.Y;

            if (args.IsInertial)
            {
                // Bounce it off the edges
                Size playground = new Size(contentGrid.ActualWidth - ellipse.Width,
                                           contentGrid.ActualHeight - ellipse.Height);

                while (x < 0 || y < 0 || x > playground.Width || y > playground.Height)
                {
                    if (x < 0)
                    {
                        x = -x;
                        xDirection *= -1;
                    }
                    if (x > playground.Width)
                    {
                        x = 2 * playground.Width - x;
                        xDirection *= -1;
                    }
                    if (y < 0)
                    {
                        y = -y;
                        yDirection *= -1;
                    }
                    if (y > playground.Height)
                    {
                        y = 2 * playground.Height - y;
                        yDirection *= -1;
                    }
                }
            }

            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);
        }

        void OnEllipseManipulationInertiaStarting(object sender,
                                                  ManipulationInertiaStartingRoutedEventArgs args)
        {
            double maxVelocity = Math.Max(Math.Abs(args.Velocities.Linear.X),
                                          Math.Abs(args.Velocities.Linear.Y));

            args.TranslationBehavior.DesiredDeceleration = maxVelocity / (1000 * slider.Value);
        }
    }
}
