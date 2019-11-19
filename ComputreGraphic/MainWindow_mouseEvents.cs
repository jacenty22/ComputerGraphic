using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;

namespace ComputreGraphic
{
    public partial class MainWindow : Window
    {

        private void MouseDownInShape(object sender, MouseButtonEventArgs e)
        {
            if (mouseFunction == MouseFunction.Edit)
            {
                startPoint = e.GetPosition(PaintField);
                object clickSource = e.OriginalSource;
                editShapeIndex = primitiveShapes.IndexOf(clickSource as Shape);
                shapeToEdit = clickSource as Shape;
                mouseFunction = MouseFunction.Edit;
                SetupEditParamsInView();
                EditParamsVisibility = Visibility.Visible;
                SetupResizePoints();
            }
        }

        private void MouseUpInCanvas(object sender, MouseButtonEventArgs e)
        {
            editShapeIndex = -1;
            resizeDirection = ResizeDirection.None;
            isMovingBezierEllipse = false;
        }

        private void MouseDownInCanvas(object sender, MouseButtonEventArgs e)
        {
            if (mouseFunction == MouseFunction.Draw)
            {
                startPoint = e.GetPosition(PaintField);
                if (shapeToDraw == ShapeToDraw.Line)
                {
                    primitiveShapes.Add(CreateNewShape((int)startPoint.X, (int)startPoint.Y, (int)startPoint.X, (int)startPoint.Y));
                }
                else
                {
                    primitiveShapes.Add(CreateNewShape((int)startPoint.X, (int)startPoint.Y, 0, 0));
                }
                primitiveShapes[primitiveShapes.Count - 1].PreviewMouseLeftButtonDown += new MouseButtonEventHandler(MouseDownInShape);
                PaintField.Children.Add(primitiveShapes[primitiveShapes.Count - 1]);
                setValuesForNewShape = true;
            }
            else if (mouseFunction == MouseFunction.Bezier)
            {
                BezierLeftClickFunction(e);
            }
        }

        private void MouseDownInResizePoint(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(resizePointNE))
            {
                resizeDirection = ResizeDirection.NE;
            }
            else if (sender.Equals(resizePointNW))
            {
                resizeDirection = ResizeDirection.NW;
            }
            else if (sender.Equals(resizePointSE))
            {
                resizeDirection = ResizeDirection.SE;
            }
            else if (sender.Equals(resizePointSW))
            {
                resizeDirection = ResizeDirection.SW;
            }
        }
        private void MouseMoveInCanvas(object sender, MouseEventArgs e)
        {
            endPoint = e.GetPosition(PaintField);
            if (mouseFunction == MouseFunction.Edit)
            {
                if (editShapeIndex != -1)
                {
                    double shiftX = endPoint.X - startPoint.X, shiftY = endPoint.Y - startPoint.Y;

                    var shape = primitiveShapes[editShapeIndex];
                    if (shape.GetType().Name.Equals("Line"))
                    {
                        (shape as Line).X1 += shiftX;
                        (shape as Line).Y1 += shiftY;
                        (shape as Line).X2 += shiftX;
                        (shape as Line).Y2 += shiftY;
                    }
                    else
                    {
                        shape.SetValue(Canvas.LeftProperty, (double)shape.GetValue(Canvas.LeftProperty) + shiftX);
                        shape.SetValue(Canvas.TopProperty, (double)shape.GetValue(Canvas.TopProperty) + shiftY);
                    }
                    SetupEditParamsInView();
                }
                ResizeShape();
                startPoint = endPoint;
            }
            else if ((mouseFunction == MouseFunction.Draw) && setValuesForNewShape)
            {
                DrawUsingMouse(e.GetPosition(PaintField));
                if (e.LeftButton == MouseButtonState.Released)
                {
                    editShapeIndex = -1;
                    editShapeIndex = -1;
                    setValuesForNewShape = false;
                }
            }
            else if (mouseFunction == MouseFunction.Bezier)
            {
                BezierUIEllipseMove(sender, e);
            }
        }

        private void DrawUsingMouse(Point endPoint)
        {
            if (shapeToDraw == ShapeToDraw.Line)
            {
                (primitiveShapes[primitiveShapes.Count - 1] as Line).X2 = endPoint.X;
                (primitiveShapes[primitiveShapes.Count - 1] as Line).Y2 = endPoint.Y;
            }
            else
            {
                double width = endPoint.X - startPoint.X;
                double height = endPoint.Y - startPoint.Y;
                if (width < 0)
                {
                    primitiveShapes[primitiveShapes.Count - 1].SetValue(Canvas.LeftProperty, endPoint.X);
                    width = Math.Abs(width);
                }
                else
                {
                    primitiveShapes[primitiveShapes.Count - 1].SetValue(Canvas.LeftProperty, startPoint.X);
                }
                if (height < 0)
                {
                    primitiveShapes[primitiveShapes.Count - 1].SetValue(Canvas.TopProperty, endPoint.Y);
                    height = Math.Abs(height);
                }
                else
                {
                    primitiveShapes[primitiveShapes.Count - 1].SetValue(Canvas.TopProperty, startPoint.Y);
                }
                primitiveShapes[primitiveShapes.Count - 1].Width = width;
                primitiveShapes[primitiveShapes.Count - 1].Height = height;
            }
        }
        private void SetupEditParamsInView()
        {
            var shapeToEdit = primitiveShapes[editShapeIndex];
            string[] labelsToSet = null;

            switch (shapeToEdit.GetType().Name)
            {
                case "Rectangle":
                case "Ellipse":
                    labelsToSet = labels[0];
                    firstParamTextBox.Text = Canvas.GetLeft(shapeToEdit).ToString("0");
                    secondParamTextBox.Text = Canvas.GetTop(shapeToEdit).ToString("0");
                    thirdParamTextBox.Text = shapeToEdit.Width.ToString("0");
                    fourthParamTextBox.Text = shapeToEdit.Height.ToString("0");
                    break;
                case "Line":
                    labelsToSet = labels[2];
                    firstParamTextBox.Text = (shapeToEdit as Line).X1.ToString();
                    secondParamTextBox.Text = (shapeToEdit as Line).Y1.ToString();
                    thirdParamTextBox.Text = (shapeToEdit as Line).X2.ToString();
                    fourthParamTextBox.Text = (shapeToEdit as Line).Y2.ToString();
                    break;
                default:
                    return;
            }
            FirstParam = labelsToSet[0];
            SecondParam = labelsToSet[1];
            ThirdParam = labelsToSet[2];
            FourthParam = labelsToSet[3];
        }

        private void SetupResizePoints()
        {
            resizePointNE.Visibility = Visibility.Visible;
            resizePointNW.Visibility = Visibility.Visible;

            if (shapeToEdit is Line)
            {
                var line = shapeToEdit as Line;
                resizePointNW.SetValue(Canvas.LeftProperty, line.X1);
                resizePointNW.SetValue(Canvas.TopProperty, line.Y1);
                resizePointNE.SetValue(Canvas.LeftProperty, line.X2);
                resizePointNE.SetValue(Canvas.TopProperty, line.Y2);
                resizePointSE.Visibility = Visibility.Collapsed;
                resizePointSW.Visibility = Visibility.Collapsed;
            }
            else
            {
                resizePointSE.Visibility = Visibility.Visible;
                resizePointSW.Visibility = Visibility.Visible;
                double x = Canvas.GetLeft(shapeToEdit), y = Canvas.GetTop(shapeToEdit);
                resizePointNW.SetValue(Canvas.LeftProperty, x - 3);
                resizePointNW.SetValue(Canvas.TopProperty, y - 3);
                resizePointNE.SetValue(Canvas.LeftProperty, x + shapeToEdit.Width);
                resizePointNE.SetValue(Canvas.TopProperty, y - 3);

                resizePointSW.SetValue(Canvas.LeftProperty, x - 3);
                resizePointSW.SetValue(Canvas.TopProperty, y + shapeToEdit.Height);
                resizePointSE.SetValue(Canvas.LeftProperty, x + shapeToEdit.Width);
                resizePointSE.SetValue(Canvas.TopProperty, y + shapeToEdit.Height);
                resizePointSE.Visibility = Visibility.Visible;
                resizePointSW.Visibility = Visibility.Visible;

            }
        }
        private void ResizeShape()
        {
            if (shapeToEdit == null)
                return;
            SetupResizePoints();
            double shiftX = endPoint.X - startPoint.X,
                shiftY = endPoint.Y - startPoint.Y;
            double x = endPoint.X, y = endPoint.Y;
            switch (resizeDirection)
            {
                case ResizeDirection.NW:
                    if (shapeToEdit is Line)
                    {
                        (shapeToEdit as Line).X1 = endPoint.X;
                        (shapeToEdit as Line).Y1 = endPoint.Y;
                    }
                    break;
                case ResizeDirection.NE:
                    if (shapeToEdit is Line)
                    {
                        (shapeToEdit as Line).X2 = endPoint.X;
                        (shapeToEdit as Line).Y2 = endPoint.Y;
                    }
                    else
                    {
                        y = Canvas.GetTop(shapeToEdit) + shiftY;
                        shiftX = -shiftX;
                        x = Canvas.GetLeft(shapeToEdit);
                    }
                    break;
                case ResizeDirection.SW:
                    x = Canvas.GetLeft(shapeToEdit) + shiftX;
                    y = Canvas.GetTop(shapeToEdit);
                    shiftY = -shiftY;
                    break;
                case ResizeDirection.SE:
                    x = Canvas.GetLeft(shapeToEdit);
                    y = Canvas.GetTop(shapeToEdit);
                    shiftY = -shiftY;
                    shiftX = -shiftX;
                    break;
            }
            if (!(shapeToEdit is Line) && resizeDirection != ResizeDirection.None)
            {
                if ((shapeToEdit.Width - shiftX) > 0 && (shapeToEdit.Height - shiftY) > 0)
                {
                    shapeToEdit.SetValue(Canvas.LeftProperty, x);
                    shapeToEdit.SetValue(Canvas.TopProperty, y);
                    shapeToEdit.Width -= shiftX;
                    shapeToEdit.Height -= shiftY;

                }
            }
        }
    }
}
