using Ovotan.Windows.Controls.Controls;
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
        static DependencyProperty IsSelectedProperty;
        static DependencyProperty HeaderProperty;

        internal Rect Rectangle;

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
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
            IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(TabHeader),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(TabHeader),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(TabHeader),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var viewboxIcon = Template.FindName("ViewBoxIcon", this) as ViewboxIcon;
            viewboxIcon.Command = new ButtonCommand<object>(_ => _closeButtonHandler());
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
