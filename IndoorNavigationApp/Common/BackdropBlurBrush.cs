using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas.Effects;

namespace IndoorNavigationApp.Common
{
    public sealed class BackdropBlurBrush : XamlCompositionBrushBase
    {
        public double BlurAmount
        {
            get { return (double)GetValue(BlurAmountProperty); }
            set { SetValue(BlurAmountProperty, value); }
        }

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public double BackgroundColorOpacity
        {
            get { return (double)GetValue(BackgroundColorOpacityProperty); }
            set { SetValue(BackgroundColorOpacityProperty, value); }
        }

        public static readonly DependencyProperty BlurAmountProperty =
            DependencyProperty.Register("BlurAmount", typeof(double), typeof(BackdropBlurBrush), new PropertyMetadata(0d, OnPropertyChanged));

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(BackdropBlurBrush), new PropertyMetadata(Colors.White, OnPropertyChanged));

        public static readonly DependencyProperty BackgroundColorOpacityProperty =
            DependencyProperty.Register("BackgroundColorOpacity", typeof(double), typeof(BackdropBlurBrush), new PropertyMetadata(1d, OnPropertyChanged));

        protected override void OnConnected()
        {
            if (CompositionBrush != null)
            {
                return;
            }

            var backdropBlurEffect = new GaussianBlurEffect
            {
                Name = "Blur",
                BlurAmount = (float) BlurAmount,
                Source = new CompositionEffectSourceParameter("backdrop")
            };
            var backgroundColorSourceEffect = new ColorSourceEffect
            {
                Name = "Tint",
                Color = BackgroundColor
            };
            var backgroundColorOpacityEffect = new OpacityEffect
            {
                Name = "Opacity",
                Opacity = (float) BackgroundColorOpacity,
                Source = backgroundColorSourceEffect
            };
            var blendEffect = new BlendEffect
            {
                Background = backdropBlurEffect,
                Foreground = backgroundColorOpacityEffect,
                Mode = BlendEffectMode.Multiply,
            };

            var effectFactory = Window.Current.Compositor.CreateEffectFactory(blendEffect, new[] { "Blur.BlurAmount", "Tint.Color", "Opacity.Opacity" });
            var effectBrush = effectFactory.CreateBrush();
                
            effectBrush.SetSourceParameter("backdrop", Window.Current.Compositor.CreateBackdropBrush());

            CompositionBrush = effectBrush;
        }

        protected override void OnDisconnected()
        {
            if (CompositionBrush != null)
            {
                CompositionBrush.Dispose();
                CompositionBrush = null;
            }
        }

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var backdropBlurBrush = (BackdropBlurBrush) sender;
            backdropBlurBrush.CompositionBrush?.Properties.InsertScalar("Blur.BlurAmount", (float) backdropBlurBrush.BlurAmount);
            backdropBlurBrush.CompositionBrush?.Properties.InsertColor("Tint.Color", backdropBlurBrush.BackgroundColor);
            backdropBlurBrush.CompositionBrush?.Properties.InsertScalar("Opacity.Opacity", (float) backdropBlurBrush.BackgroundColorOpacity);
        }
    }
}