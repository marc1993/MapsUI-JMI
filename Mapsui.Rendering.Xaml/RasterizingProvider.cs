﻿using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;

namespace Mapsui.Rendering.Xaml
{
    /// <summary>
    /// Wrapper around a feature provider that returns a rasterized image of the features.
    /// </summary>
    public class RasterizingProvider : IProvider
    {
        private readonly object _syncLock = new object();
        private readonly ILayer _layer;

        public RasterizingProvider(ILayer layer)
        {
            _layer = layer;
        }

        public RasterizingProvider(IProvider provider, IStyle style = null)
        {
            _layer = new MemoryLayer {DataSource = provider, Style = style};
        }

        public string CRS { get; set; }

        public IEnumerable<IFeature> GetFeaturesInView(BoundingBox extent, double resolution)
        {
            lock (_syncLock)
            {
                foreach (var feature in _layer.GetFeaturesInView(extent, resolution)) 
                {
                    // hack: clear cache to prevent cross thread exception. 
                    // todo: remove this caching mechanism.
                    feature.RenderedGeometry.Clear();
                }

                IFeatures features = null;
                var viewport = CreateViewport(extent, resolution);
                RunMethodOnStaThread(() => RenderToRaster(viewport, _layer, out features));
                return features;
            }
        }

        private static void RunMethodOnStaThread(ThreadStart operation)
        {
            var thread = new Thread(operation);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Priority = ThreadPriority.Lowest;
            thread.Start();
            thread.Join();
        }

        public BoundingBox GetExtents()
        {
            return _layer.Envelope;
        }

        private void RenderToRaster(IViewport viewport, ILayer layer, out IFeatures features)
        {
            var canvas = new Canvas();
            MapRenderer.RenderLayer(canvas, viewport, layer);
            canvas.UpdateLayout();
            var bitmap = BitmapRendering.BitmapConverter.ToBitmapStream(canvas, viewport.Width, viewport.Height);
            features = new Features { new Feature { Geometry = new Raster(bitmap, viewport.Extent) } };
        }

        public IProvider DataSource
        {
            get
            {
                var layer = _layer as MemoryLayer;
                return layer != null ? layer.DataSource : null;
            }
        }

        private static Viewport CreateViewport(BoundingBox extent, double resolution)
        {
            return new Viewport
                {
                    Resolution = resolution,
                    Center = extent.GetCentroid(),
                    Width = extent.Width/resolution,
                    Height = extent.Height/resolution
                };
        }
    }
}
