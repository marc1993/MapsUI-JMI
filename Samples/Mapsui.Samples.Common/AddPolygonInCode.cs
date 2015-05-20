﻿using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;

namespace Mapsui.Samples.Common
{
    public static class CreatePolygonSample
    {
        public static ILayer AddLayerWithOnePolygon()
        {
            return new Layer("LayerWithPolygon")
                {
                    DataSource = new MemoryProvider(CreatePolygon())
                };
        }

        private static Polygon CreatePolygon()
        {
            var polygon = new Polygon();
            polygon.ExteriorRing.Vertices.Add(new Point(0, 0));
            polygon.ExteriorRing.Vertices.Add(new Point(0, 1000000));
            polygon.ExteriorRing.Vertices.Add(new Point(1000000, 1000000));
            polygon.ExteriorRing.Vertices.Add(new Point(1000000, 0));
            polygon.ExteriorRing.Vertices.Add(new Point(0, 0));
            return polygon;
        }
    }
}
