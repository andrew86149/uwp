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
    public sealed partial class Page23 : Page
    {
        Inclinometer inclinometer = Inclinometer.GetDefault();

        public Page23()
        {
            this.InitializeComponent();
            pname.Text = "Page 23";

            DisplayProperties.AutoRotationPreferences = DisplayProperties.NativeOrientation;
            Loaded += OnMainPageLoaded;
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        async void OnMainPageLoaded(object sender, RoutedEventArgs args)
        {
            if (inclinometer == null)
            {
                await new MessageDialog("Cannot obtain Inclinometer").ShowAsync();
            }
            else
            {
                ShowYawPitchRoll(inclinometer.GetCurrentReading());
                inclinometer.ReportInterval = inclinometer.MinimumReportInterval;
                inclinometer.ReadingChanged += OnInclinometerReadingChanged;
            }
        }

        async void OnInclinometerReadingChanged(Inclinometer sender,
                                                InclinometerReadingChangedEventArgs args)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ShowYawPitchRoll(args.Reading);
            });
        }

        void ShowYawPitchRoll(InclinometerReading inclinometerReading)
        {
            if (inclinometerReading == null)
                return;

            double yaw = inclinometerReading.YawDegrees;
            double pitch = inclinometerReading.PitchDegrees;
            double roll = inclinometerReading.RollDegrees;

            yawValue.Text = yaw.ToString("F0") + "°";
            pitchValue.Text = pitch.ToString("F0") + "°";
            rollValue.Text = roll.ToString("F0") + "°";

            yawRotate.Angle = yaw;

            if (pitch <= 90 && pitch >= -90)
            {
                pitchPath.Fill = pitchPath.Stroke;
                pitchEllipse.Center = new Point(this.ActualWidth / 2,
                                                this.ActualHeight * (pitch + 90) / 180);
            }
            else
            {
                pitchPath.Fill = null;

                if (pitch > 90)
                    pitchEllipse.Center = new Point(this.ActualWidth / 2,
                                                    this.ActualHeight * (270 - pitch) / 180);
                else // pitch < -90
                    pitchEllipse.Center = new Point(this.ActualWidth / 2,
                                                    this.ActualHeight * (-90 - pitch) / 180);
            }
            rollEllipse.Center = new Point(this.ActualWidth * (roll + 90) / 180,
                                           this.ActualHeight / 2);
        }
    }
}
