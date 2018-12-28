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
    public sealed partial class Page9 : Page
    {
        bool flag = false;
        ManipulationManager manipulationManager = new ManipulationManager();

        public Page9()
        {
            this.InitializeComponent();
            pname.Text = "Page 9";

            image.ManipulationMode = ManipulationModes.All &
                                     ~ManipulationModes.TranslateRailsX &
                                     ~ManipulationModes.TranslateRailsY;
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        protected override void OnManipulationStarting(ManipulationStartingRoutedEventArgs e)
        {
            if (flag)
            {
                e.Pivot = new ManipulationPivot(new Point(image.ActualWidth / 2, image.ActualHeight / 2), 50);
            }
            base.OnManipulationStarting(e);
        }
        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs args)
        {
            manipulationManager.AccumulateDelta(args.Position, args.Delta);
            matrixXform.Matrix = manipulationManager.Matrix;
            // Make this the entire transform to date
            //matrixXform.Matrix = xformGroup.Value;

            // Use that to transform the Position property
            //Point center = matrixXform.TransformPoint(args.Position);

            // That becomes the center of the new incremental transform
            //compositeXform.CenterX = center.X;
            //compositeXform.CenterY = center.Y;

            // Set the other properties
            //compositeXform.TranslateX = args.Delta.Translation.X;
            //compositeXform.TranslateY = args.Delta.Translation.Y;
            //compositeXform.ScaleX = args.Delta.Scale;
            //compositeXform.ScaleY = args.Delta.Scale;
            //compositeXform.Rotation = args.Delta.Rotation;

            base.OnManipulationDelta(args);
        }

        private void AlignCenter_Click(object sender, RoutedEventArgs e)
        {
            flag = true;
        }
    }
}
