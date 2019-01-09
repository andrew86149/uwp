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
    public sealed partial class Page25 : Page
    {
        public Page25()
        {
            this.InitializeComponent();
            pname.Text = "Page 25";
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void AppBarButton2_Click(object sender, RoutedEventArgs e)
        {
            string pname = "4PMMc6Nwuk4";
            this.Frame.Navigate(typeof(PageVideo), pname);
        }

        async private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            object dataContext = (e.OriginalSource as FrameworkElement).DataContext;
            string st = dataContext.ToString();
            switch (st)
            {
                case "video1":
                    {
                        string str = "mnjbHvYhl-w";
                        this.Frame.Navigate(typeof(PageVideo), str);
                        break;
                    }
                default:
                    {
                        ContentDialog1 dialog1 = new ContentDialog1
                        {
                            PrimaryButtonText = st
                        };
                        await dialog1.ShowAsync();
                        break;
                    }
            }
        }
    }
}
