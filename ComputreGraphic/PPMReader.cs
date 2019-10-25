using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ComputreGraphic
{
    static public class PPMReader
    {
        enum PPMFileType { P3, P6, OTHERS, UNDEFINED };

        public class PPMProperties
        {
            public int? Height { get; set; }
            public int? Width { get; set; }
            public int? ColourRange { get; set; }
        }

        private static PPMFileType openedType;
        private static PPMProperties dimension;
        private static List<string> otherValues;
        private static Bitmap bmp;
        private static BitmapData data;
        private static string name;
        private static string totalReadingLines;
        private static void InitalizeVariables()
        {
            openedType = PPMFileType.UNDEFINED;
            dimension = new PPMProperties();
            otherValues = new List<string>();
            bmp = null;
            data = null;
            totalReadingLines = string.Empty;
        }
        public static Bitmap ConvertPPMToBitmap(string filePath)
        {
            name = filePath;
            Console.WriteLine(DateTime.Now.ToString("m:s:fff") + " - START odczytu pliku PPM");
            InitalizeVariables();
            if (Path.GetExtension(filePath).Equals(".ppm"))
            {
                try
                {
                    using (var file = new StreamReader(new FileStream(filePath, FileMode.Open), Encoding.ASCII))
                    {
                        ReadFileHeader(file);
                        if (openedType != PPMFileType.UNDEFINED && openedType != PPMFileType.OTHERS)
                        {
                            bmp = new Bitmap(dimension.Width.Value, dimension.Height.Value, PixelFormat.Format24bppRgb);
                            data = bmp.LockBits(new System.Drawing.Rectangle(0, 0,
                                dimension.Width.Value, dimension.Height.Value),
                                ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                            if (openedType == PPMFileType.P3)
                            {
                                return HandleP3File(file);
                            }
                            else if (openedType == PPMFileType.P6)
                            {
                                return HandleP6File(file);
                            }
                        }

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Błąd odczytu pliku - " + e);
                    MessageBox.Show("Błąd odczytu pliku");
                }
            }

            return null;
        }

        private static Bitmap HandleP6File(StreamReader file)
        {
            byte[] buffer = new byte[1024 * 1024 * 32];
            int counter = 0;
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int colorValue = 0;
                counter = bmp.Width * bmp.Height * 3;
                byte RGBoffset = 2;

                foreach (var value in otherValues)
                {
                    if (int.TryParse(value, out colorValue))
                    {
                        ptr[RGBoffset] = (byte)((colorValue * 255) / dimension.ColourRange.Value);
                        if (RGBoffset-- == 0)
                        {
                            RGBoffset = 2;
                            ptr += 3;
                        }
                        if ((counter--) == 0)
                            break;
                    }
                }
                long offset = System.Text.Encoding.ASCII.GetByteCount(totalReadingLines);
                file.Close();
                var fileStream = new FileStream(name, FileMode.Open, FileAccess.Read);
                colorValue = 0;
                for (int length = fileStream.Read(buffer, 0, buffer.Length); (length != 0 && counter != 0); length = fileStream.Read(buffer, 0, buffer.Length))
                {
                    for (int i = (int)offset; (i < length && counter > 0); i++)
                    {
                        offset = 0;
                        ptr[RGBoffset] = (byte)((buffer[i] * 255) / dimension.ColourRange.Value);
                        if (RGBoffset-- == 0)
                        {
                            RGBoffset = 2;
                            ptr += 3;
                        }
                        if ((counter--) == 0)
                            break;
                    }
                }
                bmp.UnlockBits(data);
            }
            if (counter > 0)
            {
                MessageBox.Show("Zbyt mało wartości kolorów dla zadeklarownych wymiarów", "Błąd odczytu pliku", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            Console.WriteLine(DateTime.Now.ToString("m:s:fff") + " - KONIEC odczytu pliku PPM");
            return bmp;
        }
        private static Bitmap HandleP3File(StreamReader file)
        {
            char[] buffer = new char[1024 * 1024 * 32];
            bool isComment = false;
            int counter = 0;
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int colorValue = 0;
                bool isLastDigit = false;
                counter = bmp.Width * bmp.Height * 3;
                byte RGBoffset = 2;

                foreach (var value in otherValues)
                {
                    if (int.TryParse(value, out colorValue))
                    {
                        ptr[RGBoffset] = (byte)((colorValue * 255) / dimension.ColourRange.Value);
                        if (RGBoffset-- == 0)
                        {
                            RGBoffset = 2;
                            ptr += 3;
                        }
                        if ((counter--) == 0)
                            break;
                    }
                }
                colorValue = 0;
                for (int length = file.ReadBlock(buffer, 0, buffer.Length); (length != 0 && counter > 0); length = file.ReadBlock(buffer, 0, buffer.Length))
                {
                    for (int i = 0; i < length; i++)
                    {
                        if (Char.IsDigit(buffer[i]) && !isComment)
                        {
                            colorValue = (colorValue * 10 + buffer[i] - '0');
                            isLastDigit = true;
                        }
                        else if (isComment && (buffer[i] == '\n'))
                        {
                            isComment = false;
                        }
                        else
                        {
                            if (isLastDigit)
                            {
                                if (colorValue <= dimension.ColourRange.Value)
                                {
                                    ptr[RGBoffset] = (byte)((colorValue * 255) / dimension.ColourRange.Value);
                                    counter--;
                                    if (RGBoffset-- == 0)
                                    {
                                        RGBoffset = 2;
                                        ptr += 3;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(colorValue);
                                }
                                isLastDigit = false;
                                colorValue = 0;
                            }
                            if (buffer[i] == '#')
                                isComment = true;
                        }
                    };
                }
                bmp.UnlockBits(data);
            }
            Console.WriteLine(DateTime.Now.ToString("m:s:fff") + " - KONIEC odczytu pliku PPM");
            if (counter > 0)
            {
                MessageBox.Show("Zbyt mało wartości kolorów dla zadeklarownych wymiarów", "Błąd odczytu pliku", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            return bmp;
        }

        private static void ReadFileHeader(StreamReader file)
        {
            string line;
            while ((line = file.ReadLine()) != null)
            {
                totalReadingLines += line + '\n';
                int hashApperanceIndex = line.IndexOf('#');
                line = line.Substring(0, (hashApperanceIndex == -1 ? line.Length : hashApperanceIndex));
                foreach (var value in line.Split(null))
                {
                    if (string.IsNullOrEmpty(value))
                        continue;
                    if (openedType == PPMFileType.UNDEFINED)
                    {
                        switch (value)
                        {
                            case "P6":
                                openedType = PPMFileType.P6;
                                break;
                            case "P3":
                                openedType = PPMFileType.P3;
                                break;
                            default:
                                throw new Exception("Undefined file type");
                        }
                    }
                    else if (!dimension.Width.HasValue)
                    {
                        int width;
                        if (int.TryParse(value, out width) && width > 0)
                        {
                            dimension.Width = width;
                        }
                        else
                        {
                            throw new Exception("bad value");
                        }
                    }
                    else if (!dimension.Height.HasValue)
                    {
                        int height;
                        if (int.TryParse(value, out height) && height > 0)
                        {
                            dimension.Height = height;
                        }
                        else
                        {
                            throw new Exception("bad value");
                        }
                    }
                    else if (!dimension.ColourRange.HasValue)
                    {
                        int range;
                        if (int.TryParse(value, out range) && range > 0)
                        {
                            dimension.ColourRange = range;
                        }
                        else
                        {
                            throw new Exception("bad value");
                        }
                    }
                    else
                    {
                        otherValues.Add(value);
                    }
                }
                if (dimension.ColourRange.HasValue)
                {
                    break;
                }
            }

        }
    }
}

