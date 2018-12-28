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
    public sealed partial class Octave : UserControl
    {
        static readonly DependencyProperty lastKeyVisibleProperty =
                DependencyProperty.Register("LastKeyVisible",
                        typeof(bool), typeof(Octave),
                        new PropertyMetadata(false, OnLastKeyVisibleChanged));
        public Octave()
        {
            this.InitializeComponent();
        }

        public static DependencyProperty LastKeyVisibleProperty
        {
            get { return lastKeyVisibleProperty; }
        }

        public bool LastKeyVisible
        {
            set { SetValue(LastKeyVisibleProperty, value); }
            get { return (bool)GetValue(LastKeyVisibleProperty); }
        }

        static void OnLastKeyVisibleChanged(DependencyObject obj,
                                            DependencyPropertyChangedEventArgs args)
        {
            (obj as Octave).lastKey.Visibility =
                (bool)args.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
