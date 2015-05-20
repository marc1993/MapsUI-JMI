﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using BruTile.Extensions;
using Mapsui.Geometries;
using Mapsui.Utilities;

namespace Mapsui.Providers.ArcGIS.Dynamic
{
    public class ArcGISDynamicProvider : IProjectingProvider
    {
        private int _timeOut;
        private string _url;
        private string _crs;

        /// <summary>
        /// Create ArcGisDynamicProvider based on a given capabilities file
        /// </summary>
        /// <param name="url">url to map service example: http://url/arcgis/rest/services/test/MapServer</param>
        /// <param name="arcGisDynamicCapabilities"></param>
        public ArcGISDynamicProvider(string url, ArcGISDynamicCapabilities arcGisDynamicCapabilities)
        {
            Url = url;
            ArcGisDynamicCapabilities = arcGisDynamicCapabilities;
            _timeOut = 10000;            
        }

        /// <summary>
        /// Create ArcGisDynamicProvider, capabilities will be parsed automatically
        /// </summary>
        /// <param name="url">url to map service example: http://url/arcgis/rest/services/test/MapServer</param>        
        public ArcGISDynamicProvider(string url)
        {
            Url = url;

            ArcGisDynamicCapabilities = new ArcGISDynamicCapabilities
            {
                fullExtent = new Extent { xmin = 0, xmax = 0, ymin = 0, ymax = 0 },
                initialExtent = new Extent { xmin = 0, xmax = 0, ymin = 0, ymax = 0 }
            };

            var capabilitiesHelper = new CapabilitiesHelper();
            capabilitiesHelper.CapabilitiesReceived += CapabilitiesHelperCapabilitiesReceived;
            capabilitiesHelper.CapabilitiesFailed += CapabilitiesHelperCapabilitiesFailed;
            capabilitiesHelper.GetCapabilities(url, CapabilitiesType.DynamicServiceCapabilities);

            _timeOut = 10000;
        }

        public ArcGISDynamicCapabilities ArcGisDynamicCapabilities { get; private set; }
        public ICredentials Credentials { get; set; }

        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                if (!string.IsNullOrEmpty(value) && value[value.Length - 1].Equals('/'))
                    _url = value.Remove(value.Length - 1);
            }
        }

        /// <summary>
        /// Timeout of webrequest in milliseconds. Default is 10 seconds
        /// </summary>
        public int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        public string CRS
        {
            get {  return _crs; }
            set { _crs = value; }
        }

        public IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            //If there are no layers (probably not initialised) return nothing
            if (ArcGisDynamicCapabilities.layers == null)
                return new Features();

            IFeatures features = new Features();
            IRaster raster = null;
            IViewport viewport = new Viewport { Resolution = resolution, Center = box.GetCentroid(), Width = (box.Width / resolution), Height = (box.Height / resolution) };
            if (TryGetMap(viewport, ref raster))
            {
                var feature = features.New();
                feature.Geometry = raster;
                features.Add(feature);
            }
            return features;
        }

        public BoundingBox GetExtents()
        {
            return new BoundingBox(ArcGisDynamicCapabilities.initialExtent.xmin, ArcGisDynamicCapabilities.initialExtent.ymin, ArcGisDynamicCapabilities.initialExtent.xmax, ArcGisDynamicCapabilities.initialExtent.ymax);
        }

        private void CapabilitiesHelperCapabilitiesFailed(object sender, EventArgs e)
        {
            Debug.WriteLine("Error getting ArcGIS Capabilities");
        }
        
        private void CapabilitiesHelperCapabilitiesReceived(object sender, EventArgs e)
        {
            var capabilities = sender as ArcGISDynamicCapabilities;
            if (capabilities == null)
                return;

            ArcGisDynamicCapabilities = capabilities;
        }

        /// <summary>
        /// Retrieves the bitmap from ArcGIS Dynamic service
        /// </summary>
        public bool TryGetMap(IViewport viewport, ref IRaster raster)
        {
            int width;
            int height;

            try
            {
                width = Convert.ToInt32(viewport.Width);
                height = Convert.ToInt32(viewport.Height);
            }
            catch (OverflowException)
            {
                Debug.WriteLine("Could not conver double to int (ExportMap size)");
                return false;
            }
           
            var uri = new Uri(GetRequestUrl(viewport.Extent, width, height));
            var request = (HttpWebRequest)WebRequest.Create(uri);
            if (Credentials == null)
                request.UseDefaultCredentials = true;
            else
                request.Credentials = Credentials;

            try
            {
                var myWebResponse = request.GetSyncResponse(_timeOut);
                var dataStream = myWebResponse.GetResponseStream();

                var bytes = BruTile.Utilities.ReadFully(myWebResponse.GetResponseStream());
                raster = new Raster(new MemoryStream(bytes), viewport.Extent);
                if (dataStream != null) dataStream.Dispose();

                myWebResponse.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the URL for a map export request base on current settings, the image size and boundingbox
        /// </summary>
        /// <param name="box">Area the request should cover</param>
        /// <param name="width"> </param>
        /// <param name="height"> </param>
        /// <returns>URL for ArcGIS Dynamic request</returns>
        public string GetRequestUrl(BoundingBox box, int width, int height)
        {
            //ArcGIS Export description see: http://resources.esri.com/help/9.3/arcgisserver/apis/rest/index.html?export.html
            
            var sr = CreateSr(CRS);
            var strReq = new StringBuilder(_url);
            strReq.Append("/export?");
            strReq.AppendFormat(CultureInfo.InvariantCulture, "bbox={0},{1},{2},{3}", box.Min.X, box.Min.Y, box.Max.X, box.Max.Y);
            strReq.AppendFormat("&bboxSR={0}", sr);
            strReq.AppendFormat("&imageSR={0}", sr);
            strReq.AppendFormat("&size={0},{1}", width, height);
            strReq.Append("&layers=show:");

            /* 
             * Add all layers to the request that have defaultVisibility to true, the normal request to ArcGIS allready does this already
             * without specifying "layers=show", but this adds the opportunity for the user to set the defaultVisibility of layers
             * to false in the capabilities so different views (layers) can be created for one service
             */
            var oneAdded = false;

            foreach (var t in ArcGisDynamicCapabilities.layers)
            {
                if (t.defaultVisibility == false)
                    continue;

                if (oneAdded)
                    strReq.Append(",");

                strReq.AppendFormat("{0}", t.id);
                oneAdded = true;
            }
           
            strReq.AppendFormat("&format={0}", GetFormat(ArcGisDynamicCapabilities));
            strReq.Append("&transparent=true");
            strReq.Append("&f=image");

            return strReq.ToString();
        }

        private static string CreateSr(string crs)
        {
            if (crs.StartsWith(ProjectionHelper.EsriStringPrefix)) return "{\"wkt\":\"" + crs.Substring(ProjectionHelper.EsriStringPrefix.Length).Replace("\"", "\\\"") + "\"}";
            if (crs.StartsWith(ProjectionHelper.EpsgPrefix)) return ProjectionHelper.ToEpsgCode(crs).ToString();
            throw new Exception("crs type not supported");
        }

        private static string GetFormat(ArcGISDynamicCapabilities arcGisDynamicCapabilities)
        {
            //png | png8 | png24 | jpg | pdf | bmp | gif | svg | png32 (png32 only supported from 9.3.1 and up)
            if (arcGisDynamicCapabilities.supportedImageFormatTypes == null)//Not all services return supported types, use png
                return "png";

            var supportedTypes = arcGisDynamicCapabilities.supportedImageFormatTypes.ToLower();

            if (supportedTypes.Contains("png32"))
                return "png32";
            if (supportedTypes.Contains("png24"))
                return "png24";
            if (supportedTypes.Contains("png8"))
                return "png8";
            if (supportedTypes.Contains("png"))
                return "png";

            return "jpg";
        }

        public bool? IsCrsSupported(string crs)
        {
            return true; // for now assuming ArcGISServer supports all CRSes 
        }
    }
}
