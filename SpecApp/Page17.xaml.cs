using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpecApp
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class Page17 : Page
    {
        MediaCapture mediaCapture = new MediaCapture();
        bool ignoreTaps = false;

        public Page17()
        {
            this.InitializeComponent();
            pname.Text = "Page 17";
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
        async void OnButtonClick(object sender, RoutedEventArgs args)
        {
            CameraCaptureUI cameraCap = new CameraCaptureUI();

            cameraCap.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.VerySmallQvga;

            StorageFile storageFile = await cameraCap.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (storageFile != null)
            {
                IRandomAccessStreamWithContentType stream = await storageFile.OpenReadAsync();
                BitmapImage bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(stream);
                image.Source = bitmap;
            }
        }

        async private void AppBarButton_Click2(object sender, RoutedEventArgs e)
        {
            DeviceInformationCollection devInfos =
                await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            if (devInfos.Count == 0)
            {
                await new MessageDialog("No video capture devices found").ShowAsync();
                return;
            }

            string id = null;

            // Try to find the front webcam
            foreach (DeviceInformation devInfo in devInfos)
            {
                if (devInfo.EnclosureLocation != null &&
                        devInfo.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front)
                    id = devInfo.Id;
            }

            // If not available, just pick the first one
            if (id == null)
                id = devInfos[0].Id;

            // Create initialization settings
            MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
            settings.VideoDeviceId = id;
            settings.StreamingCaptureMode = StreamingCaptureMode.Video;

            // Initialize the MediaCapture device
            await mediaCapture.InitializeAsync(settings);

            // Associate with the CaptureElement
            captureElement.Source = mediaCapture;

            // Start the preview
            await mediaCapture.StartPreviewAsync();
        }

        async private void AppBarButton_Click3(object sender, RoutedEventArgs e)
        {
            if (ignoreTaps)
                return;

            // Capture photo to memory stream
            ImageEncodingProperties imageEncodingProps = ImageEncodingProperties.CreateJpeg();
            InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();
            await mediaCapture.CapturePhotoToStreamAsync(imageEncodingProps, memoryStream);

            // Use BitmapDecoder to get pixels array
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(memoryStream);
            PixelDataProvider pixelProvider = await decoder.GetPixelDataAsync();
            byte[] pixels = pixelProvider.DetachPixelData();

            // Saturate the colors
            for (int index = 0; index < pixels.Length; index += 4)
            {
                Color color = Color.FromArgb(pixels[index + 3],
                                             pixels[index + 2],
                                             pixels[index + 1],
                                             pixels[index + 0]);
                HSL hsl = new HSL(color);
                hsl = new HSL(hsl.Hue, 1.0, hsl.Lightness);
                color = hsl.Color;

                pixels[index + 0] = color.B;
                pixels[index + 1] = color.G;
                pixels[index + 2] = color.R;
                pixels[index + 3] = color.A;
            }

            // Create a WriteableBitmap and initialize it
            WriteableBitmap bitmap = new WriteableBitmap((int)decoder.PixelWidth,
                                                         (int)decoder.PixelHeight);
            Stream pixelStream = bitmap.PixelBuffer.AsStream();
            await pixelStream.WriteAsync(pixels, 0, pixels.Length);
            bitmap.Invalidate();

            // Display the bitmap
            image.Source = bitmap;

            // Set a timer for the image
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2.5)
            };
            timer.Tick += OnTimerTick;
            timer.Start();
            ignoreTaps = true;
        }

        void OnTimerTick(object sender, object args)
        {
            // Disable the timer
            DispatcherTimer timer = sender as DispatcherTimer;
            timer.Stop();
            timer.Tick -= OnTimerTick;

            // Get rid of the bitmap
            image.Source = null;
            ignoreTaps = false;
        }
    }
}
