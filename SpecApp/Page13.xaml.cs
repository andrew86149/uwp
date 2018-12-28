using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
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
    public sealed partial class Page13 : Page
    {
        double dpiX, dpiY;

        WriteableBitmap bitmap;
        Stream pixelStream;
        byte[] srcPixels;
        byte[] dstPixels;

        // Byte masks for blue, green, red
        byte[] masks = { 0xFF, 0xFF, 0xFF };

        public Page13()
        {
            this.InitializeComponent();
            pname.Text = "Page 13";

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            string[] prefix = { "r", "g", "b", "a" };

            for (int col = 0; col < 4; col++)
                for (int row = 1; row < 9; row++)
                {
                    RadioButton radio = new RadioButton
                    {
                        Content = row.ToString(),
                        Margin = new Thickness(12, 6, 12, 6),
                        GroupName = prefix[col],
                        Tag = prefix[col] + row,
                        IsChecked = row == 8
                    };
                    radio.Checked += OnRadioButtonChecked;

                    Grid.SetColumn(radio, col);
                    Grid.SetRow(radio, row);
                    controlPanelGrid.Children.Add(radio);
                }
            //throw new NotImplementedException();
        }

        private void OnRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            // Decode the RadioButton Tag property
            RadioButton radio = sender as RadioButton;
            string tag = radio.Tag as string;
            int maskIndex = -1;
            int bits = Int32.Parse(tag[1].ToString()); // 1 to 8
            byte mask = (byte)(0xFF << 8 - bits);
            bool needsUpdate;

            // Find the index of the masks array
            switch (tag[0])
            {
                case 'r': maskIndex = 2; break;
                case 'g': maskIndex = 1; break;
                case 'b': maskIndex = 0; break;
            }

            // For "All", check all the other buttons in the row
            if (tag[0] == 'a')
            {
                needsUpdate = masks[0] != mask && masks[1] != mask && masks[2] != mask;

                if (needsUpdate)
                    masks[0] = masks[1] = masks[2] = mask;

                foreach (UIElement child in (radio.Parent as Panel).Children)
                {
                    if (child != radio &&
                        Grid.GetRow(child as FrameworkElement) == Grid.GetRow(radio))
                    {
                        (child as RadioButton).IsChecked = true;
                    }
                }
            }
            else
            {
                needsUpdate = masks[maskIndex] != mask;

                if (needsUpdate)
                    masks[maskIndex] = mask;
            }

            if (needsUpdate)
                UpdateBitmap();
            //throw new NotImplementedException();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void OnRotateLeftAppBarButtonClick(object sender, RoutedEventArgs e)
        {
            Rotate((BitmapSource bitmap, int x, int y) =>
            {
                return 4 * (bitmap.PixelWidth * x + (bitmap.PixelWidth - y - 1));
            });
        }

        private void OnRotateRightAppBarButtonClick(object sender, RoutedEventArgs e)
        {
            Rotate((BitmapSource bitmap, int x, int y) =>
            {
                return 4 * (bitmap.PixelWidth * (bitmap.PixelHeight - x - 1) + y);
            });
        }

        async private void OnOpenAppBarButtonClick(object sender, RoutedEventArgs args)
        {
            // Create FileOpenPicker
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            // Initialize with filename extensions
            IReadOnlyList<BitmapCodecInformation> codecInfos =
                                    BitmapDecoder.GetDecoderInformationEnumerator();

            foreach (BitmapCodecInformation codecInfo in codecInfos)
                foreach (string extension in codecInfo.FileExtensions)
                    picker.FileTypeFilter.Add(extension);

            // Get the selected file
            StorageFile storageFile = await picker.PickSingleFileAsync();

            if (storageFile == null)
                return;

            // Open the stream and create a decoder
            BitmapDecoder decoder = null;

            using (IRandomAccessStreamWithContentType stream = await storageFile.OpenReadAsync())
            {
                string exception = null;

                try
                {
                    decoder = await BitmapDecoder.CreateAsync(stream);
                }
                catch (Exception exc)
                {
                    exception = exc.Message;
                }

                if (exception != null)
                {
                    MessageDialog msgdlg =
                        new MessageDialog("That particular image file could not be loaded. " +
                                          "The system reports on error of: " + exception);
                    await msgdlg.ShowAsync();
                    return;
                }

                // Get the first frame
                BitmapFrame bitmapFrame = await decoder.GetFrameAsync(0);

                // Set information title
                txtblk.Text = String.Format("{0}: {1} x {2} {3} {4} x {5} DPI",
                                            storageFile.Name,
                                            bitmapFrame.PixelWidth, bitmapFrame.PixelHeight,
                                            bitmapFrame.BitmapPixelFormat,
                                            bitmapFrame.DpiX, bitmapFrame.DpiY);
                // Save the resolution
                dpiX = bitmapFrame.DpiX;
                dpiY = bitmapFrame.DpiY;

                // Get the pixels
                PixelDataProvider dataProvider =
                    await bitmapFrame.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                                                        BitmapAlphaMode.Premultiplied,
                                                        new BitmapTransform(),
                                                        ExifOrientationMode.RespectExifOrientation,
                                                        ColorManagementMode.ColorManageToSRgb);

                //byte[] pixels = dataProvider.DetachPixelData();
                srcPixels = dataProvider.DetachPixelData();
                //srcPixels = new byte[pixels.Length];
                dstPixels = new byte[srcPixels.Length];

                // Create WriteableBitmap and set the pixels
                //WriteableBitmap 
                bitmap = new WriteableBitmap((int)bitmapFrame.PixelWidth, (int)bitmapFrame.PixelHeight);
                pixelStream = bitmap.PixelBuffer.AsStream();
                //using (Stream pixelStream = bitmap.PixelBuffer.AsStream())
                //{
                //    await pixelStream.WriteAsync(pixels, 0, pixels.Length);
                //}

                // Invalidate the WriteableBitmap and set as Image source
                //bitmap.Invalidate();
                image.Source = bitmap;
                UpdateBitmap();
            }

            // Enable the other buttons
            saveAsButton.IsEnabled = true;
            rotateLeftButton.IsEnabled = true;
            rotateRightButton.IsEnabled = true;
        }

        async private void OnSaveAsAppBarButtonClick(object sender, RoutedEventArgs args)
        {
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            // Get the encoder information
            Dictionary<string, Guid> imageTypes = new Dictionary<string, Guid>();
            IReadOnlyList<BitmapCodecInformation> codecInfos =
                                    BitmapEncoder.GetEncoderInformationEnumerator();

            foreach (BitmapCodecInformation codecInfo in codecInfos)
            {
                List<string> extensions = new List<string>();

                foreach (string extension in codecInfo.FileExtensions)
                    extensions.Add(extension);

                string filetype = codecInfo.FriendlyName.Split(' ')[0];
                picker.FileTypeChoices.Add(filetype, extensions);

                foreach (string mimeType in codecInfo.MimeTypes)
                    imageTypes.Add(mimeType, codecInfo.CodecId);
            }

            // Get a selected StorageFile
            StorageFile storageFile = await picker.PickSaveFileAsync();

            if (storageFile == null)
                return;

            // Open the StorageFile
            using (IRandomAccessStream fileStream =
                                    await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                // Create an encoder
                Guid codecId = imageTypes[storageFile.ContentType];
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(codecId, fileStream);

                // Get the pixels from the existing WriteableBitmap
                WriteableBitmap bitmap = image.Source as WriteableBitmap;
                byte[] pixels = new byte[4 * bitmap.PixelWidth * bitmap.PixelHeight];

                using (Stream pixelStream = bitmap.PixelBuffer.AsStream())
                {
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                }

                // Write those pixels to the first frame
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied,
                                     (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight,
                                     dpiX, dpiY, pixels);

                await encoder.FlushAsync();
            }
        }

        private void OnCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            UpdateBitmap();
        }

        void UpdateBitmap()
        {
            if (bitmap == null)
                return;

            for (int index = 0; index < srcPixels.Length; index += 4)
            {
                // Mask source pixels
                byte B = (byte)(masks[0] & srcPixels[index + 0]);
                byte G = (byte)(masks[1] & srcPixels[index + 1]);
                byte R = (byte)(masks[2] & srcPixels[index + 2]);
                byte A = srcPixels[index + 3];

                // Possibly convert to gray shade
                if (monochromeCheckBox.IsChecked.Value)
                    B = G = R = (byte)(0.30 * R + 0.59 * G + 0.11 * B);

                // Save destination pixels
                dstPixels[index + 0] = B;
                dstPixels[index + 1] = G;
                dstPixels[index + 2] = R;
                dstPixels[index + 3] = A;
            }

            // Update bitmap
            pixelStream.Seek(0, SeekOrigin.Begin);
            pixelStream.Write(dstPixels, 0, dstPixels.Length);
            bitmap.Invalidate();
        }

        async void Rotate(Func<BitmapSource, int, int, int> calculateSourceIndex)
        {
            // Get the source bitmap pixels
            WriteableBitmap srcBitmap = image.Source as WriteableBitmap;
            byte[] srcPixels = new byte[4 * srcBitmap.PixelWidth * srcBitmap.PixelHeight];

            using (Stream pixelStream = srcBitmap.PixelBuffer.AsStream())
            {
                await pixelStream.ReadAsync(srcPixels, 0, srcPixels.Length);
            }

            // Create a destination bitmap and pixels array
            WriteableBitmap dstBitmap =
                    new WriteableBitmap(srcBitmap.PixelHeight, srcBitmap.PixelWidth);
            byte[] dstPixels = new byte[4 * dstBitmap.PixelWidth * dstBitmap.PixelHeight];

            // Transfer the pixels
            int dstIndex = 0;
            for (int y = 0; y < dstBitmap.PixelHeight; y++)
                for (int x = 0; x < dstBitmap.PixelWidth; x++)
                {
                    int srcIndex = calculateSourceIndex(srcBitmap, x, y);

                    for (int i = 0; i < 4; i++)
                        dstPixels[dstIndex++] = srcPixels[srcIndex++];
                }

            // Move the pixels into the destination bitmap
            using (Stream pixelStream = dstBitmap.PixelBuffer.AsStream())
            {
                await pixelStream.WriteAsync(dstPixels, 0, dstPixels.Length);
            }
            dstBitmap.Invalidate();

            // Swap the DPIs
            double dpi = dpiX;
            dpiX = dpiY;
            dpiY = dpi;

            // Display the new bitmap
            image.Source = dstBitmap;
        }
    }
}
