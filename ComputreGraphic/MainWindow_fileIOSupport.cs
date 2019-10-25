using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Image = System.Windows.Controls.Image;
using System.Windows.Media;
using Size = System.Windows.Size;

namespace ComputreGraphic
{
    public partial class MainWindow : Window
    {
        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.ppm)|*.jpg; *.jpeg; *.ppm";
            if (openFileDialog.ShowDialog() == true)
            {
                var selectedFilePath = openFileDialog.FileName;
                var extension = Path.GetExtension(selectedFilePath);
                bool correctReadFile = false;

                try
                {
                    if (extension.Equals(".jpg") || extension.Equals(".jpeg"))
                    {
                        bmp = new Bitmap(selectedFilePath);
                        correctReadFile = true;
                    }
                    else
                    {
                        bmp = PPMReader.ConvertPPMToBitmap(selectedFilePath);
                        if (bmp != null)
                            correctReadFile = true;
                    }
                    if (correctReadFile)
                    {
                        LoadNewImageToCanvas(selectedFilePath);
                    }

                }
                catch (Exception exception)
                {
                    Console.WriteLine("Błąd odczytu - " + exception);
                }
            }

        }
        private void LoadNewImageToCanvas(string selectedFilePath)
        {
            foreach (var p in primitiveShapes)
                PaintField.Children.Remove(p);
            primitiveShapes.Clear();
            resizePointNW.Visibility = Visibility.Collapsed;
            resizePointNE.Visibility = Visibility.Collapsed;
            resizePointSE.Visibility = Visibility.Collapsed;
            resizePointSW.Visibility = Visibility.Collapsed;
            editShapeIndex = -1;
            shapeToEdit = null;
            if (image != null)
                PaintField.Children.Remove(image);
            image = new Image();
            image.Source = BitmapToBitmapImage(bmp);
            image.Width = bmp.Width;
            image.Height = bmp.Height;
            PaintField.Width = bmp.Width;
            PaintField.Height = bmp.Height;
            Canvas.SetLeft(image, 0);
            Canvas.SetTop(image, 0); 
            PaintField.Children.Add(image);
           
            this.Title = "ImageEditor - " + Path.GetFileName(selectedFilePath);
        }
        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage;
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        private void SaveFileClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "(*.jpg)|*.jpg";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveFile(saveFileDialog.FileName);
            }
        }
        private void SaveFile(string filePath)
        {
            var dialog = new QualityDialog();
            if (dialog.ShowDialog() == true)
            {
                int quality = dialog.Quality;
                try
                {
                    double dpiX = 96d, dpiY = 96d;
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                   // System.Drawing.Imaging.PixelFormat pixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;

                    if (bmp != null)
                    {
                        dpiX = bmp.HorizontalResolution;
                        dpiY = bmp.VerticalResolution;
                        //pixelFormat = bmp.PixelFormat;
                    }
                    Transform transform = PaintField.LayoutTransform;
                    PaintField.LayoutTransform = null;
                    Size size = new Size(PaintField.Width, PaintField.Height);
                    PaintField.Measure(size);
                    PaintField.Arrange(new Rect(size));

                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)size.Width, (int)size.Height,
                        dpiX, dpiY, PixelFormats.Default);
                    rtb.Render(PaintField);

                    BitmapEncoder bitmapEncoder = new JpegBitmapEncoder();
                    bitmapEncoder.Frames.Add(BitmapFrame.Create(rtb));

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitmapEncoder.Save(ms);
                        new Bitmap(ms).Save(filePath, jpgEncoder, myEncoderParameters);
                    }
                    /*
                    using (Stream s = new MemoryStream())
                    {
                       bitmapEncoder.Save(s);
                        Bitmap newBitmap = new Bitmap(s);
                        Bitmap bitmapClone = new Bitmap(newBitmap.Width, newBitmap.Height, pixelFormat);
                            using (Graphics gr = Graphics.FromImage(bitmapClone))
                        {
                            gr.DrawImage(newBitmap, new Rectangle(0, 0, bitmapClone.Width, bitmapClone.Height));
                        }
                        newBitmap.Save(filePath, jgpEncoder, myEncoderParameters);
                    }
                    */
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Błąd zapisu - " + exception);
                }

            }
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
