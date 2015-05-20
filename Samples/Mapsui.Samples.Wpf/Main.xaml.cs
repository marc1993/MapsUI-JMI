using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace Mapsui.Samples.Wpf
{
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            MessageBox.Show("Welcome!");
        }

        private void Map_Button_Click(object sender, RoutedEventArgs e)
        {
            Window1 mapsform = new Window1();
            mapsform.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
