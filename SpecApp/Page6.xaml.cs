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
using Windows.UI.Xaml.Shapes;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpecApp
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class Page6 : Page
    {
        public Page6()
        {
            this.InitializeComponent();
            pname.Text = "Page 6";
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        void OnManipulationModeCheckBoxChecked(object sender, RoutedEventArgs args)
        {
            // Get composite ManipulationModes value of checked CheckBoxes
            ManipulationModes manipulationModes = ManipulationModes.None;

            foreach (UIElement child in checkBoxPanel.Children)
            {
                ManipulationModeCheckBox checkBox = child as ManipulationModeCheckBox;

                if ((bool)checkBox.IsChecked)
                    manipulationModes |= checkBox.ManipulationModes;
            }

            // Set ManipulationMode property of each Rectangle
            foreach (UIElement child in rectanglePanel.Children)
                child.ManipulationMode = manipulationModes;
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs args)
        {
            // OriginalSource is always Rectangle because nothing else has its
            //     ManipulationMode set to anything other than ManipulationModes.None
            Rectangle rectangle = args.OriginalSource as Rectangle;
            CompositeTransform transform = rectangle.RenderTransform as CompositeTransform;

            transform.TranslateX += args.Delta.Translation.X;
            transform.TranslateY += args.Delta.Translation.Y;

            transform.ScaleX *= args.Delta.Scale;
            transform.ScaleY *= args.Delta.Scale;

            transform.Rotation += args.Delta.Rotation;

            base.OnManipulationDelta(args);
        }
    }
}
