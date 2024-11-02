using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ovotan.Windows.Controls
{
    public class TabHeader : ContentControl
    {
        SolidColorBrush _mouseOverCloseBrush;
        SolidColorBrush _borderBrush;
        GeometryDrawing _closeGeometry;
        Rectangle _closeButton;

        static DependencyProperty IsActiveProperty;
        static DependencyProperty HeaderProperty;

        static DependencyProperty ActiveBackgroundProperty;
        static DependencyProperty ActiveBorderBrushProperty;
        static DependencyProperty ActiveForegroundProperty;

        static DependencyProperty MouseOverBackgroundProperty;
        static DependencyProperty MouseOverBorderBrushProperty;
        static DependencyProperty MouseOverForegroundProperty;

        static DependencyProperty ActiveCloseIconBackgroundProperty;
        static DependencyProperty MouseOverCloseIconBackgroundProperty;

        public Brush MouseOverBackground
        {
            get { return GetValue(MouseOverBackgroundProperty) as Brush; }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }
        
        public Brush MouseOverBorderBrush
        {
            get { return GetValue(MouseOverBorderBrushProperty) as Brush; }
            set { SetValue(MouseOverBorderBrushProperty, value); }
        }

        public Brush MouseOverForeground
        {
            get { return GetValue(MouseOverForegroundProperty) as Brush; }
            set { SetValue(MouseOverForegroundProperty, value); }
        }

        public Brush ActiveBackground
        {
            get { return GetValue(ActiveBackgroundProperty) as Brush; }
            set { SetValue(ActiveBackgroundProperty, value); }
        }

        public Brush ActiveBorderBrush
        {
            get { return GetValue(ActiveBorderBrushProperty) as Brush; }
            set { SetValue(ActiveBorderBrushProperty, value); }
        }

        public Brush ActiveForeground
        {
            get { return GetValue(ActiveForegroundProperty) as Brush; }
            set { SetValue(ActiveForegroundProperty, value); }
        }

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
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabHeader), new FrameworkPropertyMetadata(typeof(ContentControl)));
            IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(TabHeader),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(TabHeader),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));

            ActiveBackgroundProperty = DependencyProperty.Register("ActiveBackground", typeof(Brush), typeof(TabHeader),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            ActiveBorderBrushProperty = DependencyProperty.Register("ActiveBorderBrush", typeof(Brush), typeof(TabHeader),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            ActiveForegroundProperty = DependencyProperty.Register("ActiveForeground", typeof(Brush), typeof(TabHeader),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));

            MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(TabHeader),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            MouseOverBorderBrushProperty = DependencyProperty.Register("MouseOverBorderBrush", typeof(Brush), typeof(TabHeader),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            MouseOverForegroundProperty = DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(TabHeader),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //_borderBrush = Template.Resources["BorderBrush"] as SolidColorBrush;
            //_mouseOverCloseBrush = Template.Resources["MouseOverClose"] as SolidColorBrush;
            _closeGeometry = Template.FindName("CloseGeometry", this) as GeometryDrawing;
            _closeButton = Template.FindName("CloseButton", this) as Rectangle;
            //_closeButton.MouseEnter += (x, d) => { _closeGeometry.Brush = _mouseOverCloseBrush; };
            //_closeButton.MouseLeave += (x, d) => { _closeGeometry.Brush = _borderBrush; };
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
