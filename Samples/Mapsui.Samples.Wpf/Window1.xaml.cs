using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Diagnostics;
using BruTile.Extensions;
using BruTile.Predefined;
using BruTile.Wmts;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Samples.Common;
using Mapsui.Samples.Common.Desktop;
using Mapsui.Samples.Wpf;
using Mapsui.Styles;
using Mapsui.UI.Xaml;
using MongoClasses;

namespace Mapsui.Samples.Wpf
{
    public partial class Window1
    {
        public Window1()
        {
            InitializeComponent();
            MapControl.ErrorMessageChanged += MapErrorMessageChanged;
            MapControl.FeatureInfo += MapControlFeatureInfo;
            Fps.SetBinding(TextBlock.TextProperty, new Binding("Fps"));
            Fps.DataContext = MapControl.FpsCounter;
            
            OsmClick(this, null);

            // Checking if MongoDB process is running. If not, it executes it.
            Process[] mongod = Process.GetProcessesByName("mongod");
            if (mongod.Length == 0)
            {
                Process.Start(@"C:\Program Files\MongoDB\Server\3.0\bin\mongod.exe");
                Process.Start(@"C:\Program Files\MongoDB\Server\3.0\bin\mongo.exe");
                MessageBox.Show("MongoDB was not executed and has been started.");
            }

        }

        static void MapControlFeatureInfo(object sender, FeatureInfoEventArgs e)
        {
            MessageBox.Show(FeaturesToString(e.FeatureInfo));
        }

        static string FeaturesToString(IEnumerable<KeyValuePair<string, IEnumerable<IFeature>>> featureInfos)
        {
            var result = string.Empty;

            foreach (var layer in featureInfos)
            {
                result += layer.Key + "\n";
                foreach (var feature in layer.Value)
                {
                    foreach (var field in feature.Fields)
                    {
                        result += field + ":" + feature[field] + ".";
                    }
                    result += "\n";
                }
                result += "\n";
            }
            return result;
        }

        private void MapErrorMessageChanged(object sender, EventArgs e)
        {
            Error.Text = MapControl.ErrorMessage;
            Utilities.AnimateOpacity(ErrorBorder, 0.75, 0, 8000);
        }

        private void OsmClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()) { Name = "OSM" });
            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.ZoomToFullEnvelope();
            MapControl.Refresh();
        }

        private void ProjectedPointClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Transformation = new MinimalTransformation();
            MapControl.Map.CRS = "EPSG:3857";
            MapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()) { Name = "OSM" });
            MapControl.Map.Layers.Add(PointLayerSample.CreateLayerWithDataSourceWithWGS84Point());
            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.ZoomToFullEnvelope();
            MapControl.Refresh();
        }

        private void AnimatedPointsClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()) { Name = "OSM" });
            MapControl.Map.Layers.Add(new AnimatedPointsWithAutoUpdateLayer { Name = "AnimatedLayer" });

            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.ZoomToFullEnvelope();
            MapControl.Refresh();
        }

        private void RandomPointWithStackLabelClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()) { Name = "OSM" });
            var provider = CreateRandomPointsProvider();
            MapControl.Map.Layers.Add(PointLayerSample.CreateStackedLabelLayer(provider));
            MapControl.Map.Layers.Add(PointLayerSample.CreateRandomPointLayer(provider));
            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.ZoomToFullEnvelope();
            MapControl.Refresh();
        }

        private void RandomPointsWithFeatureInfoClick(object server, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()) { Name = "OSM" });
            var pointLayer = PointLayerSample.CreateRandomPointLayer(CreateRandomPointsProvider());
            pointLayer.Style = new StyleCollection {
                new SymbolStyle {
                        SymbolScale = 1, Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.White), 
                        Outline = { Color = Mapsui.Styles.Color.Black, Width = 1}}
                };
            MapControl.Map.Layers.Add(pointLayer);
            MapControl.MouseInfoUp += MapControlOnMouseInfoDown;
            MapControl.MouseInfoUpLayers.Add(pointLayer);
            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.ZoomToFullEnvelope();
            MapControl.Refresh();
        }

        private static void MapControlOnMouseInfoDown(object sender, MouseInfoEventArgs mouseInfoEventArgs)
        {
            if (mouseInfoEventArgs.Feature != null)
            {
                MessageBox.Show(mouseInfoEventArgs.Feature["Label"].ToString());
            }
        }
        private static void MapControlOnMouseInfoDown2(object sender, MouseInfoEventArgs mouseInfoEventArgs)
        {
            if (mouseInfoEventArgs.Feature != null)
            {
                double lat = Convert.ToDouble(mouseInfoEventArgs.Feature["Latitude"]);
                double lon = Convert.ToDouble(mouseInfoEventArgs.Feature["Longitude"]);
                pointSettings newSettings = new pointSettings();
                newSettings.SetData(lat, lon);
                newSettings.ShowDialog();
                //MessageBox.Show(mouseInfoEventArgs.Feature.Geometry.ToString());               
            }
        }


        private MemoryProvider CreateRandomPointsProvider()
        {
            var randomPoints = PointLayerSample.GenerateRandomPoints(MapControl.Map.Envelope, 100);
            var features = new Features();
            var count = 0;
            foreach (var point in randomPoints)
            {
                var feature = new Feature { Geometry = point };
                feature["Label"] = count.ToString(CultureInfo.InvariantCulture);
                features.Add(feature);
                count++;
            }
            return new MemoryProvider(features);
        }

        private void GeodanWmsClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Layers.Add(new TileLayer(new GeodanWorldWmsTileSource()) { Name = "WMS called as WMSC"});
            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.ZoomToFullEnvelope();
            MapControl.Refresh();
        }

        private void GeodanTmsClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Layers.Add(new TileLayer(
                    () => TmsTileSourceBuilder.Build("http://geoserver.nl/tiles/tilecache.aspx/1.0.0/worlddark_GM", true))
                    { Name = "TMS"});
            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.Refresh();
        }

        private void BingMapsClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create(KnownTileSource.BingAerial)) 
                { Name = "Bing Aerial"});
            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.ZoomToFullEnvelope();
            MapControl.Refresh();
        }

        private void GeodanWmscClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Layers.Add(new TileLayer(new GeodanWorldWmsCTileSource()));
            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.Refresh();
        }

        private void ShapefileClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            foreach (var layer in ShapefileSample.CreateLayers())
            {
                MapControl.Map.Layers.Add(layer);
            }
            MapControl.ZoomToFullEnvelope();
            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.Refresh();
        }

        private void MapTilerClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Layers.Add(new TileLayer(new MapTilerTileSource()){Name = "True Marble in MapTiler"} );
            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.ZoomToFullEnvelope();
            MapControl.Refresh();
        }

        private void PointSymbolsClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Transformation = new MinimalTransformation();
            MapControl.Map.CRS = "EPSG:3857";
            MapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()) { Name = "OSM" });
            MapControl.Map.Layers.Add(PointLayerSample.Create());
            MapControl.Map.Layers.Add(CreatePointLayerWithWorldUnitSymbol());
            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.Refresh();
        }

        private static ILayer CreatePointLayerWithWorldUnitSymbol()
        {
            // Initializes the db and gets all the logs
            DataBase db = new DataBase();
            List<Entity> AllList = db.SearchAll();
            int count = AllList.Count;

            //System.Drawing.Bitmap image = new System.Drawing.Bitmap(@"C:\Papadetes\13052015180915.jpg");

            // Starts a memory provider
            var memoryProvider = new MemoryProvider { CRS = "EPSG:4326" };
            for (int i = 0; i < count; i++) // For each log, a feature is created through the function and added to the memory provider
            {
                double y = Convert.ToDouble(AllList.ElementAt(i).Position.Coordinates.Latitude);
                double x = Convert.ToDouble(AllList.ElementAt(i).Position.Coordinates.Longitude);
                Int64 name = Convert.ToInt64(AllList.ElementAt(i).ImageName);
                //Int64 realName = name % 100000000;
                Feature feat = PointLayerWithWorldUnitsForSymbolsSample2.Create(y, x, name); // Function to create the feature
                memoryProvider.Features.Add(feat);
            }
            return new Layer { Name = "PointLayer WorldUnits", DataSource = memoryProvider }; // Layer is created with the memory provider

        }

        private void WmsClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Layers.Add(WmsSample.Create());
            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.ZoomToFullEnvelope();
            MapControl.Refresh();
        }

        private void ArcGISImageServiceClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Layers.Add(ArcGISImageServiceSample.CreateLayer());
            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.ZoomToFullEnvelope();
            MapControl.Refresh();
        }

        private void WmtsClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            var webRequest = (HttpWebRequest)WebRequest.Create("http://geodata.nationaalgeoregister.nl/wmts/top10nl?VERSION=1.0.0&request=GetCapabilities");
            WebResponse webResponse = webRequest.GetSyncResponse(10000);
            if (webResponse == null) throw (new WebException("An error occurred while fetching tile", null));
            using (var responseStream = webResponse.GetResponseStream())
            {
                var tileSources = WmtsParser.Parse(responseStream);
                var natura2000 = tileSources.First(t => t.Name.ToLower().Contains("natura2000"));
                MapControl.Map.Layers.Add(new TileLayer(natura2000) { Name = "Natura 2000"});
                MapControl.ZoomToFullEnvelope();
                MapControl.Refresh();
                LayerList.Initialize(MapControl.Map.Layers);
            }
        }

        private void PointsWithLabelsClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()) { Name = "OSM" });
            MapControl.Map.Layers.Add(PointLayerSample.CreatePointLayerWithLabels());
            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.ZoomToFullEnvelope();
            MapControl.Refresh();
        }

        private void RotationSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var percent = RotationSlider.Value / (RotationSlider.Maximum - RotationSlider.Minimum);
            MapControl.Map.Viewport.Rotation = percent * 360;
            MapControl.Refresh();
        }

        private void MYPOINTSClick(object sender, RoutedEventArgs e)
        {
            MapControl.Map.Layers.Clear();
            MapControl.Map.Transformation = new MinimalTransformation();
            MapControl.Map.CRS = "EPSG:3857";
            MapControl.Map.Layers.Add(new TileLayer(KnownTileSources.Create()) { Name = "OSM" });
            ILayer layer = MyLayer.CreateLayerWithDataSourceWithWGS84Point();
            MapControl.Map.Layers.Add(layer);

            MapControl.MouseInfoUp += MapControlOnMouseInfoDown2;
            MapControl.MouseInfoUpLayers.Add(layer);

            LayerList.Initialize(MapControl.Map.Layers);
            MapControl.ZoomToFullEnvelope();
            MapControl.Refresh();
        }
    }
}