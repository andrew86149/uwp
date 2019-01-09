﻿using PetzoldVectorDrawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Popups;
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
    public sealed partial class Page20 : Page
    {
        const double GRAVITY = 5000;    // pixels per second squared
        const double BALL_RADIUS = 32;

        Accelerometer accelerometer = Accelerometer.GetDefault();
        TimeSpan timeSpan;
        Vector2 acceleration;
        Vector2 ballPosition;
        Vector2 ballVelocity;

        public Page20()
        {
            this.InitializeComponent();
            pname.Text = "Page 20";

            DisplayProperties.AutoRotationPreferences = DisplayProperties.NativeOrientation;

            ball.RadiusX = BALL_RADIUS;
            ball.RadiusY = BALL_RADIUS;

            Loaded += OnMainPageLoaded;
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        async void OnMainPageLoaded(object sender, RoutedEventArgs args)
        {
            if (accelerometer == null)
            {
                await new MessageDialog("Accelerometer is not available").ShowAsync();
            }
            else
            {
                CompositionTarget.Rendering += OnCompositionTargetRendering;
            }
        }

        void OnCompositionTargetRendering(object sender, object args)
        {
            AccelerometerReading reading = accelerometer.GetCurrentReading();

            if (reading == null)
                return;

            // Get elapsed time since last event
            TimeSpan timeSpan = (args as RenderingEventArgs).RenderingTime;
            double elapsedSeconds = (timeSpan - this.timeSpan).TotalSeconds;
            this.timeSpan = timeSpan;

            // Convert accelerometer reading to display coordinates
            double x = reading.AccelerationX;
            double y = -reading.AccelerationY;

            // Get current X-Y acceleration and smooth it
            acceleration = 0.5 * (acceleration + new Vector2(x, y));

            // Calculate new velocity and position
            ballVelocity += GRAVITY * acceleration * elapsedSeconds;
            ballPosition += ballVelocity * elapsedSeconds;

            // Check for hitting edge
            if (ballPosition.X - BALL_RADIUS < 0)
            {
                ballPosition = new Vector2(BALL_RADIUS, ballPosition.Y);
                ballVelocity = new Vector2(0, ballVelocity.Y);
            }
            if (ballPosition.X + BALL_RADIUS > this.ActualWidth)
            {
                ballPosition = new Vector2(this.ActualWidth - BALL_RADIUS, ballPosition.Y);
                ballVelocity = new Vector2(0, ballVelocity.Y);
            }
            if (ballPosition.Y - BALL_RADIUS < 0)
            {
                ballPosition = new Vector2(ballPosition.X, BALL_RADIUS);
                ballVelocity = new Vector2(ballVelocity.X, 0);
            }
            if (ballPosition.Y + BALL_RADIUS > this.ActualHeight)
            {
                ballPosition = new Vector2(ballPosition.X, this.ActualHeight - BALL_RADIUS);
                ballVelocity = new Vector2(ballVelocity.X, 0);
            }
            ball.Center = new Point(ballPosition.X, ballPosition.Y);
        }
    }
}
