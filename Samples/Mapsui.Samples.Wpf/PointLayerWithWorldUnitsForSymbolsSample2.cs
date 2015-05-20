using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Mapsui.Providers;
using Mapsui.Geometries;
using System.Drawing;
using Mapsui.Styles;

namespace Mapsui.Samples.Wpf
{
    public static class PointLayerWithWorldUnitsForSymbolsSample2
    {
        public static Feature Create(double lat, double lon, Int64 imageName)
        {

            var feat = new Feature { Geometry = new Mapsui.Geometries.Point(lon, lat) };

            //const string location = @".\Resources\example.tif";
            //const string resource = "Mapsui.Samples.Common.Images.netherlands.jpg";
            //13052015180915
            string name = Convert.ToString(imageName);
            string filePath = @"C:\Papadetes\" + name + ".jpg";
            var bitmapDataStream = File.OpenRead(filePath);
            //var bitmapDataStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            feat.Styles.Add(new SymbolStyle
            {
                BitmapId = BitmapRegistry.Instance.Register(bitmapDataStream),
                SymbolType = SymbolType.Rectangle,
                UnitType = UnitType.WorldUnit,
                SymbolRotation = 0f,
                SymbolScale = 150,
            });

            return feat;
        }
    }
}
