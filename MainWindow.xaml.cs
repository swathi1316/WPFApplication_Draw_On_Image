using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace DrawImage
{
    public partial class MainWindow : Window
    {
        // Variables to keep track of the state of the drawing process
        private bool isDraw;
        private System.Windows.Point startDrawingPoint;
        private System.Windows.Shapes.Rectangle rectanglePoint;
        private System.Windows.Shapes.Rectangle recSelected = null;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                // Handle the exception here or re-throw it to the calling code
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open a file dialog to let the user select an image
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png)|*.BMP;*.JPG;*.JPEG;*.PNG",
                    Title = "Select an Image File"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // Get the file path of the selected image
                    var imagePath = openFileDialog.FileName;

                    // Load the image into a BitmapImage object and set it as the source of the Image element
                    var bitmap = new BitmapImage(new Uri(imagePath));
                    MyImage.Source = bitmap;

                    // Remove any existing rectangles from the Grid
                    var existingRectangles = MyGrid.Children.OfType<System.Windows.Shapes.Rectangle>().ToList();
                    foreach (var rectangle in existingRectangles)
                    {
                        MyGrid.Children.Remove(rectangle);
                    }

                    // Attach event handlers to the Image element to allow the user to draw rectangles on it
                    MyImage.MouseDown += Rec_MouseDown;
                    MyImage.MouseMove += Rec_MouseMovement;
                    MyImage.MouseUp += Rec_MouseUp;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception here or re-throw it to the calling code
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private void Rec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // If the user is not already drawing a rectangle
                if (!isDraw)
                {
                    // Set the starting point of the rectangle to the current mouse position
                    startDrawingPoint = e.GetPosition(MyImage);

                    // Create a new rectangle and add it to the Grid
                    rectanglePoint = new System.Windows.Shapes.Rectangle
                    {
                        Stroke = System.Windows.Media.Brushes.Black,
                        StrokeThickness = 2
                    };
                    rectanglePoint.MouseDown += Color_Rec_MouseDown;
                    MyGrid.Children.Add(rectanglePoint);

                    // Set the flag to indicate that the user is drawing a rectangle
                    isDraw = true;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception here or re-throw it to the calling code
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        private void Rec_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // If the user was drawing a rectangle
                if (isDraw)
                {
                    // If the rectangle is too small, remove it from the Grid
                    if (rectanglePoint.Width < 10 || rectanglePoint.Height < 10)
                    {
                        MyGrid.Children.Remove(rectanglePoint);
                    }
                    else
                    {
                        // Set the flag to indicate that the user is not drawing a rectangle anymore
                        isDraw = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception here or re-throw it to the calling code
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }


        private void Rec_MouseMovement(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                // If the user is drawing a rectangle
                if (isDraw)
                {
                    // Calculate the size and position of the rectangle based on the current mouse position
                    double width = Math.Abs(e.GetPosition(MyImage).X - startDrawingPoint.X);
                    double height = Math.Abs(e.GetPosition(MyImage).Y - startDrawingPoint.Y);
                    double left = Math.Min(startDrawingPoint.X, e.GetPosition(MyImage).X);
                    double top = Math.Min(startDrawingPoint.Y, e.GetPosition(MyImage).Y);

                    // Get the dimensions of the image
                    double imageWidth = MyImage.ActualWidth;
                    double imageHeight = MyImage.ActualHeight;

                    // Restrict the rectangle to be drawn only inside the image
                    if (left < 0) left = 0;
                    if (top < 0) top = 0;
                    if (left + width > imageWidth) width = imageWidth - left;
                    if (top + height > imageHeight) height = imageHeight - top;

                    // Set the properties of the rectangle to draw it on the screen
                    Canvas.SetLeft(rectanglePoint, left);
                    Canvas.SetTop(rectanglePoint, top);
                    rectanglePoint.Width = width;
                    rectanglePoint.Height = height;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception here or re-throw it to the calling code
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        private void Color_Rec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var rect = sender as System.Windows.Shapes.Rectangle;
                if (rect != null)
                {
                    var colorDialog = new System.Windows.Forms.ColorDialog();
                    if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        rect.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                    }

                    // Select the rectangle and store it in the recSelected variable
                    recSelected = rect;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception here or re-throw it to the calling code
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // If a rectangle is selected, remove it from the Grid

                if (recSelected != null)
                {
                    MyGrid.Children.Remove(recSelected);
                    recSelected = null;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception here or re-throw it to the calling code
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a new instance of SaveFileDialog class
                Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();

                // Set the file filter and default extension
                dialog.Filter = "PNG Image (*.png)|*.png";
                dialog.DefaultExt = ".png";

                // Show the SaveFileDialog and wait for the user to select a file
                if (dialog.ShowDialog() == true)
                {
                    // Create a new BitmapSource object from the current state of the image
                    Rect bounds = VisualTreeHelper.GetDescendantBounds(MyImage);
                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Default);
                    DrawingVisual dv = new DrawingVisual();

                    using (DrawingContext dc = dv.RenderOpen())
                    {
                        // Draw the image onto the DrawingContext
                        dc.DrawImage(MyImage.Source, new Rect(0, 0, bounds.Width, bounds.Height));

                        // Loop through each rectangle in the Grid and draw it on the image
                        foreach (System.Windows.Shapes.Rectangle rect in MyGrid.Children.OfType<System.Windows.Shapes.Rectangle>())
                        {
                            SolidColorBrush brush = rect.Fill as SolidColorBrush;
                            if (brush != null)
                            {
                                System.Windows.Media.Pen pen = new System.Windows.Media.Pen(brush, 1.0);
                                dc.DrawRectangle(null, pen, new Rect(Canvas.GetLeft(rect), Canvas.GetTop(rect), rect.Width, rect.Height));
                            }
                        }
                    }

                    rtb.Render(dv);

                    // Create a new Bitmap object from the BitmapSource object
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    using (Stream stream = File.Create(dialog.FileName))
                    {
                        encoder.Save(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception here or re-throw it to the calling code
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

    }

}





















