using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
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
    public sealed partial class Page11 : Page
    {
        public Page11()
        {
            this.InitializeComponent();
            pname.Text = "Page 11";
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        async private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            WriteableBitmap bitmap = new WriteableBitmap(256, 256);
            byte[] pixels = new byte[4 * bitmap.PixelWidth * bitmap.PixelHeight];

            for (int y = 0; y < bitmap.PixelHeight; y++)
                for (int x = 0; x < bitmap.PixelWidth; x++)
                {
                    int index = 4 * (y * bitmap.PixelWidth + x);
                    pixels[index + 0] = (byte)x;    // Blue
                    pixels[index + 1] = 0;          // Green
                    pixels[index + 2] = (byte)y;    // Red
                    pixels[index + 3] = 255;        // Alpha
                }

            using (Stream pixelStream = bitmap.PixelBuffer.AsStream())
            {
                await pixelStream.WriteAsync(pixels, 0, pixels.Length);
            }
            bitmap.Invalidate();
            image.Source = bitmap;
        }

        async private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            WriteableBitmap bitmap = new WriteableBitmap(256, 256);
            byte[] pixels = new byte[4 * bitmap.PixelWidth * bitmap.PixelHeight];
            int index = 0;
            int centerX = bitmap.PixelWidth / 2;
            int centerY = bitmap.PixelHeight / 2;

            for (int y = 0; y < bitmap.PixelHeight; y++)
                for (int x = 0; x < bitmap.PixelWidth; x++)
                {
                    double angle =
                        Math.Atan2(((double)y - centerY) / bitmap.PixelHeight,
                                   ((double)x - centerX) / bitmap.PixelWidth);
                    double fraction = angle / (2 * Math.PI);
                    pixels[index++] = (byte)(fraction * 255);       // Blue
                    pixels[index++] = 0;                            // Green
                    pixels[index++] = (byte)(255 * (1 - fraction)); // Red
                    pixels[index++] = 255;                          // Alpha
                }

            using (Stream pixelStream = bitmap.PixelBuffer.AsStream())
            {
                await pixelStream.WriteAsync(pixels, 0, pixels.Length);
            }
            bitmap.Invalidate();
            imageBrush.ImageSource = bitmap;
        }

        async private void AppBarButton_Click_3(object sender, RoutedEventArgs e)
        {
            WriteableBitmap bitmap = new WriteableBitmap(256, 256);
            byte[] pixels = new byte[4 * bitmap.PixelWidth * bitmap.PixelHeight];
            int index = 0;
            int centerX = bitmap.PixelWidth / 2;
            int centerY = bitmap.PixelHeight / 2;

            for (int y = 0; y < bitmap.PixelHeight; y++)
                for (int x = 0; x < bitmap.PixelWidth; x++)
                {
                    double angle =
                        Math.Atan2(((double)y - centerY) / bitmap.PixelHeight,
                                   ((double)x - centerX) / bitmap.PixelWidth);
                    double fraction = angle / (2 * Math.PI);
                    pixels[index++] = (byte)(fraction * 255);       // Blue
                    pixels[index++] = 0;                            // Green
                    pixels[index++] = (byte)(255 * (1 - fraction)); // Red
                    pixels[index++] = 255;                          // Alpha
                }

            using (Stream pixelStream = bitmap.PixelBuffer.AsStream())
            {
                await pixelStream.WriteAsync(pixels, 0, pixels.Length);
            }
            bitmap.Invalidate();
            imRect.ImageSource = bitmap;
        }

        async private void AppBarButton_Click_4(object sender, RoutedEventArgs e)
        {
            //Uri uri = new Uri("http://www.charlespetzold.com/pw6/PetzoldJersey.jpg");
            Uri uri = new Uri("ms-appx:///PetzoldJersey.jpg");
            RandomAccessStreamReference streamRef = RandomAccessStreamReference.CreateFromUri(uri);

            // Create a buffer for reading the stream
            Windows.Storage.Streams.Buffer buffer = null;

            // Read the entire file
            using (IRandomAccessStreamWithContentType fileStream = await streamRef.OpenReadAsync())
            {
                buffer = new Windows.Storage.Streams.Buffer((uint)fileStream.Size);
                await fileStream.ReadAsync(buffer, (uint)fileStream.Size, InputStreamOptions.None);
            }

            // Create WriteableBitmap with unknown size
            WriteableBitmap bitmap = new WriteableBitmap(1, 1);

            // Create a memory stream for transferring the data
            using (InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream())
            {
                await memoryStream.WriteAsync(buffer);
                memoryStream.Seek(0);

                // Use the memory stream as the Bitmap source
                bitmap.SetSource(memoryStream);
            }

            // Now get the pixels from the bitmap
            byte[] pixels = new byte[4 * bitmap.PixelWidth * bitmap.PixelHeight];
            int index = 0;

            using (Stream pixelStream = bitmap.PixelBuffer.AsStream())
            {
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                // Apply opacity to the pixels
                for (int y = 0; y < bitmap.PixelHeight; y++)
                {
                    double opacity = (double)y / bitmap.PixelHeight;

                    for (int x = 0; x < bitmap.PixelWidth; x++)
                        for (int i = 0; i < 4; i++)
                        {
                            pixels[index] = (byte)(opacity * pixels[index]);
                            index++;
                        }
                }

                // Put the pixels back in the bitmap
                pixelStream.Seek(0, SeekOrigin.Begin);
                await pixelStream.WriteAsync(pixels, 0, pixels.Length);
            }
            bitmap.Invalidate();
            reflectedImage.Source = bitmap;
        }
    }
}
