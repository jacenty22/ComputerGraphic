using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace ComputreGraphic
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window, INotifyPropertyChanged
    {
        #region fields
        private bool isRGBSelected = true;
        private Visibility _fourthSliderVisibility = Visibility.Collapsed;
        private Color _selectedColor;
        private double[] CMYK = new double[4];
        private bool ignoreChangeValues = false;

        public Visibility FourthSliderVisibility
        {
            get { return _fourthSliderVisibility; }
            set
            {
                _fourthSliderVisibility = value;
                OnPropertyRaised(nameof(FourthSliderVisibility));
            }
        }
        public Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                _selectedColor = value;
                OnPropertyRaised(nameof(SelectedColor));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public ColorPicker()
        {
            InitializeComponent();
            DataContext = this;
            SelectedColor = Color.FromRgb((byte)FirstSlider.Value, (byte)SecondSlider.Value, (byte)ThirdSlider.Value);
            RGBRadioButton.IsChecked = true;
            CMYKRadioButton.Checked += ChangeColorModel;
            RGBRadioButton.Checked += ChangeColorModel;
            FirstSlider.ValueChanged += ColorValueChanged;
            SecondSlider.ValueChanged += ColorValueChanged;
            ThirdSlider.ValueChanged += ColorValueChanged;
            FourthSlider.ValueChanged += ColorValueChanged;
            CalculateRGBToCMYK();
        }


        void ChangeColorModel(object sender, RoutedEventArgs e)
        {
            ignoreChangeValues = true;

            if (sender.Equals(RGBRadioButton))
            {
                FirstSliderLabel.Content = "R: ";
                SecondSliderLabel.Content = "G: ";
                ThirdSliderLabel.Content = "B: ";
                FourthSliderVisibility = Visibility.Collapsed;
                ColorModelNameLabel.Content = "CMYK";

                FirstSlider.Maximum = 255;
                SecondSlider.Maximum = 255;
                ThirdSlider.Maximum = 255;
                isRGBSelected = true;
            }
            else if (sender.Equals(CMYKRadioButton))
            {
                FirstSliderLabel.Content = "C: ";
                SecondSliderLabel.Content = "M: ";
                ThirdSliderLabel.Content = "Y: ";
                FourthSliderLabel.Content = "K: ";

                FirstSlider.Maximum = 100;
                SecondSlider.Maximum = 100;
                ThirdSlider.Maximum = 100;
                FourthSlider.Maximum = 100;

                FourthSliderVisibility = Visibility.Visible;
                ColorModelNameLabel.Content = "RGB";
                isRGBSelected = false;
            }
            UpdateContent();
            ignoreChangeValues = false;
        }

        private void UpdateContent()
        {
            if (isRGBSelected)
            {
                ColorModelValuesLabel.Content = String.Format("({0:N0}%,{1:N0}%,{2:N0}%,{3:N0}%)",
                                                CMYK[0], CMYK[1], CMYK[2], CMYK[3]);
                FirstSlider.Value = SelectedColor.R;
                SecondSlider.Value = SelectedColor.G;
                ThirdSlider.Value = SelectedColor.B;
            }
            else
            {
                ColorModelValuesLabel.Content = String.Format("({0:N0} ,{1:N0} ,{2:N0})",
                                                SelectedColor.R, SelectedColor.G, SelectedColor.B);

                FirstSlider.Value = Math.Ceiling(CMYK[0]);
                SecondSlider.Value =Math.Ceiling(CMYK[1]);
                ThirdSlider.Value = Math.Ceiling(CMYK[2]);
                FourthSlider.Value =Math.Ceiling(CMYK[3]);
            }
        }

        private void CalculateRGBToCMYK()
        {
            double R = FirstSlider.Value / 255.0, G = SecondSlider.Value / 255.0, B = ThirdSlider.Value / 255.0;//RGB
            double black = Math.Min(1 - R, Math.Min(1 - G, 1 - B));
            double cyan = (1 - R - black) / (1 - black);
            if (double.IsNaN(cyan))
                cyan = 0;
            double magenta = (1 - G - black) / (1 - black);
            if (double.IsNaN(magenta))
                magenta = 0;
            double yellow = (1 - B - black) / (1 - black);
            if (double.IsNaN(yellow))
                yellow = 0;
            SelectedColor = Color.FromRgb((byte)FirstSlider.Value, (byte)SecondSlider.Value, (byte)ThirdSlider.Value);

            CMYK[0] = cyan * 100;
            CMYK[1] = magenta * 100;
            CMYK[2] = yellow * 100;
            CMYK[3] = black * 100;
            ColorModelValuesLabel.Content = String.Format("({0:N0}%,{1:N0}%,{2:N0}%,{3:N0}%)",
                CMYK[0], CMYK[1], CMYK[2], CMYK[3]);
        }

        private void CalculateCMYKToRGB()
        {
            double[] CMYKTmp = { FirstSlider.Value / 100, SecondSlider.Value / 100, ThirdSlider.Value / 100, FourthSlider.Value / 100 };
            double red = (1 - Math.Min(1, CMYKTmp[0] * (1 - CMYKTmp[3]) + CMYKTmp[3])) * 255,
                green = (1 - Math.Min(1, CMYKTmp[1] * (1 - CMYKTmp[3]) + CMYKTmp[3])) * 255,
                blue = (1 - Math.Min(1, CMYKTmp[2] * (1 - CMYKTmp[3]) + CMYKTmp[3])) * 255;

            SelectedColor = Color.FromRgb((byte)red, (byte)green, (byte)blue);
            CMYK[0] = CMYKTmp[0] * 100;
            CMYK[1] = CMYKTmp[1] * 100;
            CMYK[2] = CMYKTmp[2] * 100;
            CMYK[3] = CMYKTmp[3] * 100;
            ColorModelValuesLabel.Content = String.Format("({0:N0} ,{1:N0} ,{2:N0})",
                red, green, blue);
        }
        void ColorValueChanged(object sender, RoutedEventArgs e)
        {
            if (ignoreChangeValues)
                return;
            if (isRGBSelected)
            {
                CalculateRGBToCMYK();
            }
            else
            {
                CalculateCMYKToRGB();
            }
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
