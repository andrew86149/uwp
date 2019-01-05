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

// Документацию по шаблону элемента "Пользовательский элемент управления" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234236

namespace SpecApp
{
    public sealed partial class ManipulableContentControl : ContentControl
    {
        static int zIndex;
        ManipulationManager manipulationManager;

        public ManipulableContentControl(CompositeTransform initialTransform)
        {
            this.InitializeComponent();

            // Create the ManipulationManager and set MatrixTransform from it
            manipulationManager = new ManipulationManager(initialTransform);
            matrixXform.Matrix = manipulationManager.Matrix;

            this.ManipulationMode = ManipulationModes.All &
                                   ~ManipulationModes.TranslateRailsX &
                                   ~ManipulationModes.TranslateRailsY;
        }

        protected override void OnManipulationStarting(ManipulationStartingRoutedEventArgs args)
        {
            Canvas.SetZIndex(this, zIndex += 1);
            base.OnManipulationStarting(args);
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs args)
        {
            manipulationManager.AccumulateDelta(args.Position, args.Delta);
            matrixXform.Matrix = manipulationManager.Matrix;
            base.OnManipulationDelta(args);
        }
    }
}
