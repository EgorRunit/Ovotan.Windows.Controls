using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Ovotan.Windows.Controls
{
    public class ViewboxIcon : ContentControl
    {
        public static readonly DependencyProperty BaseColorProperty;
        public static readonly DependencyProperty ViewboxProperty;

        public SolidColorBrush BaseColor
        {
            get { return GetValue(BaseColorProperty) as SolidColorBrush; }
            set { SetValue(BaseColorProperty, value); }
        }

        public Viewbox Viewbox
        {
            get { return GetValue(ViewboxProperty) as Viewbox; }
            set { SetValue(ViewboxProperty, value); }
        }

        public ICommand Command { get; set; }

        static ViewboxIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ViewboxIcon), new FrameworkPropertyMetadata(typeof(ContentControl)));

            ViewboxProperty = DependencyProperty.Register("Viewbox", typeof(Viewbox), typeof(ViewboxIcon),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            BaseColorProperty = DependencyProperty.Register("BaseColor", typeof(SolidColorBrush), typeof(ViewboxIcon),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, _fffF, null));

            SchemaManager.AddResource("pack://application:,,,/Ovotan.Windows.Controls;component/Resources/ViewboxButtonResource.xaml", "");
        }

        static void _fffF(DependencyObject d, DependencyPropertyChangedEventArgs e)

        {
            var self = d as ViewboxIcon;
            self._setColors();
        }

        public ViewboxIcon()
        {
            MouseDown += (s, a) =>
            {
                if (Command != null)
                {
                    Command.Execute(this);
                }
            };
        }

        void _setColors()
        {
            if (_baseColor != null && BaseColor != null)
            {
                _baseColor.Color = BaseColor.Color;
                if (_baseColorOpacity != null)
                {
                    _baseColorOpacity.Color = BaseColor.Color;
                }
            }
        }

        SolidColorBrush _baseColor;
        SolidColorBrush _baseColorOpacity;
        double _startOpacityValue;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _baseColor = Viewbox.Resources["base-color"] as SolidColorBrush;
            _baseColorOpacity = Viewbox.Resources["base-color-o"] as SolidColorBrush;
            Content = Viewbox;
            Viewbox.Width = Width;
            Viewbox.Height = Height;
            if (BaseColor != null)
            {
                _setColors();
            }

            MouseEnter += (s, a) =>
            {
                Opacity = 1;
            };
            MouseLeave += (s, a) =>
            {
                Opacity = _startOpacityValue;
            };
            _startOpacityValue = Opacity;

            //var firstColor = Viewbox.Resources["first-color"] as SolidColorBrush;
            //var secondColor = Viewbox.Resources["second-color"] as SolidColorBrush;
            //if (firstColor != null && FirstColor != null)
            //{
            //    firstColor.Color = FirstColor.Color;
            //}
            //if (secondColor != null && SecondColor != null)
            //{
            //    secondColor.Color = SecondColor.Color;
            //}

            //MouseEnter += (s, a) =>
            //{
            //    if (MouseOverFirstColor != null && firstColor != null)
            //    {
            //        firstColor.Color = MouseOverFirstColor.Color;
            //    }
            //    if (MouseOverSecondColor != null && secondColor != null)
            //    {
            //        secondColor.Color = MouseOverSecondColor.Color;
            //    }
            //};
            //MouseLeave += (s, a) =>
            //{
            //    if (FirstColor != null && firstColor != null)
            //    {
            //        firstColor.Color = FirstColor.Color;
            //    }
            //    if (SecondColor != null && secondColor != null)
            //    {
            //        secondColor.Color = SecondColor.Color;
            //    }
            //};
            //Content = Viewbox;
            //Viewbox.Width = Width;
            //Viewbox.Height = Height;
        }
    }
}
