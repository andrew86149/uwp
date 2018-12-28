﻿using System;
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

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace SpecApp
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        async private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            object dataContext = (e.OriginalSource as FrameworkElement).DataContext;
            string str = dataContext.ToString();

            switch (str)
            {
                case "WhatSize":
                    {
                        this.Frame.Navigate(typeof(Page0));
                        break;
                    }
                case "Finger123":
                    {
                        this.Frame.Navigate(typeof(Page1));
                        break;
                    }
                case "PointerLog":
                    {
                        this.Frame.Navigate(typeof(Page2));
                        break;
                    }
                case "Finger45":
                    {
                        this.Frame.Navigate(typeof(Page3));
                        break;
                    }
                case "Whirligig":
                    {
                        this.Frame.Navigate(typeof(Page4));
                        break;
                    }
                case "Piano":
                    {
                        this.Frame.Navigate(typeof(Page5));
                        break;
                    }
                case "ManipulTracker":
                    {
                        this.Frame.Navigate(typeof(Page6));
                        break;
                    }
                case "FlickAndBounce":
                    {
                        this.Frame.Navigate(typeof(Page7));
                        break;
                    }
                case "XYSlider":
                    {
                        this.Frame.Navigate(typeof(Page8));
                        break;
                    }
                case "CenteredTransforms":
                    {
                        this.Frame.Navigate(typeof(Page9));
                        break;
                    }
                case "DialSketch":
                    {
                        this.Frame.Navigate(typeof(Page10));
                        break;
                    }
                case "test11":
                    {
                        this.Frame.Navigate(typeof(Page11));
                        break;
                    }
                case "test12":
                    {
                        this.Frame.Navigate(typeof(Page12));
                        break;
                    }
                default:
                    {
                        ContentDialog1 dialog1 = new ContentDialog1();
                        dialog1.PrimaryButtonText = str;
                        await dialog1.ShowAsync();
                        break;
                    }
            }
        }

        async private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog1 dialog1 = new ContentDialog1();
            dialog1.Title = "Help";
            await dialog1.ShowAsync();
        }
    }
}
