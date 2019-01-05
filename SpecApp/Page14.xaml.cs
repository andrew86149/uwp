using PetzoldVectorDrawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
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
    public sealed partial class Page14 : Page
    {
        AppSettings appSettings = new AppSettings();
        double imageScale = 1;
        Point imageOffset = new Point();

        struct PointerInfo
        {
            public Brush Brush;
            public Point PreviousPoint;
            public double PreviousRadius;
        }

        Dictionary<uint, PointerInfo> pointerDictionary = new Dictionary<uint, PointerInfo>();
        List<double> xCollection = new List<double>();

        WriteableBitmap bitmap;
        Stream pixelStream;
        byte[] pixels;

        public Page14()
        {
            this.InitializeComponent();
            pname.Text = "Page 14";

            SizeChanged += OnMainPageSizeChanged;
            Loaded += OnMainPageLoaded;
            Application.Current.Suspending += OnApplicationSuspending;
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        void OnMainPageSizeChanged(object sender, SizeChangedEventArgs args)
        {
            VisualStateManager.GoToState(this, ApplicationView.Value.ToString(), true);

            if (bitmap != null)
            {
                CalculateImageScaleAndOffset();
            }
        }

        void CalculateImageScaleAndOffset()
        {
            imageScale = Math.Min(this.ActualWidth / bitmap.PixelWidth,
                                  this.ActualHeight / bitmap.PixelHeight);
            imageOffset = new Point((this.ActualWidth - imageScale * bitmap.PixelWidth) / 2,
                                    (this.ActualHeight - imageScale * bitmap.PixelHeight) / 2);
        }

        async void OnMainPageLoaded(object sender, RoutedEventArgs args)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile storageFile = await localFolder.GetFileAsync("FingerPaint.png");
                await LoadBitmapFromFile(storageFile);
            }
            catch
            {
                // Ignore any errors
            }

            if (bitmap == null)
                await CreateNewBitmapAndPixelArray();
        }

        async void OnApplicationSuspending(object sender, SuspendingEventArgs args)
        {
            // Save application settings
            appSettings.Save();

            // Save current bitmap
            SuspendingDeferral deferral = args.SuspendingOperation.GetDeferral();

            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile storageFile = await localFolder.CreateFileAsync("FingerPaint.png",
                                                        CreationCollisionOption.ReplaceExisting);
                await SaveBitmapToFile(storageFile);
            }
            catch
            {
                // Ignore any errors
            }

            deferral.Complete();
        }

        void OnColorAppBarButtonClick(object sender, RoutedEventArgs args)
        {
            DisplayDialog(sender, new ColorSettingDialog());
        }

        void OnThicknessAppBarButtonClick(object sender, RoutedEventArgs args)
        {
            DisplayDialog(sender, new ThicknessSettingDialog());
        }

        void DisplayDialog(object sender, FrameworkElement dialog)
        {
            dialog.DataContext = appSettings;

            Popup popup = new Popup
            {
                Child = dialog,
                IsLightDismissEnabled = true
            };

            dialog.SizeChanged += (dialogSender, dialogArgs) =>
            {
                // Get Button location relative to screen
                Button btn = sender as Button;
                Point pt = btn.TransformToVisual(null).TransformPoint(
                                                            new Point(btn.ActualWidth / 2,
                                                                      btn.ActualHeight / 2));

                popup.HorizontalOffset = Math.Max(24, pt.X - dialog.ActualWidth / 2);

                if (popup.HorizontalOffset + dialog.ActualWidth > this.ActualWidth)
                    popup.HorizontalOffset = this.ActualWidth - dialog.ActualWidth;

                popup.HorizontalOffset = Math.Max(0, popup.HorizontalOffset);

                popup.VerticalOffset = this.ActualHeight - dialog.ActualHeight
                                                         - this.BottomAppBar.ActualHeight - 24;
            };

            popup.Closed += (popupSender, popupArgs) =>
            {
                this.BottomAppBar.IsOpen = false;
            };

            popup.IsOpen = true;
        }

        // Pointer ------------------------------------------------------------
        protected override void OnPointerPressed(PointerRoutedEventArgs args)
        {
            // Get information from event arguments
            uint id = args.Pointer.PointerId;
            PointerPoint pointerPoint = args.GetCurrentPoint(this);

            // Create PointerInfo
            PointerInfo pointerInfo = new PointerInfo
            {
                PreviousPoint = pointerPoint.Position,
                PreviousRadius = appSettings.Thickness * pointerPoint.Properties.Pressure,
                Brush = new SolidColorBrush(appSettings.Color)
            };

            // Add to dictionary
            pointerDictionary.Add(id, pointerInfo);

            // Capture the Pointer
            CapturePointer(args.Pointer);

            base.OnPointerPressed(args);
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs args)
        {
            // Get ID from event arguments
            uint id = args.Pointer.PointerId;

            // If ID is in dictionary, start a loop
            if (pointerDictionary.ContainsKey(id))
            {
                PointerInfo pointerInfo = pointerDictionary[id];

                foreach (PointerPoint pointerPoint in args.GetIntermediatePoints(this).Reverse())
                {
                    // For each point, get new position and pressure
                    Point point = pointerPoint.Position;
                    double radius = appSettings.Thickness * pointerPoint.Properties.Pressure;

                    // Render and flag that it's modified
                    appSettings.IsImageModified =
                        RenderOnBitmap(pointerInfo.PreviousPoint, pointerInfo.PreviousRadius,
                                       point, radius,
                                       appSettings.Color);

                    // Update PointerInfo
                    pointerInfo.PreviousPoint = point;
                    pointerInfo.PreviousRadius = radius;
                }

                // Store PointerInfo back in dictionary
                pointerDictionary[id] = pointerInfo;
            }
            base.OnPointerMoved(args);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs args)
        {
            // Get information from event arguments
            uint id = args.Pointer.PointerId;

            // If ID is in dictionary, remove it
            if (pointerDictionary.ContainsKey(id))
                pointerDictionary.Remove(id);

            base.OnPointerReleased(args);
        }

        protected override void OnPointerCaptureLost(PointerRoutedEventArgs args)
        {
            // Get information from event arguments
            uint id = args.Pointer.PointerId;

            // If ID is still in dictionary, remove it
            if (pointerDictionary.ContainsKey(id))
                pointerDictionary.Remove(id);

            base.OnPointerCaptureLost(args);
        }

        bool RenderOnBitmap(Point point1, double radius1, Point point2, double radius2, Color color)
        {
            bool bitmapNeedsUpdate = false;

            // Define a line between the two points with rounded caps
            IGeometrySegment geoseg = null;

            // Adjust the points for any bitmap scaling
            Point center1 = ScaleToBitmap(point1);
            Point center2 = ScaleToBitmap(point2);

            // Find the distance between them
            double distance = Math.Sqrt(Math.Pow(center2.X - center1.X, 2) +
                                        Math.Pow(center2.Y - center1.Y, 2));

            // Choose the proper way to render the segment
            if (radius1 == radius2)
                geoseg = new RoundCappedLine(center1, center2, radius1);

            else if (radius1 < radius2 && radius1 + distance < radius2)
                geoseg = new RoundCappedLine(center1, center2, radius2);

            else if (radius2 < radius1 && radius2 + distance < radius1)
                geoseg = new RoundCappedLine(center1, center2, radius1);

            else if (radius1 < radius2)
                geoseg = new RoundCappedPath(center1, center2, radius1, radius2);

            else
                geoseg = new RoundCappedPath(center2, center1, radius2, radius1);

            // Find the minimum and maximum vertical coordinates
            int yMin = (int)Math.Min(center1.Y - radius1, center2.Y - radius2);
            int yMax = (int)Math.Max(center1.Y + radius1, center2.Y + radius2);

            yMin = Math.Max(0, Math.Min(bitmap.PixelHeight, yMin));
            yMax = Math.Max(0, Math.Min(bitmap.PixelHeight, yMax));

            // Loop through all the y coordinates that contain part of the segment
            for (int y = yMin; y < yMax; y++)
            {
                // Get the range of x coordinates in the segment
                xCollection.Clear();
                geoseg.GetAllX(y, xCollection);

                if (xCollection.Count == 2)
                {
                    // Find the minimum and maximum horizontal coordinates
                    int xMin = (int)(Math.Min(xCollection[0], xCollection[1]) + 0.5f);
                    int xMax = (int)(Math.Max(xCollection[0], xCollection[1]) + 0.5f);

                    xMin = Math.Max(0, Math.Min(bitmap.PixelWidth, xMin));
                    xMax = Math.Max(0, Math.Min(bitmap.PixelWidth, xMax));

                    // Loop through the X values
                    for (int x = xMin; x < xMax; x++)
                    {
                        {
                            // Set the pixel
                            int index = 4 * (y * bitmap.PixelWidth + x);
                            pixels[index + 0] = color.B;
                            pixels[index + 1] = color.G;
                            pixels[index + 2] = color.R;
                            pixels[index + 3] = 255;

                            bitmapNeedsUpdate = true;
                        }
                    }
                }
            }
            // Update bitmap
            if (bitmapNeedsUpdate)
            {
                // Find the starting index and number of pixels
                int start = 4 * yMin * bitmap.PixelWidth;
                int count = 4 * (yMax - yMin) * bitmap.PixelWidth;

                pixelStream.Seek(start, SeekOrigin.Begin);
                pixelStream.Write(pixels, start, count);
                bitmap.Invalidate();
            }

            return bitmapNeedsUpdate;
        }

        Point ScaleToBitmap(Point pt)
        {
            return new Point((pt.X - imageOffset.X) / imageScale,
                             (pt.Y - imageOffset.Y) / imageScale);
        }

        // File ======================================================================================

        async Task CreateNewBitmapAndPixelArray()
        {
            bitmap = new WriteableBitmap((int)this.ActualWidth, (int)this.ActualHeight);
            pixels = new byte[4 * bitmap.PixelWidth * bitmap.PixelHeight];

            // Set whole bitmap to white
            for (int index = 0; index < pixels.Length; index++)
                pixels[index] = 0xFF;

            await InitializeBitmap();

            appSettings.LoadedFilePath = null;
            appSettings.LoadedFilename = null;
            appSettings.IsImageModified = false;
        }

        async Task LoadBitmapFromFile(StorageFile storageFile)
        {
            using (IRandomAccessStreamWithContentType stream = await storageFile.OpenReadAsync())
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                BitmapFrame bitmapframe = await decoder.GetFrameAsync(0);
                PixelDataProvider dataProvider =
                    await bitmapframe.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                                                        BitmapAlphaMode.Premultiplied,
                                                        new BitmapTransform(),
                                                        ExifOrientationMode.RespectExifOrientation,
                                                        ColorManagementMode.ColorManageToSRgb);
                pixels = dataProvider.DetachPixelData();
                bitmap = new WriteableBitmap((int)bitmapframe.PixelWidth,
                                             (int)bitmapframe.PixelHeight);
                await InitializeBitmap();
            }
        }

        async Task InitializeBitmap()
        {
            pixelStream = bitmap.PixelBuffer.AsStream();
            await pixelStream.WriteAsync(pixels, 0, pixels.Length);
            bitmap.Invalidate();
            image.Source = bitmap;
            CalculateImageScaleAndOffset();
        }

        async void OnAddAppBarButtonClick(object sender, RoutedEventArgs args)
        {
            Button button = sender as Button;
            button.IsEnabled = false;

            await CheckIfOkToTrashFile(CreateNewBitmapAndPixelArray);

            button.IsEnabled = true;
            this.BottomAppBar.IsOpen = false;
        }

        async void OnOpenAppBarButtonClick(object sender, RoutedEventArgs args)
        {
            Button button = sender as Button;
            button.IsEnabled = false;

            await CheckIfOkToTrashFile(LoadFileFromOpenPicker);

            button.IsEnabled = true;
            this.BottomAppBar.IsOpen = false;
        }

        async Task CheckIfOkToTrashFile(Func<Task> commandAction)
        {
            if (!appSettings.IsImageModified)
            {
                await commandAction();
                return;
            }

            string message =
                String.Format("Do you want to save changes to {0}?",
                              appSettings.LoadedFilePath ?? "(untitled)");

            MessageDialog msgdlg = new MessageDialog(message, "Finger Paint");
            msgdlg.Commands.Add(new UICommand("Save", null, "save"));
            msgdlg.Commands.Add(new UICommand("Don't Save", null, "dont"));
            msgdlg.Commands.Add(new UICommand("Cancel", null, "cancel"));
            msgdlg.DefaultCommandIndex = 0;
            msgdlg.CancelCommandIndex = 2;
            IUICommand command = await msgdlg.ShowAsync();

            if ((string)command.Id == "cancel")
                return;

            if ((string)command.Id == "dont")
            {
                await commandAction();
                return;
            }

            if (appSettings.LoadedFilePath == null)
            {
                StorageFile storageFile = await GetFileFromSavePicker();

                if (storageFile == null)
                    return;

                appSettings.LoadedFilePath = storageFile.Path;
                appSettings.LoadedFilename = storageFile.Name;
            }

            string exception = null;

            try
            {
                await SaveBitmapToFile(appSettings.LoadedFilePath);
            }
            catch (Exception exc)
            {
                exception = exc.Message;
            }

            if (exception != null)
            {
                msgdlg = new MessageDialog("The image file could not be saved. " +
                                           "The system reports an error of: " + exception,
                                           "Finger Paint");
                await msgdlg.ShowAsync();
                return;
            }

            await commandAction();
        }

        async Task LoadFileFromOpenPicker()
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

            string exception = null;

            try
            {
                await LoadBitmapFromFile(storageFile);
            }
            catch (Exception exc)
            {
                exception = exc.Message;
            }

            if (exception != null)
            {
                MessageDialog msgdlg =
                    new MessageDialog("The image file could not be loaded. " +
                                      "The system reports an error of: " + exception,
                                      "Finger Paint");
                await msgdlg.ShowAsync();
                return;
            }

            appSettings.LoadedFilePath = storageFile.Path;
            appSettings.LoadedFilename = storageFile.Name;
            appSettings.IsImageModified = false;
        }

        async void OnSaveAppBarButtonClick(object sender, RoutedEventArgs args)
        {
            Button button = sender as Button;
            button.IsEnabled = false;

            if (appSettings.LoadedFilePath != null)
            {
                await SaveWithErrorNotification(appSettings.LoadedFilePath);
            }
            else
            {
                StorageFile storageFile = await GetFileFromSavePicker();

                if (storageFile == null)
                    return;

                await SaveWithErrorNotification(storageFile);
            }

            button.IsEnabled = true;
        }

        async void OnSaveAsAppBarButtonClick(object sender, RoutedEventArgs args)
        {
            StorageFile storageFile = await GetFileFromSavePicker();

            if (storageFile == null)
                return;

            await SaveWithErrorNotification(storageFile);
        }

        async Task<StorageFile> GetFileFromSavePicker()
        {
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.SuggestedFileName = appSettings.LoadedFilename ?? "MyFingerPainting";

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
            return await picker.PickSaveFileAsync();
        }

        async Task<bool> SaveWithErrorNotification(string filename)
        {
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filename);
            return await SaveWithErrorNotification(storageFile);
        }

        async Task<bool> SaveWithErrorNotification(StorageFile storageFile)
        {
            string exception = null;

            try
            {
                await SaveBitmapToFile(storageFile);
            }
            catch (Exception exc)
            {
                exception = exc.Message;
            }

            if (exception != null)
            {
                MessageDialog msgdlg =
                    new MessageDialog("The image file could not be saved. " +
                                      "The system reports an error of: " + exception,
                                      "Finger Paint");
                await msgdlg.ShowAsync();
                return false;
            }

            appSettings.LoadedFilePath = storageFile.Path;
            appSettings.IsImageModified = false;
            return true;
        }

        async Task SaveBitmapToFile(string filename)
        {
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filename);
            await SaveBitmapToFile(storageFile);
        }

        async Task SaveBitmapToFile(StorageFile storageFile)
        {
            using (IRandomAccessStream fileStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied,
                                     (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight,
                                     96, 96, pixels);
                await encoder.FlushAsync();
            }
        }
    }
}
