using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ComputreGraphic
{
    public static class ShapeCreator
    {
        public static Rectangle GetNewRectangle(int X, int Y, Canvas canvas, int width, int height)
        {
            Rectangle rectangle = null;

            if (width >= 0 && height >= 0)
            {
                rectangle = new Rectangle()
                {
                    Fill = Brushes.Red,
                };
                if ((X + width) > canvas.Width)
                {
                    width = (int)canvas.Width - X - 1;
                }
                if ((Y + height) > canvas.Height)
                {
                    height = (int)canvas.Height - Y + 1;
                }
                rectangle.Width = width;
                rectangle.Height = height;
                rectangle.SetValue(Canvas.LeftProperty, (double)X);
                rectangle.SetValue(Canvas.TopProperty, (double)Y);
            }

            return rectangle;
        }
        public static Ellipse GetNewEllipse(int X, int Y, Canvas canvas, int radius1, int radius2)
        {
            Ellipse ellipse = null;
            if ((radius1 >= 0) && (radius2 >= 0))
            {
                ellipse = new Ellipse()
                {
                    Fill = Brushes.Yellow
                };

                ellipse.Width = radius1;
                ellipse.Height = radius2;
                ellipse.SetValue(Canvas.LeftProperty, (double)X);
                ellipse.SetValue(Canvas.TopProperty, (double)Y);
            }

            return ellipse;
        }
        public static Line GetNewLine(int X1, int Y1, int X2, int Y2, Canvas canvas)
        {
            Line line = null;
            line = new Line()
            {
                Stroke = Brushes.Black,
                X1 = X1,
                Y1 = Y1,
                X2 = X2,
                Y2 = Y2,
                StrokeThickness = 4,
            };

            return line;
        }
    }
}
