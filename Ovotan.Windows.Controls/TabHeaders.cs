using Ovotan.Windows.Controls.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ovotan.Windows.Controls
{
    public class TabHeaders : Panel
    {
        ICommand _removeHeaderCommand;
        Menu _actionMenu;
        Canvas _canvas;
        /// <summary>
        /// Previously selected header.
        /// </summary>
        TabHeader _previouslySelectedHeader;
        bool _isMultiRows;


        /// <summary>
        /// Constructor.
        /// </summary>
        static TabHeaders()
        { 
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabHeaders), new FrameworkPropertyMetadata(typeof(TabHeaders)));
            SchemaManager.AddResource("pack://application:,,,/Ovotan.Windows.Controls;component/Resources/TabControl.xaml", "");
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TabHeaders()
        {
            MouseDown += _mouseDownHandler;
            _removeHeaderCommand = new ButtonCommand<TabHeader>((x) => _removeHeader(x));
        }

        public void AddHeader(TabHeader header)
        {
            Children.Insert(0,header);
            _setActiveTab(header);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (Children.Count > 0)
            {
                var headers = Children.ToList<TabHeader>();
                var startRenderIndex = 0;
                var endRenderIndex = headers.Count;

                if (!_isMultiRows)
                {
                    if (_previouslySelectedHeader != null && _previouslySelectedHeader.Rectangle.Left > arrangeBounds.Width)
                    {
                        endRenderIndex = headers.IndexOf(_previouslySelectedHeader);
                        var leftMax = _previouslySelectedHeader.Rectangle.Left + _previouslySelectedHeader.Rectangle.Width - arrangeBounds.Width;
                        var right = headers.FirstOrDefault(x => x.Rectangle.Left + x.Rectangle.Width > leftMax);
                        startRenderIndex = headers.IndexOf(right) + 1;
                    }
                    else
                    {
                        var right = headers.FirstOrDefault(x => x.Rectangle.Left + x.Rectangle.Width > arrangeBounds.Width);
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
                arrangeBounds.Height = rect.Top + rect.Height;
            }
            Height = arrangeBounds.Height;
            return base.ArrangeOverride(new Size(arrangeBounds.Width, arrangeBounds.Height));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var left = 0.0;
            var top = 0.0;
            if (Children.Count > 0)
            {
                for (var i = 0; i < Children.Count; i++)
                {
                    var header = Children[i] as TabHeader;
                    if (header != null)
                    {
                        header.Measure(constraint);
                        header.Rectangle = new Rect(left, top, header.DesiredSize.Width, header.DesiredSize.Height);
                        left += header.DesiredSize.Width + 2;
                        if (_isMultiRows)
                        {
                            if (left != 0 && left + header.DesiredSize.Width + 2 > constraint.Width)
                            {
                                left = 0;
                                top += header.DesiredSize.Height + 2;
                            }
                        }
                    }
                    header.RemoveCommand = _removeHeaderCommand;
                }
                constraint.Height = top + Children[0].DesiredSize.Height;
            }
            return base.MeasureOverride(new Size(constraint.Width, constraint.Height));

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
        /// Removing header.
        /// </summary>
        /// <param name="item">Еhe header is being deleted.</param>
        void _removeHeader(TabHeader item)
        {
            var headerIndex = Children.IndexOf(item);
            Children.Remove(item);
            if (Children.Count > 0 && item.IsActive)
            {
                UIElement newActiveSiteHostTabControlItem = null;
                if (headerIndex < Children.Count)
                {
                    newActiveSiteHostTabControlItem = Children[headerIndex];
                }
                else
                {
                    newActiveSiteHostTabControlItem = Children[headerIndex - 1];
                }
                if (newActiveSiteHostTabControlItem is TabHeader)
                {
                    _previouslySelectedHeader = newActiveSiteHostTabControlItem as TabHeader;
                    _previouslySelectedHeader.IsActive = true;
                }
            }
            InvalidateMeasure();
        }

        public void SetMultiRows(bool isMultiRows)
        {
            _isMultiRows = isMultiRows;
            InvalidateMeasure();
        }
    }
}
