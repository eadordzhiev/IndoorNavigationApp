using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using GalaSoft.MvvmLight.Command;
using IndoorNavigationApp.Models;
using IndoorNavigationApp.Service;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Newtonsoft.Json;

namespace IndoorNavigationApp.Controls
{
    public sealed partial class MapPresenterControl : UserControl
    {
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }
        
        public static readonly DependencyProperty MapProperty =
            DependencyProperty.Register("Map", typeof(Map), typeof(MapPresenterControl), new PropertyMetadata(null, MapChangedCallback));



        public PointU? CurrentLocation
        {
            get { return (PointU?)GetValue(CurrentLocationProperty); }
            set { SetValue(CurrentLocationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentLocation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentLocationProperty =
            DependencyProperty.Register("CurrentLocation", typeof(PointU?), typeof(MapPresenterControl), new PropertyMetadata(null, CurrentLocationChangedCallback));

        private static void CurrentLocationChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var mapPresenter = (MapPresenterControl)sender;
            mapPresenter.OnCurrentLocationChanged(sender, args);
        }

        private void OnCurrentLocationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (CurrentLocation != null)
            {
                mePoiControl.Offset(
                    offsetX: CurrentLocation.Value.X,
                    offsetY: CurrentLocation.Value.Y,
                    duration: args.OldValue != null ? 500 : 0)
                    .Start();
                mePoiControl.Fade(1).Start();
            }
            else
            {
                mePoiControl.Fade(0).Start();
            }
        }

        public MapPresenterControl()
        {
            InitializeComponent();
        }

        private static void MapChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var mapPresenter = (MapPresenterControl) sender;
            mapPresenter.OnMapChanged(sender, args);
        }

        private void OnMapChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            tileCanvas.Children.Clear();
            nodeCanvas.Children.Clear();

            if (Map == null)
            {
                return;
            }

            CreateTiles(Map.TileProvider);

            var zoomFactorBinding = new Binding
            {
                Source = scrollViewer,
                Path = new PropertyPath("ZoomFactor")
            };

            foreach (var node in Map.Nodes)
            {
                var nodeView = new MapNodeControl();
                nodeView.Node = node;
                nodeView.Command = new RelayCommand<Node>(NodeClicked);
                
                var nodeViewWrapper = new MapElementAdorner();
                nodeViewWrapper.Child = nodeView;
                nodeViewWrapper.SetBinding(MapElementAdorner.ZoomFactorProperty, zoomFactorBinding);
                Canvas.SetLeft(nodeViewWrapper, node.Position.X);
                Canvas.SetTop(nodeViewWrapper, node.Position.Y);
                
                nodeCanvas.Children.Add(nodeViewWrapper);
            }
        }

        private Node startingNode;
        private List<int[]> edges = new List<int[]>();

        private void NodeClicked(Node node)
        {
            if (startingNode == null)
            {
                startingNode = node;
                return;
            }

            edges.Add(new int[]{startingNode.AdjacencyMatrixId, node.AdjacencyMatrixId});
            var line = new Line();
            line.X1 = startingNode.Position.X;
            line.Y1 = startingNode.Position.Y;
            line.X2 = node.Position.X;
            line.Y2 = node.Position.Y;
            line.Stroke = new SolidColorBrush(Colors.Blue);
            line.StrokeThickness = 3;
            routeCanvas.Children.Add(line);

            startingNode = null;
        }

        string dumpEdges()
        {
            return JsonConvert.SerializeObject(edges);
        }

        private void CreateTiles(ITileProvider tileProvider)
        {
#if DEBUG_TILES
            var random = new Random();
#endif
            for (uint x = 0; x < tileProvider.XCount; x++)
            for (uint y = 0; y < tileProvider.YCount; y++)
            {
                var tileImageSource = new BitmapImage(tileProvider.GetTileUri(x, y));
                var tile = new Border()
                {
                    Width = tileProvider.TileWidth,
                    Height = tileProvider.TileHeight,
                    Child = new Image()
                    {
                        Source = tileImageSource
                    }
                };
                tileImageSource.ImageOpened += (sender, args) =>
                {
#if DEBUG_TILES
                    var randomBytes = new byte[3];
                    random.NextBytes(randomBytes);
                    var randomColor = Color.FromArgb(255, randomBytes[0], randomBytes[1], randomBytes[2]);
                    tile.Background = new SolidColorBrush(randomColor);
#else
                    tile.Background = new SolidColorBrush(Colors.White);
#endif
                };
                Canvas.SetLeft(tile, x * tile.Width);
                Canvas.SetTop(tile, y * tile.Height);
                tileCanvas.Children.Add(tile);
            }
        }
    }
}
