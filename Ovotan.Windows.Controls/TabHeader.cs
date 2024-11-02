using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ovotan.Windows.Controls
{
    public class TabHeader :ContentControl
    {
        SolidColorBrush _mouseOverCloseBrush;
        SolidColorBrush _borderBrush;
        GeometryDrawing _closeGeometry;
        Rectangle _closeButton;

        static DependencyProperty IsActiveProperty;
        static DependencyProperty HeaderProperty;

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        /// <summary>
        /// get,set - The name of the tab element.
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public ICommand RemoveCommand { get; set; }

        static TabHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabHeader), new FrameworkPropertyMetadata(typeof(TabHeader)));
            IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(TabHeader),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(TabHeader),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));

        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _borderBrush = Template.Resources["BorderBrush"] as SolidColorBrush;
            _mouseOverCloseBrush = Template.Resources["MouseOverClose"] as SolidColorBrush;
            _closeButton = Template.FindName("CloseButton", this) as Rectangle;
            _closeGeometry = Template.FindName("CloseGeometry", this) as GeometryDrawing;
            _closeButton.MouseEnter += (x, d) => { _closeGeometry.Brush = _mouseOverCloseBrush; };
            _closeButton.MouseLeave += (x, d) => { _closeGeometry.Brush = _borderBrush; };
            _closeButton.MouseDown += (x, d) => { _closeButtonHandler(); };
            DataContext = this;
        }

        void _closeButtonHandler()
        {
            if (RemoveCommand != null)
            {
                RemoveCommand.Execute(this);
            }
        }
    }
}
