using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Command;
using IndoorNavigationApp.Models;

namespace IndoorNavigationApp.Controls
{
    public sealed partial class MapNodeControl : UserControl
    {
        public Node Node
        {
            get { return (Node)GetValue(NodeProperty); }
            set { SetValue(NodeProperty, value); }
        }
        
        public static readonly DependencyProperty NodeProperty =
            DependencyProperty.Register("Node", typeof(Node), typeof(MapNodeControl), new PropertyMetadata(null, NodeChangedCallback));




        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(MapNodeControl), new PropertyMetadata(null));




        public MapNodeControl()
        {
            InitializeComponent();
        }

        private static void NodeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = (MapNodeControl) d;
            sender.OnNodeChanged();
        }

        private void OnNodeChanged()
        {
            textPresenter.Text = string.Empty;
            imagePresenter.UriSource = null;
            
            if (Node == null)
            {
                return;
            }

            Padding = new Thickness(Node.Position.X, Node.Position.Y, 0, 0);

            if (Node.Type == NodeType.GenericRoom)
            {
                textPresenter.Text = Node.Title;
                return;
            }

            if (Node.Type == NodeType.NavHint)
            {
                navHintEllipse.Visibility = Visibility.Visible;
                return;
            }

            string iconName;
            switch (Node.Type)
            {
                case NodeType.WC:
                    iconName = "wc.png";
                    break;
                case NodeType.Elevator:
                    iconName = "elevator.png";
                    break;
                case NodeType.Stairs:
                    iconName = "stairs.png";
                    break;
                case NodeType.Library:
                    iconName = "library.png";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            imagePresenter.UriSource = new Uri($"ms-appx:/Assets/NodeIcons/{iconName}");
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (Command != null)
            {
                Command.Execute(Node);
            }
        }
    }
}
