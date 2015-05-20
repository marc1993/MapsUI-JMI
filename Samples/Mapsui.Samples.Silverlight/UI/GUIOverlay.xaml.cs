﻿using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Samples.Common;
using Mapsui.Styles;
using Mapsui.UI.Xaml;
using Mapsui.UI.Xaml.Layers;

namespace Mapsui.Samples.Silverlight.UI
{
    public partial class GUIOverlay : UserControl
    {
        public MapControl _mapControl; //todo: remove
        bool _isMenuDown;

        public GUIOverlay()
        {
            InitializeComponent();
            hideMenu.Completed += hideMenu_Completed;
            showMenu.Completed += showMenu_Completed;
            Loaded += GUIOverlay_Loaded;
            SizeChanged += GUIOverlay_SizeChanged;
        }

        internal void SetMap(MapControl mapControl)
        {
            _mapControl = mapControl;
            mapControl.ErrorMessageChanged += map_ErrorMessageChanged;

            mapControl.Map = CreateMap();

            FillLayerList(mapControl.Map);
            if (mapControl.Map.Envelope == null) return;

            var center = mapControl.Map.Envelope.GetCentroid();
            mapControl.Map.Viewport.Center = new Geometries.Point(center.X, center.Y);
            mapControl.Map.Viewport.Resolution = 10000;
        }

        private static Map CreateMap()
        {
            var bitmapData = System.Reflection.Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Mapsui.Samples.Silverlight.UI.Images.btnBbox.png");

            var osmLayer = new TileLayer(KnownTileSources.Create()) {Name = "OSM"};
            var provider = CreateRandomPointsProvider(osmLayer.Envelope);
            
            var map = new Map();
            map.Layers.Add(osmLayer);
            // map.Layers.Add(CreateRandomPointLayer(provider, bitmapData));
            map.Layers.Add(new RasterizingLayer(PointLayerSample.CreateRandomPointLayerWithLabel(provider)));
            return map;
        }

        private static MemoryProvider CreateRandomPointsProvider(BoundingBox box)
        {
            var randomPoints = PointLayerSample.GenerateRandomPoints(box, 200);
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
        void FillLayerList(Map map)
        {
            var random = new Random(DateTime.Now.Second);

            bool firstButton = true;

            foreach (var layer in map.Layers)
            {
                if (layer is GroupTileLayer)
                {
                    foreach (ILayer subLayer in (layer as GroupTileLayer).Layers)
                    {
                        var checkBox = new CheckBox
                            {
                                Name = random.Next().ToString(CultureInfo.InvariantCulture),
                                Content = subLayer.Name,
                                Tag = subLayer,
                                Margin = new Thickness(4),
                                FontSize = 12,
                                IsChecked = true
                            };

                        checkBox.Click += checkBox_Click;

                        layerList.Children.Add(checkBox);

                        if (!firstButton) continue;
                        checkBox.IsChecked = true;
                        firstButton = false;
                    }
                }
            }
        }

        void checkBox_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement)) return;
            var layer = ((sender as FrameworkElement).Tag as ILayer);
            layer.Enabled = !layer.Enabled;
            _mapControl.Clear();
            _mapControl.OnViewChanged();
        }

        void map_ErrorMessageChanged(object sender, EventArgs e)
        {
            Error.Text = _mapControl.ErrorMessage;
            AnimateOpacity(errorBorder, 0.75, 0, 8000);
        }

        public static void AnimateOpacity(UIElement target, double from, double to, int duration)
        {
            target.Opacity = 0;
            var animation = new DoubleAnimation();
            animation.From = from;
            animation.To = to;
            animation.Duration = new TimeSpan(0, 0, 0, 0, duration);

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            var storyBoard = new Storyboard();
            storyBoard.Children.Add(animation);
            storyBoard.Begin();
        }

        void SetClip()
        {
            var geom = new RectangleGeometry();
            geom.Rect = new Rect(0, 0, ActualWidth, ActualHeight);
            Clip = geom;
        }

        

        
        
        void showMenu_Completed(object sender, EventArgs e)
        {
            _isMenuDown = true;
        }

        private void showBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            menuShowHideOn.Visibility = Visibility.Collapsed;
        }

        private void showBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            menuShowHideOn.Visibility = Visibility.Visible;
        }

        private void ShowMenuStart()
        {
            showBtn.Visibility = Visibility.Collapsed;
            showMenu.Begin();
        }

        private void showBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowMenuStart();
        }

        void hideMenu_Completed(object sender, EventArgs e)
        {
            menuShowHideOn.Visibility = Visibility.Visible;
            showBtn.Visibility = Visibility.Visible;
            _isMenuDown = false;
        }

        private void hideBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            menuShowHideOff.Visibility = Visibility.Collapsed;
        }

        private void hideBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            menuShowHideOff.Visibility = Visibility.Visible;
        }

        private void hideBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            hideMenu.Begin();
        }
        
        void GUIOverlay_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetClip();
        }

        void GUIOverlay_Loaded(object sender, RoutedEventArgs e)
        {
            SetClip();
            GoTo.SetGui(this);
        }

        private void buttonZoomIn_Click(object sender, RoutedEventArgs e)
        {
            _mapControl.ZoomIn();
        }

        private void buttonZoomOut_Click(object sender, RoutedEventArgs e)
        {
            _mapControl.ZoomOut();
        }

        private void btnLayers_Click(object sender, RoutedEventArgs e)
        {
            if (!_isMenuDown)
                ShowMenuStart();
        }

        private void btnFullscreen_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
        }

        private void btnBbox_Click(object sender, RoutedEventArgs e)
        {
            _mapControl.ZoomToBoxMode = true;
        }

        private void buttonMaxExtend_Click(object sender, RoutedEventArgs e)
        {
            var extent = _mapControl.Map.Envelope;
            _mapControl.ZoomToBox(new Mapsui.Geometries.Point(extent.MinX, extent.MinY), new Mapsui.Geometries.Point(extent.MaxX, extent.MaxY));
        }

        private void btnGoto_Click(object sender, RoutedEventArgs e)
        {
            GoTo.Visibility = Visibility.Visible;
            GoTo.ShowGoTo.Begin();

            if (Application.Current.Host.Content.IsFullScreen)
            {
                GoTo.errorGrid.Visibility = Visibility.Visible;
            }
            else
            {
                GoTo.errorGrid.Visibility = Visibility.Collapsed;
            }
        }

        
        
        private void buttonZoomIn_MouseEnter(object sender, MouseEventArgs e)
        {
            txtTooltipTop.Text = "Zoom In";
            ShowTopTooltip.Begin();
        }

        private void buttonZoomIn_MouseLeave(object sender, MouseEventArgs e)
        {
            HideTopTooltip.Begin();
        }

        private void buttonZoomOut_MouseEnter(object sender, MouseEventArgs e)
        {
            txtTooltipTop.Text = "Zoom Out";
            ShowTopTooltip.Begin();
        }

        private void buttonZoomOut_MouseLeave(object sender, MouseEventArgs e)
        {
            HideTopTooltip.Begin();
        }

        private void buttonMaxExtend_MouseEnter(object sender, MouseEventArgs e)
        {
            txtTooltipTop.Text = "Max Extend";
            ShowTopTooltip.Begin();
        }

        private void buttonMaxExtend_MouseLeave(object sender, MouseEventArgs e)
        {
            HideTopTooltip.Begin();
        }

        
        
        private void btnLayers_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowLowerTooltip.Begin();
            txtTooltipBottom.Text = "Layers";
        }

        private void btnFullscreen_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowLowerTooltip.Begin();
            txtTooltipBottom.Text = "Fullscreen";
        }

        private void btnBbox_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowLowerTooltip.Begin();
            txtTooltipBottom.Text = "bbox zoom";
        }

        private void btnHand_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowLowerTooltip.Begin();
            txtTooltipBottom.Text = "Pan";
        }

        private void btnGoto_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowLowerTooltip.Begin();
            txtTooltipBottom.Text = "Go To";
        }

        private void lower_MouseLeave(object sender, MouseEventArgs e)
        {
            HideLowerTooltip.Begin();
        }

            }
}