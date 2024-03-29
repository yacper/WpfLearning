﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SkiaSharp;

namespace SkiaSharpTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var writeableBitmap = CreateImage(800, 600);
            UpdateImage(writeableBitmap);
            Image.Source = writeableBitmap;
        }

        private WriteableBitmap CreateImage(int width, int height)
        {
            var writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
            return writeableBitmap;
        }
        private void UpdateImage(WriteableBitmap writeableBitmap)
        {
            int width = (int)writeableBitmap.Width,
                height = (int)writeableBitmap.Height;
            writeableBitmap.Lock();
            var skImageInfo = new SKImageInfo()
            {
                Width = width,
                Height = height,
                ColorType = SKColorType.Bgra8888,
                AlphaType = SKAlphaType.Premul,
                ColorSpace = SKColorSpace.CreateSrgb()
            };

            using (var surface = SKSurface.Create(skImageInfo, writeableBitmap.BackBuffer))
            {
                SKCanvas canvas = surface.Canvas;
                SKPaint paint = new SKPaint() {Color = new SKColor(0, 0, 0), IsAntialias = true,};
                canvas.Clear(new SKColor(130, 130, 130));
                canvas.DrawText("SkiaSharp in Wpf!", 50, 200, new SKPaint() { IsAntialias = true,  Color = new SKColor(0, 0, 0), TextSize = 100 });
                canvas.DrawLine(0,0, 200, 200, paint);
                canvas.DrawLine(200,200, 250, 300, paint);
                canvas.DrawText("Using SkiaSharp for making graphs in WPF", new SKPoint(50, 500), new SKPaint(new SKFont(SKTypeface.FromFamilyName("Microsoft YaHei UI") ))
                {
                    IsAntialias = true,  
                    Color = new SKColor(0, 0, 0),
                    TextSize =12 
                });
            }
            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            writeableBitmap.Unlock();
        }
    }
}
