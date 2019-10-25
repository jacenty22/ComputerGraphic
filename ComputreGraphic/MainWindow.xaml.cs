using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Data;
using System.Drawing;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;
using Image = System.Windows.Controls.Image;

namespace ComputreGraphic
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Enums
        private enum ShapeToDraw { Rectangle = 0, Circle, Line };
        private enum MouseFunction { None, Edit, Draw };

        private enum ResizeDirection { None, NW, NE, SE, SW};
        #endregion

        #region Fields

        #region private fields
        private ShapeToDraw shapeToDraw = ShapeToDraw.Rectangle;
        private int editShapeIndex = -1;
        private Shape shapeToEdit = null;
        private Point startPoint, endPoint;
        private MouseFunction mouseFunction = MouseFunction.Draw;
        private ResizeDirection resizeDirection = ResizeDirection.None;
        private bool setValuesForNewShape = false;
        private string _firstParam = "X", _secondParam = "Y", _thridParam = "szerokość", _fourthParam = "wysokość";
        private string _firstParamDraw = "X", _secondParamDraw = "Y",
                        _thirdParamDraw = "wysokość",
                        _fourthParamDraw = "szerokość";
        private Visibility _editParamsVisibility = Visibility.Hidden;
        private readonly List<string[]> labels = new List<string[]>()
        {
            new string[]{ "X", "Y", "szerokość", "wysokość"},
            new string[]{ "X", "Y", "szerokość", "wysokość"},
            new string[]{ "X1", "Y1", "X2", "Y2" },
        };
        private Ellipse resizePointNW = new Ellipse() { Visibility = Visibility.Collapsed, Width = 7, Height = 7, Fill = Brushes.Gray },
                        resizePointNE = new Ellipse() { Visibility = Visibility.Collapsed, Width = 7, Height = 7, Fill = Brushes.Gray },
                        resizePointSE = new Ellipse() { Visibility = Visibility.Collapsed, Width = 7, Height = 7, Fill = Brushes.Gray },
                        resizePointSW = new Ellipse() { Visibility = Visibility.Collapsed, Width = 7, Height = 7, Fill = Brushes.Gray };
        private List<Shape> primitiveShapes { get; set; } = new List<Shape>();
        private Bitmap bmp;
        private Image image;
        #endregion
        #region public fields
        public string FirstParam
        {
            set
            {
                _firstParam = value;
                OnPropertyRaised(nameof(FirstParam));
            }
            get { return _firstParam; }
        }
        public string SecondParam
        {
            set
            {
                _secondParam = value;
                OnPropertyRaised(nameof(SecondParam));
            }
            get { return _secondParam; }
        }
        public string ThirdParam
        {
            set
            {
                _thridParam = value;
                OnPropertyRaised(nameof(ThirdParam));

            }
            get { return _thridParam; }
        }
        public string FourthParam
        {
            set
            {
                _fourthParam = value;
                OnPropertyRaised(nameof(FourthParam));

            }
            get { return _fourthParam; }
        }

        public string FirstParamDraw
        {
            set
            {
                _firstParamDraw = value;
                OnPropertyRaised(nameof(FirstParamDraw));
            }
            get { return _firstParamDraw; }
        }
        public string SecondParamDraw
        {
            set
            {
                _secondParamDraw = value;
                OnPropertyRaised(nameof(SecondParamDraw));
            }
            get { return _secondParamDraw; }
        }
        public string ThirdParamDraw
        {
            set
            {
                _thirdParamDraw = value;
                OnPropertyRaised(nameof(ThirdParamDraw));
            }
            get { return _thirdParamDraw; }
        }
        public string FourthParamDraw
        {
            set
            {
                _fourthParamDraw = value;
                OnPropertyRaised(nameof(FourthParamDraw));
            }
            get { return _fourthParamDraw; }
        }


        public Visibility EditParamsVisibility
        {
            set
            {
                _editParamsVisibility = value;
                OnPropertyRaised(nameof(EditParamsVisibility));
            }
            get
            {
                return _editParamsVisibility;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            RectangleRadioButton.IsChecked = true;
            RadioButtonDraw.IsChecked = true;

            resizePointNE.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(MouseDownInResizePoint);
            resizePointNW.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(MouseDownInResizePoint);
            resizePointSE.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(MouseDownInResizePoint);
            resizePointSW.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(MouseDownInResizePoint);
            Canvas.SetZIndex(resizePointNW, int.MaxValue);
            Canvas.SetZIndex(resizePointNE, int.MaxValue);
            Canvas.SetZIndex(resizePointSW, int.MaxValue);
            Canvas.SetZIndex(resizePointSE, int.MaxValue);
            PaintField.Children.Add(resizePointNE);
            PaintField.Children.Add(resizePointNW);
            PaintField.Children.Add(resizePointSE);
            PaintField.Children.Add(resizePointSW);
        }
        void ChangeShape(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                var selectedRadioButton = (RadioButton)sender;
                if (selectedRadioButton.Equals(RectangleRadioButton))
                {
                    shapeToDraw = ShapeToDraw.Rectangle;
                }
                else if (selectedRadioButton.Equals(CircleRadioButton))
                {
                    shapeToDraw = ShapeToDraw.Circle;
                }
                else if (selectedRadioButton.Equals(LineRadioButton))
                {
                    shapeToDraw = ShapeToDraw.Line;
                }
                var labelsToSet = labels[(int)shapeToDraw];
                FirstParamDraw = labelsToSet[0];
                SecondParamDraw = labelsToSet[1];
                ThirdParamDraw = labelsToSet[2];
                FourthParamDraw = labelsToSet[3];
            }
        }
        void DrawButtonHandle(object sender, RoutedEventArgs e)
        {
            int X = -1, Y = -1, width = -1, height = -1;

            if (int.TryParse(XPositionTextBox.Text, out X)
                && int.TryParse(YPositionTextBox.Text, out Y)
                && int.TryParse(widthTextBox.Text, out width)
                && int.TryParse(heightTextBox.Text, out height))
            {
                Shape newShape = CreateNewShape(X, Y, width, height);
                if (newShape != null)
                {
                    newShape.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(MouseDownInShape);
                    primitiveShapes.Add(newShape);
                    PaintField.Children.Add(newShape as UIElement);
                }
            }
            else
            {
                MessageBox.Show("Wprowadziłeś niepoprawne parametry!", "Błędne dane", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Shape CreateNewShape(int X, int Y, int width, int height)
        {
            Shape newShape = null;
            switch (shapeToDraw)
            {
                case ShapeToDraw.Rectangle:
                    newShape = ShapeCreator.GetNewRectangle(X, Y, PaintField, width, height);
                    break;
                case ShapeToDraw.Circle:
                    newShape = ShapeCreator.GetNewEllipse(X, Y, PaintField, width, height);
                    break;
                case ShapeToDraw.Line:
                    newShape = ShapeCreator.GetNewLine(X, Y, width, height, PaintField);
                    break;
            }
            return newShape;
        }
        void ChangeMouseFunction(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(RadioButtonDraw))
            {
                mouseFunction = MouseFunction.Draw;
                EditParamsVisibility = Visibility.Hidden;
                resizePointNE.Visibility = Visibility.Collapsed;
                resizePointNW.Visibility = Visibility.Collapsed;
                resizePointSE.Visibility = Visibility.Collapsed;
                resizePointSW.Visibility = Visibility.Collapsed;

            }
            else if (sender.Equals(RadioButtonEdit))
            {
                mouseFunction = MouseFunction.Edit;
            }
        }
        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            int param1 = -1, param2 = -1, param3 = -1, param4 = -1;
            ;
            if (int.TryParse(firstParamTextBox.Text, out param1)
                && int.TryParse(secondParamTextBox.Text, out param2)
                && int.TryParse(thirdParamTextBox.Text, out param3)
                && int.TryParse(fourthParamTextBox.Text, out param4))
            {

                if (shapeToEdit.GetType().Name.Equals("Line"))
                {
                    (shapeToEdit as Line).X1 = param1;
                    (shapeToEdit as Line).Y1 = param2;
                    (shapeToEdit as Line).X2 = param3;
                    (shapeToEdit as Line).Y2 = param4;
                }
                else
                {
                    shapeToEdit.SetValue(Canvas.LeftProperty, (double)param1);
                    shapeToEdit.SetValue(Canvas.TopProperty, (double)param2);
                    shapeToEdit.Width = param3;
                    shapeToEdit.Height = param4;
                }
                ResizeShape();
            }
            else
            {
                MessageBox.Show("Wprowadziłeś niepoprawne parametry!", "Błędne dane", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}

