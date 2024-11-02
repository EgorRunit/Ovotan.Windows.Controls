using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ovotan.Windows.Controls
{
    public class ViewboxIcon : ContentControl
    {
        public static readonly DependencyProperty ViewboxProperty;

        public SolidColorBrush FirstColor { get; set; }
        public SolidColorBrush SecondColor { get; set; }
        public SolidColorBrush MouseOverFirstColor { get; set; }
        public SolidColorBrush MouseOverSecondColor { get; set; }

        public Viewbox Viewbox
        {
            get { return GetValue(ViewboxProperty) as Viewbox; }
            set { SetValue(ViewboxProperty, value); }
        }

        static ViewboxIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ViewboxIcon), new FrameworkPropertyMetadata(typeof(ContentControl)));
            ViewboxProperty = DependencyProperty.Register("Viewbox", typeof(Viewbox), typeof(ViewboxIcon),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            SchemaManager.AddResource("pack://application:,,,/Ovotan.Windows.Controls;component/Resources/ViewboxButtonResource.xaml", "");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var firstColor = Viewbox.Resources["first-color"] as SolidColorBrush;
            var secondColor = Viewbox.Resources["second-color"] as SolidColorBrush;
            if (firstColor != null && FirstColor != null)
            {
                FirstColor = new SolidColorBrush(Color.FromScRgb((float)firstColor.Opacity, FirstColor.Color.ScR, FirstColor.Color.ScG, FirstColor.Color.ScB));
                Viewbox.Resources["first-color"] = FirstColor;
                if (MouseOverFirstColor != null)
                {
                    MouseOverFirstColor = new SolidColorBrush(Color.FromScRgb((float)firstColor.Opacity, MouseOverFirstColor.Color.ScR, MouseOverFirstColor.Color.ScG, MouseOverFirstColor.Color.ScB));
                }
            }
            if (secondColor != null && SecondColor != null)
            {
                SecondColor = new SolidColorBrush(Color.FromScRgb((float)secondColor.Opacity, SecondColor.Color.ScR, SecondColor.Color.ScG, SecondColor.Color.ScB));
                Viewbox.Resources["second-color"] = SecondColor;
                if (MouseOverSecondColor != null)
                {
                    MouseOverSecondColor = new SolidColorBrush(Color.FromScRgb((float)secondColor.Opacity, MouseOverSecondColor.Color.ScR, MouseOverSecondColor.Color.ScG, MouseOverSecondColor.Color.ScB));
                }
            }
            MouseEnter += (s, a) =>
            {
                if (MouseOverFirstColor != null && firstColor != null)
                {
                    Viewbox.Resources["first-color"] = MouseOverFirstColor;
                }
                if (MouseOverSecondColor != null && secondColor != null)
                {
                    Viewbox.Resources["second-color"] = MouseOverSecondColor;
                }
            };
            MouseLeave += (s, a) =>
            {
                if (FirstColor != null && firstColor != null)
                {
                    Viewbox.Resources["first-color"] = FirstColor;
                }
                if (SecondColor != null && secondColor != null)
                {
                    Viewbox.Resources["second-color"] = SecondColor;
                }
            };
            Content = Viewbox;
            Viewbox.Width = Width;
            Viewbox.Height = Height;
        }
    }
}
