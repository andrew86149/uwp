using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpecApp
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class Page1 : Page
    {
        Dictionary<uint, Polyline> pointerDictionary = new Dictionary<uint, Polyline>();
        Random rand = new Random();
        byte[] rgb = new byte[3];

        public Page1()
        {
            this.InitializeComponent();
            pname.Text = "Page 1";
            this.IsTabStop = true;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            uint id = e.Pointer.PointerId;
            Point point = e.GetCurrentPoint(this).Position;
            rand.NextBytes(rgb);
            Color color = Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);
            Polyline polyline = new Polyline
            {
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 24,
            };

            polyline.PointerPressed += OnPolylinePointerPressed;
            polyline.RightTapped += OnPolylineRightTapped;

            polyline.Points.Add(point);

            contentGrid.Children.Add(polyline);
            pointerDictionary.Add(id, polyline);

            CapturePointer(e.Pointer);
            Focus(FocusState.Programmatic);
            base.OnPointerPressed(e);
        }

        async private void OnPolylineRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            Polyline polyline = sender as Polyline;
            PopupMenu popupMenu = new PopupMenu();
            popupMenu.Commands.Add(new UICommand("Change color", OnMenuChangeColor, polyline));
            popupMenu.Commands.Add(new UICommand("Delete", OnMenuDelete, polyline));
            await popupMenu.ShowAsync(e.GetPosition(this));
            //throw new NotImplementedException();
        }

        private void OnMenuDelete(IUICommand command)
        {
            Polyline polyline = command.Id as Polyline;
            contentGrid.Children.Remove(polyline);
            //throw new NotImplementedException();
        }

        private void OnMenuChangeColor(IUICommand command)
        {
            Polyline polyline = command.Id as Polyline;
            rand.NextBytes(rgb);
            Color color = Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);
            (polyline.Stroke as SolidColorBrush).Color = color;
            //throw new NotImplementedException();
        }

        private void OnPolylinePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            //throw new NotImplementedException();
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            uint id = e.Pointer.PointerId;
            Point point = e.GetCurrentPoint(this).Position;
            if (pointerDictionary.ContainsKey(id))
                pointerDictionary[id].Points.Add(point);
            base.OnPointerMoved(e);
        }
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            uint id = e.Pointer.PointerId;
            if (pointerDictionary.ContainsKey(id))
                pointerDictionary.Remove(id);
            base.OnPointerReleased(e);
        }
        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            uint id = e.Pointer.PointerId;
            if (pointerDictionary.ContainsKey(id))
            {
                contentGrid.Children.Remove(pointerDictionary[id]);
                pointerDictionary.Remove(id);
            }
            base.OnPointerCaptureLost(e);
        }
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape) ReleasePointerCaptures();
            base.OnKeyDown(e);
        }
    }
}
