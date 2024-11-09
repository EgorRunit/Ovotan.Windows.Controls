using Ovotan.Windows.Controls.Controls;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ovotan.Windows.Controls
{
    public class TabHeaders : Panel
    {
        static DependencyProperty IsMouseWheelProperty;
        static DependencyProperty IsMultiRowsProperty;
        static DependencyProperty SelectedItemCommandProperty;


        /// <summary>
        /// 
        /// </summary>
        public bool IsMouseWheel
        {
            get { return (bool)GetValue(IsMouseWheelProperty); }
            set { SetValue(IsMouseWheelProperty, value); }
        }

        public ICommand SelectedItemCommand
        {
            get { return GetValue(SelectedItemCommandProperty) as ICommand; }
            set { SetValue(SelectedItemCommandProperty, value); }
        }

        ICommand _removeHeaderCommand;
        Border _splitter;
        StackPanel _menuBlock;
        /// <summary>
        /// Previously selected header.
        /// </summary>
        TabHeader _previouslySelectedHeader;


        /// <summary>
        /// get,set - Displaying mode for headers.
        /// </summary>
        public bool IsMultiRows
        {
            get { return (bool)GetValue(IsMultiRowsProperty); }
            set { SetValue(IsMultiRowsProperty, value); }
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        static TabHeaders()
        { 
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabHeaders), new FrameworkPropertyMetadata(typeof(Panel)));
            IsMultiRowsProperty = DependencyProperty.Register("IsMultiRows", typeof(bool), typeof(TabHeaders),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, _isMultiRowChangedHandler, null));
            IsMouseWheelProperty = DependencyProperty.Register("IsMouseWheel", typeof(bool), typeof(TabHeaders),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, _isMultiRowChangedHandler, null));
            SelectedItemCommandProperty = DependencyProperty.Register("SelectedItemCommand", typeof(ICommand), typeof(TabHeaders),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            
            SchemaManager.AddResource("pack://application:,,,/Ovotan.Windows.Controls;component/Resources/TabControl.xaml", "");
        }

        /// <summary>
        /// Handler for changing the headers display mode
        /// </summary>
        static void _isMultiRowChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as TabHeaders;
            if (self.ActualWidth > 0)
            {
                self.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TabHeaders()
        {
            MouseDown += _mouseDownHandler;
            _removeHeaderCommand = new ButtonCommand<TabHeader>((x) => _removeHeaderHandler(x));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            MouseWheel += _mouseWheelHandler;

            _splitter = Application.Current.Resources["Ovotan.Windows.Controls.TabHeaders.Splitter"] as Border;
            _menuBlock = Application.Current.Resources["Ovotan.Windows.Controls.TabHeaders.RightMenu"] as StackPanel;
            _menuBlock.DataContext = this;
            var menu = _menuBlock.Children[0] as Menu;
            var dropDownListElements = menu.Items[0] as MenuItem;

            dropDownListElements.MouseEnter += (x, d) =>
            {
                d.Handled = true;
                dropDownListElements.Background = null;
            };

            var s = dropDownListElements.Resources;
            dropDownListElements.ItemsSource = new List<int>() { 4 };
            dropDownListElements.SubmenuOpened += (d, f) =>
            {
                var headers = Children.ToList<TabHeader>();
                var list = new List<TabHeader>(headers.Count);
                foreach (var header in headers)
                {
                    if (header is TabHeader)
                    {
                        list.Add(header as TabHeader);
                    }
                }
                var elements = list.Select(x => new MenuItem()
                {
                    Header = x.Header,
                    Tag = x,
                    Command = new ButtonCommand<TabHeader>(x => _setActiveTab(x)),
                    CommandParameter = x
                }).ToList();
                dropDownListElements.ItemsSource = elements;
            };
            Children.Add(_menuBlock);
            Children.Add(_splitter);
        }


        public void AddHeader(TabHeader header)
        {
            Children.Insert(0,header);
            _setActiveTab(header);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var headers = Children.ToList<TabHeader>();
            if (headers.Count > 0)
            {
                var startRenderIndex = 0;
                var endRenderIndex = headers.Count;
                var headersWidth = arrangeBounds.Width - _menuBlock.DesiredSize.Width;

                if (!IsMultiRows)
                {
                    if (_previouslySelectedHeader != null && _previouslySelectedHeader.Rectangle.Left > headersWidth)
                    {
                        endRenderIndex = headers.IndexOf(_previouslySelectedHeader);
                        var leftMax = _previouslySelectedHeader.Rectangle.Left + _previouslySelectedHeader.Rectangle.Width - headersWidth;
                        var right = headers.FirstOrDefault(x => x.Rectangle.Left + x.Rectangle.Width > leftMax);
                        startRenderIndex = headers.IndexOf(right) + 1;
                    }
                    else
                    {
                        var right = headers.FirstOrDefault(x => x.Rectangle.Left + x.Rectangle.Width > headersWidth);
                        if (right != null)
                        {
                            endRenderIndex = headers.IndexOf(right) - 1;
                        }
                    }
                }

                var delta = headers[startRenderIndex].Rectangle.Left;
                Rect rect;
                for (var i = 0; i < headers.Count; i++)
                {
                    var header = headers[i];
                    rect = header.Rectangle;
                    if (i >= startRenderIndex && i <= endRenderIndex)
                    {
                        header.Arrange(new Rect(rect.Left - delta, rect.Top, rect.Width, rect.Height));
                    }
                    else
                    {
                        header.Arrange(new Rect(0,0,0,0));
                    }
                }
                arrangeBounds.Height = rect.Top + rect.Height + _splitter.Height;


                _menuBlock.Arrange(new Rect(
                    arrangeBounds.Width - _menuBlock.DesiredSize.Width,
                    Height - _menuBlock.DesiredSize.Height,
                    _menuBlock.DesiredSize.Width,
                    _menuBlock.DesiredSize.Height));
                _splitter.Arrange(new Rect(0, rect.Top + rect.Height, arrangeBounds.Width, _splitter.Height));
            }
            else
            {
                _menuBlock.Arrange(new Rect(0, 0, 0, 0));
                _splitter.Arrange(new Rect(0, 0, 0, 0));
                arrangeBounds.Height = 0;
            }

            Height = arrangeBounds.Height;
            return base.ArrangeOverride(new Size(arrangeBounds.Width, arrangeBounds.Height));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            constraint.Height = Double.PositiveInfinity;
            var left = 0.0;
            var top = 0.0;
            if (Children.Count > 0)
            {
                _menuBlock.Measure(constraint);
                _splitter.Measure(constraint);
                var headersWidth = constraint.Width - _menuBlock.DesiredSize.Width;
                var isMultiRows = IsMultiRows;
                for (var i = 0; i < Children.Count; i++)
                {
                    var header = Children[i] as TabHeader;
                    if (header != null)
                    {
                        header.Measure(constraint);
                        header.Rectangle = new Rect(left, top, header.DesiredSize.Width, header.DesiredSize.Height);
                        left += header.DesiredSize.Width + 2;
                        if (isMultiRows)
                        {
                            if (left != 0 && left + header.DesiredSize.Width + 2 > headersWidth)
                            {
                                left = 0;
                                top += header.DesiredSize.Height + 2;
                            }
                        }
                        header.RemoveCommand = _removeHeaderCommand;
                    }
                }
                constraint.Height = top + Children[0].DesiredSize.Height + _splitter.Height;
            }
            return base.MeasureOverride(new Size(constraint.Width, constraint.Height));
        }

        /// <summary>
        /// Mouse wheel handler. Changing IsMultiRows property.
        /// </summary>
        void _mouseWheelHandler(object sender, MouseWheelEventArgs args)
        {
            if (IsMouseWheel)
            {
                if (args.Delta > 0 && IsMultiRows)
                {
                    IsMultiRows = false;
                    InvalidateMeasure();
                }
                else if (args.Delta < 0 && !IsMultiRows)
                {
                    IsMultiRows = true;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Set active header.
        /// </summary>
        /// <param name="item">The instance of header.</param>
        void _setActiveTab(TabHeader item)
        {
            if (_previouslySelectedHeader != null)
            {
                _previouslySelectedHeader.IsActive = false;
            }
            _previouslySelectedHeader = item;
            _previouslySelectedHeader.IsActive = true;
            SelectedItemCommand?.Execute(_previouslySelectedHeader);
            if(_previouslySelectedHeader.Rectangle.Left + _previouslySelectedHeader.Rectangle.Width > ActualWidth)
            {
                InvalidateArrange();
            }
        }

        /// <summary>
        /// Handler for the mouse down click event.
        /// </summary>
        void _mouseDownHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            if (element.TemplatedParent is TabHeader)
            {
                _setActiveTab(element.TemplatedParent as TabHeader);
            }
        }

        /// <summary>
        /// The remove header handler.
        /// </summary>
        /// <param name="item">The header is being deleted.</param>
        void _removeHeaderHandler(TabHeader item)
        {
            var headers = Children.ToList<TabHeader>();
            var headerIndex = headers.IndexOf(item);
            Children.Remove(item);
            headers.Remove(item);
            UIElement newActiveSiteHostTabControlItem = null;
            if (headers.Count > 0 && item.IsActive)
            {
                if (headerIndex < headers.Count)
                {
                    newActiveSiteHostTabControlItem = headers[headerIndex];
                }
                else
                {
                    newActiveSiteHostTabControlItem = headers[headerIndex - 1];
                }
                _previouslySelectedHeader = newActiveSiteHostTabControlItem as TabHeader;
                _previouslySelectedHeader.IsActive = true;
            }
            if (item.IsActive)
            {
                SelectedItemCommand?.Execute(newActiveSiteHostTabControlItem);
            }
            InvalidateMeasure();
        }
    }
}
