using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace IndoorNavigationApp.Controls
{
    [ContentProperty(Name = nameof(Child))]
    public sealed partial class MapElementAdorner : UserControl
    {
        public UIElement Child
        {
            get { return (UIElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }
        
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(UIElement), typeof(MapElementAdorner), new PropertyMetadata(null));
        
        public float ZoomFactor
        {
            get { return (float)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }
        
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(float), typeof(MapElementAdorner), new PropertyMetadata(1f));
        
        public MapElementAdorner()
        {
            this.InitializeComponent();
        }

        public float InvertAndLimitZoomFactor(float factor)
        {
            factor = Math.Max(factor, 0.7f);
            return 1 / factor;
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            SetControlOriginToCenter();
        }

        private void SetControlOriginToCenter()
        {
            translateTransform.X = -ActualWidth / 2;
            translateTransform.Y = -ActualHeight / 2;
        }
    }
}
