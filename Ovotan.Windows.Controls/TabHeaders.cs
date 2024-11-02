using Ovotan.Windows.Controls.Controls;
using Ovotan.Windows.Controls.Docking;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ovotan.Windows.Controls
{
    public class TabHeaders : ContentControl
    {
        ICommand _removeHeaderCommand;
        Menu _actionMenu;
        Canvas _canvas;
        /// <summary>
        /// Previously selected header.
        /// </summary>
        TabHeader _previouslySelectedHeader;
        public static DependencyProperty IsMultiRowsProperty;
        public static DependencyProperty HeadersProperty;
        public static DependencyProperty HasOverflowItemsProperty;

        /// <summary>
        /// get,set - It has overflow headers.
        /// </summary>
        public bool HasOverflowItems
        {
            get { return (bool)GetValue(HasOverflowItemsProperty); }
            set { SetValue(HasOverflowItemsProperty, value); }
        }

        /// <summary>
        /// get,set - Displaying mode for headers.
        /// </summary>
        public bool IsMultiRows
        {
            get { return (bool)GetValue(IsMultiRowsProperty); }
            set { SetValue(IsMultiRowsProperty, value); }
        }

        /// <summary>
        /// get,set - Collection for headers.
        /// </summary>
        public ObservableCollection<TabHeader> Headers
        {
            get { return GetValue(HeadersProperty) as ObservableCollection<TabHeader>; }
            set { SetValue(HeadersProperty, value); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        static TabHeaders()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabHeaders), new FrameworkPropertyMetadata(typeof(TabHeaders)));
            HasOverflowItemsProperty = DependencyProperty.Register("HasOverflowItems", typeof(bool), typeof(TabHeaders),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            HeadersProperty = DependencyProperty.Register("Headers", typeof(ObservableCollection<TabHeader>), typeof(TabHeaders),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            IsMultiRowsProperty = DependencyProperty.Register("IsMultiRows", typeof(bool), typeof(TabHeaders),
             new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, _isMultiRowChangedHandler, null));
            SchemaManager.AddResource("pack://application:,,,/Ovotan.Windows.Controls;component/Resources/TabControl.xaml", "");
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TabHeaders()
        {
            Headers = new ObservableCollection<TabHeader>();
            MouseDown += _mouseDownHandler;
            _removeHeaderCommand = new ButtonCommand<TabHeader>((x) => _removeHeader(x));
        }

        public void AddHeader(TabHeader header)
        {
            Headers.Add(header);
            header.IsActive = true;
            _canvas.Children.Add(header);
            _setActiveTab(header);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = Template.FindName("Canvas", this) as Canvas;
            _actionMenu = Template.FindName("ActionMenu", this) as Menu;

            var dropDownListElements = _actionMenu.Items[0] as MenuItem;
            dropDownListElements.ItemsSource = new List<int>() { 4 };
            dropDownListElements.SubmenuOpened += (d, f) =>
            {
                var elements = Headers.Select(x => new MenuItem()
                {
                    Header = x.Header,
                    Tag = x,
                    Command = new ButtonCommand<TabHeader>(x => _setActiveTab(x)),
                    CommandParameter = x
                }).ToList();
                dropDownListElements.ItemsSource = elements;
            };
            foreach (var element in Headers)
            {
                element.DataContext = this;
                _canvas.Children.Add(element);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (Headers.Count > 0)
            {
                if (IsMultiRows)
                {
                    _renderTabsInMultipleRows(constraint);
                }
                else
                {
                    _renderTabsInSingleRow(constraint);
                }
                constraint.Height = Height;
            }
            else if (ActualHeight == 0)
            {
                var tempHeader = new TabHeader() { Header = "T" };
                _canvas.Children.Add(tempHeader);
                var size = new Size(double.PositiveInfinity, double.PositiveInfinity);
                tempHeader.Measure(size);
                _canvas.Children.Clear();
                Height = tempHeader.DesiredSize.Height + 2;
                constraint.Height = Height;
            }

            return base.MeasureOverride(constraint);
        }


        /// <summary>
        /// Displaying tabs in multiple row.
        /// </summary>
        /// <param name="constraint">Size of content.</param>
        void _renderTabsInMultipleRows(Size constraint)
        {
            var size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            var actionMenuWidth = _actionMenu.ActualWidth;
            if (actionMenuWidth == 0.0)
            {
                _actionMenu.Measure(size);
                actionMenuWidth = _actionMenu.DesiredSize.Width;
            }
            var actualWidth = constraint.Width;
            var left = 0.0;
            var top = 0.0;
            var height = 0.0;
            foreach (var element in Headers)
            {
                element.Measure(size);
                height = element.DesiredSize.Height;
                var width = element.DesiredSize.Width;
                if (left + width + actionMenuWidth > actualWidth)
                {
                    top += element.ActualHeight + 2;
                    left = 0;
                }
                element.Visibility = Visibility.Visible;
                element.SetValue(Canvas.LeftProperty, left);
                element.SetValue(Canvas.TopProperty, top);
                element.RemoveCommand = _removeHeaderCommand;
                left += width + 2;
            }

            Height = top + height + 2;
            if (HasOverflowItems)
            {
                HasOverflowItems = false;
            }
        }


        /// <summary>
        /// Displaying tabs in one row.
        /// </summary>
        /// <param name="constraint">Size of content.</param>
        void _renderTabsInSingleRow(Size constraint)
        {
            var size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            var actionMenuWidth = _actionMenu.ActualWidth;
            if (actionMenuWidth == 0.0)
            {
                _actionMenu.Measure(size);
                actionMenuWidth = _actionMenu.DesiredSize.Width;
            }
            var actualWidth = constraint.Width;
            var left = 0.0;
            var top = 0.0;
            var hasOverflowItems = false;
            var height = 0.0;
            foreach (var element in Headers)
            {
                element.Measure(size);
                height = element.DesiredSize.Height;
                var width = element.DesiredSize.Width;
                if (left + width + actionMenuWidth > actualWidth)
                {
                    hasOverflowItems = true;
                    element.Visibility = Visibility.Hidden;
                }
                else
                {
                    element.Visibility = Visibility.Visible;
                    element.SetValue(Canvas.LeftProperty, left);
                    element.SetValue(Canvas.TopProperty, top);
                }
                element.RemoveCommand = _removeHeaderCommand;
                left += width + 2;
            }

            Height = height + 2;
            if (HasOverflowItems != hasOverflowItems)
            {
                HasOverflowItems = hasOverflowItems;
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
            var documentLeftPosition = (double)item.GetValue(Canvas.LeftProperty);
            if (documentLeftPosition + item.ActualWidth > ActualWidth)
            {
                var left = 0.0;
                for (var i = 0; i < Headers.Count; i++)
                {
                    var document = Headers[i];
                    if (left + document.ActualWidth > ActualWidth)
                    {
                        //stop
                        if (i > 0)
                        {
                            i--;
                        }
                        var documentIndex = Headers.IndexOf(item);
                        Headers[documentIndex] = Headers[i];
                        Headers[i] = item;
                        break;
                    }
                    left += document.ActualWidth + 2;
                }
            }
            MeasureOverride(new Size(ActualWidth, ActualHeight));
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
        /// Removing header.
        /// </summary>
        /// <param name="item">Еhe header is being deleted.</param>
        void _removeHeader(TabHeader item)
        {
            var headerIndex = Headers.IndexOf(item);
            Headers.Remove(item);
            _canvas.Children.Remove(item);
            if (Headers.Count > 0 && item.IsActive)
            {
                TabHeader newActiveSiteHostTabControlItem = null;
                if (headerIndex < Headers.Count)
                {
                    newActiveSiteHostTabControlItem = Headers[headerIndex];
                }
                else
                {
                    newActiveSiteHostTabControlItem = Headers[headerIndex - 1];
                }
                newActiveSiteHostTabControlItem.IsActive = true;
            }
            InvalidateMeasure();
        }

        /// <summary>
        /// handler for changing the tab display mode
        /// </summary>
        static void _isMultiRowChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as TabHeaders;
            if (self.ActualWidth > 0)
            {
                self.MeasureOverride(new Size(self.ActualWidth, self.ActualHeight));
            }
        }
    }
}
