using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Samples.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using GeoJSON;
using GeoJSON.Net.Geometry;
using GeoJSON.Net.Converters;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoClasses;


namespace Mapsui.Samples.Wpf
{
    class MyLayer
    {
        public static ILayer CreateLayerWithDataSourceWithWGS84Point()
        {
            
            DataBase db = new DataBase();
            List<Entity> AllList = db.SearchAll();
            List<Mapsui.Geometries.Point> l = new List<Mapsui.Geometries.Point>();
            int count = AllList.Count;
            var memoryProvider = new MemoryProvider(l) { CRS = "EPSG:4326" };

            for (int i = 0; i < count; i++)
            {

                double y = Convert.ToDouble(AllList.ElementAt(i).Position.Coordinates.Latitude);
                double x = Convert.ToDouble(AllList.ElementAt(i).Position.Coordinates.Longitude);
                string label = Convert.ToString(AllList.ElementAt(i).Value);

                Mapsui.Geometries.Point p = new Mapsui.Geometries.Point(x,y);

                // Colored point feature
                var feat = new Feature { Geometry = new Mapsui.Geometries.Point(x, y) };
                feat.Styles.Add(StyleSamples.CreateMyColoredLabelStyle(label));
                feat["Latitude"] = y;
                feat["Longitude"] = x;
                memoryProvider.Features.Add(feat);

                // Coordinates feature (for showing them when clicking on the point)
                //var featureCoordinates = new Feature { Geometry = new Mapsui.Geometries.Point(x, y) };
                //featureCoordinates["Coordinates"] = x;
                //memoryProvider.Features.Add(featureCoordinates);

                l.Add(p);
            }

            // Returns the layer to be added into the main program
            return new Layer { Name = "WGS84 Points", DataSource = memoryProvider };
        }
    }
}
