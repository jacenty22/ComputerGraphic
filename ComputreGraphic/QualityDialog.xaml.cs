using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ComputreGraphic
{
    /// <summary>
    /// Interaction logic for QualityDialog.xaml
    /// </summary>
    public partial class QualityDialog : Window
    {
        public int Quality { get; set; } = 50;

        public QualityDialog()
        {
            InitializeComponent();
            QualityTextBox.Text = Quality.ToString();
            QualityTextBox.Focus();
            QualityTextBox.CaretIndex = QualityTextBox.Text.Length;

        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            int quality = -1;
            bool parseSuceed = int.TryParse(QualityTextBox.Text, out quality);
            if (!parseSuceed || quality <0 || quality > 100)
            {
                System.Windows.MessageBox.Show("Wprowadź liczbę w zakresie od 0 do 100", "Nieprawidłowa wartość");
            }
            else
            {
                this.Quality = quality;
                DialogResult = true;
            }
        }

        private void QualityTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OkButtonClick(this, new RoutedEventArgs());
            }
        }
    }
}
