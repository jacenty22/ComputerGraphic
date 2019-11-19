

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ComputreGraphic
{
    public partial class MainWindow
    {
        private List<Point> BezierPoints = new List<Point>();
        private List<Ellipse> BezierUIEllipses = new List<Ellipse>();
        private List<Line> BezierLines = new List<Line>();
        private bool BezierProcess = false;
        private bool isMovingBezierEllipse = false;
        private long[,] Pascal;
        private int ellipseIndex = -1;
        List<Func<double, bool, double>> functions = new List<Func<double, bool, double>>();

        private void BezierLeftClickFunction(MouseButtonEventArgs e)
        {
            if (isMovingBezierEllipse)
                return;
            if (BezierProcess)
            {
                BezierProcess = false;
                BezierPoints.Clear();
                PaintField.Children.Clear();
                BezierUIEllipses.Clear();
            }
            Point point = e.GetPosition(PaintField);
            BezierPoints.Add(point);
            Ellipse ellipse = new Ellipse() { Width = 12, Height = 12, Fill = Brushes.Gray };
            ellipse.SetValue(Canvas.LeftProperty, point.X - 6);
            ellipse.SetValue(Canvas.TopProperty, point.Y - 6);
            ellipse.MouseLeftButtonDown += BezierUIEllipseDown;
            PaintField.Children.Add(ellipse);
            BezierUIEllipses.Add(ellipse);
        }
        private void BezierUIEllipseMove(object sender, MouseEventArgs e)
        {
            if (isMovingBezierEllipse)
            {
                endPoint = e.GetPosition(PaintField);
                double shiftX = endPoint.X - startPoint.X, shiftY = endPoint.Y - startPoint.Y;
                BezierUIEllipses[ellipseIndex].SetValue(Canvas.LeftProperty, (double)BezierUIEllipses[ellipseIndex].GetValue(Canvas.LeftProperty) + shiftX);
                BezierUIEllipses[ellipseIndex].SetValue(Canvas.TopProperty, (double)BezierUIEllipses[ellipseIndex].GetValue(Canvas.TopProperty) + shiftY);
                Point point = new Point() { X = BezierPoints[ellipseIndex].X + shiftX, Y = BezierPoints[ellipseIndex].Y + shiftY };
                BezierPoints[ellipseIndex] = point;
                startPoint = endPoint;
                if (BezierProcess)
                    BezierFunction();
            }
        }

        private void BezierUIEllipseDown(object sender, MouseButtonEventArgs e)
        {
            isMovingBezierEllipse = true;
            startPoint = e.GetPosition(PaintField);
            ellipseIndex = BezierUIEllipses.IndexOf((Ellipse)sender);
        }

        private void MouseRightClick(object sender, MouseButtonEventArgs e)
        {
            if (BezierProcess)
            {
                return;
            }
            Point point = e.GetPosition(PaintField);
            BezierPoints.Add(point);
            Ellipse ellipse = new Ellipse() { Width = 12, Height = 12, Fill = Brushes.Gray };
            ellipse.SetValue(Canvas.LeftProperty, point.X - 6);
            ellipse.SetValue(Canvas.TopProperty, point.Y - 6);
            ellipse.MouseLeftButtonDown += BezierUIEllipseDown;
            ellipse.MouseMove += BezierUIEllipseMove;
            PaintField.Children.Add(ellipse);
            BezierUIEllipses.Add(ellipse);
            BezierProcess = true;

            var degree = BezierPoints.Count;
            functions = new List<Func<double, bool, double>>();

            for (int i = 0; i < degree; i++)
            {
                int j = i;
                functions.Add((t, isX) => (Pascal[degree - 1, j]) * (isX ? BezierPoints[j].X : BezierPoints[j].Y) * Math.Pow(t, j) * Math.Pow((1 - t), degree - j - 1));
            }

            BezierFunction();
        }

        private void BezierFunction()
        {
            foreach (Line line in BezierLines)
            {
                PaintField.Children.Remove(line);
            }
            Point prevPoint = BezierPoints[0];
            Point currentPoint;

            for (double t = 0; t <= 1.00001d; t += 0.005d)
            {
                double xValue = 0;
                double yValue = 0;
                foreach (var x in functions)
                {
                    xValue += x(t, true);
                    yValue += x(t, false);
                }
                currentPoint = new Point((int)xValue, (int)yValue);
                Line line = new Line() { X1 = prevPoint.X, Y1 = prevPoint.Y, X2 = currentPoint.X, Y2 = currentPoint.Y, Stroke = Brushes.Blue };
                PaintField.Children.Add(line);
                BezierLines.Add(line);
                prevPoint = currentPoint;
            }
        }
        //private int degree;
        //private int[] pascalOneLine;

        //public int GetCountOfPointsForDegree(int degree)
        //{
        //    if (degree < 1)
        //        return 0;
        //    List<List<int>> pascal = new List<List<int>>();

        //    for (int i = 0; i < degree; i++)
        //    {
        //        List<int> oneLine = new List<int>(i + 1);
        //        oneLine[0] = 1;
        //        oneLine[degree] = 1;

        //        for (int j = 0; j < (i - 1); j++)
        //        {
        //            oneLine[j + 1] = pascal[i - 1][j] + pascal[i - 1][j + 1];
        //        }
        //        pascal.Add(oneLine);
        //    }
        //    pascalOneLine = pascal[degree].ToArray();
        //    return (degree + 1);
        //}

        //public List<Point> GenerateBezierCurve(Point startPoint, Point endPoint, List<Point> checkpoints)
        //{
        //    if (degree < 1)
        //        return new List<Point>();
        //    return null;
        //}
    }
}
