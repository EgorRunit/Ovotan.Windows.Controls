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

        public void SetActive(TabHeader tabHeader)
        {
            _setActiveTab(tabHeader);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var left = 0.0;
            var top = 0.0;
            var height = 0.0;
            if (Children.Count > 0)
            {
                var firstHeader = Children[0];
                height = firstHeader.DesiredSize.Height;
                if (!_isMultiRows)
                {
                    for (var i = 0; i < Children.Count; i++)
                    {
                        var header = Children[i] as TabHeader;
                        if (left != 0 && left + header.DesiredSize.Width + 2 > arrangeBounds.Width)
                        {
                            //header.Arrange(new Rect(0, 0, 0, 0));
                        }
                        else
                        {
                            header.Rect = new Rect(left, top, header.DesiredSize.Width, header.DesiredSize.Height);
                            header.Arrange(header.Rect);
                            arrangeBounds.Height = header.DesiredSize.Height + 2;
                        }
                        left += header.DesiredSize.Width + 2;
                    }
                }
                else
                {
                    for (var i = 0; i < Children.Count; i++)
                    {
                        var header = Children[i];
                        if (left != 0 && left + header.DesiredSize.Width + 2 > arrangeBounds.Width)
                        {
                            left = 0;
                            top += header.DesiredSize.Height + 2;
                        }
                        header.Arrange(new Rect(left, top, header.DesiredSize.Width, header.DesiredSize.Height));
                        left += header.DesiredSize.Width + 2;
                    }
                }
                arrangeBounds.Height = top + height;
            }
            Height = arrangeBounds.Height;
            return base.ArrangeOverride(new Size(arrangeBounds.Width, arrangeBounds.Height));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var left = 0.0;
            var top = 0.0;
            var height = 0.0; 
            if (Children.Count > 0)
            {
                var firstHeader = Children[0];
                firstHeader.Measure(constraint);
                height = firstHeader.DesiredSize.Height;
                for (var i = 0; i < Children.Count; i++)
                {
                    var header = Children[i] as TabHeader;
                    if (header != null)
                    {
                        header.Measure(constraint);
                        if (!_isMultiRows)
                        {
                            left += header.DesiredSize.Width + 2;
                        }
                        else
                        {
                            if (left != 0 && left + header.DesiredSize.Width + 2 > constraint.Width)
                            {
                                left = 0;
                                top += header.DesiredSize.Height + 2;
                            }
                            left += header.DesiredSize.Width + 2;
                        }
                    }
                    header.RemoveCommand = _removeHeaderCommand;
                }
                constraint.Height = height + top;
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
            return;
            //var point = new Point();
            //var ssds = this.TranslatePoint(point, Children[0]);
            //var ssds1 = Children[0].GetValue(Canvas.LeftProperty);
            //var documentLeftPosition = (double)item.GetValue(Canvas.LeftProperty);
            //if (documentLeftPosition + item.ActualWidth > ActualWidth)
            //{
            //    var left = 0.0;
            //    for (var i = 0; i <Children.Count; i++)
            //    {
            //        var document = Children[i] as TabHeader;
            //        if (left + document.ActualWidth > ActualWidth && document != null)
            //        {
            //            if (i > 0)
            //            {
            //                i--;
            //            }
            //            var documentIndex = Children.IndexOf(item);
            //            Children[documentIndex] = Children[i];
            //            Children[i] = item;
            //            break;
            //        }
            //        left += document.ActualWidth + 2;
            //    }
            //    MeasureOverride(new Size(ActualWidth, ActualHeight));
            //}
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
