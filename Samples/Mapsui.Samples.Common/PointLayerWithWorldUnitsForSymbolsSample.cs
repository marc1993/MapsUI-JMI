using Mapsui.Geometries;
using Mapsui.Providers;
using Mapsui.Styles;
using System.IO;


namespace Mapsui.Samples.Common
{
    public static class PointLayerWithWorldUnitsForSymbolsSample
    {
        public static Feature Create(double lat, double lon)
        {

            var feat = new Feature { Geometry = new Point(lon, lat) };

            //const string location = @".\Resources\example.tif";
            const string resource = "Mapsui.Samples.Common.Images.netherlands.jpg";
            //var bitmapDataStream = File.OpenRead(@"c:\foto.jpg");
            var bitmapDataStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
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
