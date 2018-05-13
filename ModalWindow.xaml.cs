using System;
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
using System.Windows.Shapes;

namespace eTracker
{
    /// <summary>
    /// Interaction logic for ModalWindow.xaml
    /// </summary>
    public partial class ModalWindow : Window
    {
        public bool Started = false;
        public ModalWindow()
        {
            InitializeComponent();
            var age = Enumerable.Range(6, 80).ToList();
            var l1s = Translator.Languages.ToList();
            l1s.Remove("English");

            AgeCombo.ItemsSource = age;
            L1Combo.ItemsSource = l1s;
            AgeCombo.SelectedItem = age.ToList().First();
            L1Combo.SelectedItem = "Greek";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Started = true;
            Close();
        }
    }
}
