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

namespace Mapsui.Samples.Wpf
{
    /// <summary>
    /// Lógica de interacción para pointSettings.xaml
    /// </summary>
    public partial class pointSettings : Window
    {

        double Latitude;
        double Longitude;


        public pointSettings()
        {
            InitializeComponent();
        }

        private void settingsInfo_Loaded(object sender, RoutedEventArgs e) // Information loaded when the form is first opened
        {
            LatBox.Text = Convert.ToString(Latitude);
            LongBox.Text = Convert.ToString(Longitude);
        }

        private void showImage_Click(object sender, RoutedEventArgs e) // Event that triggers when the user clicks on the image button
        {
            imageCharged newPhoto = new imageCharged();
            newPhoto.ShowDialog();
        }

        public void SetData(double lat, double lon) // Sets the information needed to show the information (and the image if needed)
        {
            this.Latitude = lat;
            this.Longitude = lon;
        }
    }
}
