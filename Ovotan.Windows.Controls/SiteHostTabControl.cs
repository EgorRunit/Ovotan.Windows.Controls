using Ovotan.Windows.Controls.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Ovotan.Windows.Controls.Docking
{
    public class SiteHostTabControlItem: ContentControl
    {
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

        public ICommand CloseCommand { get; set; }

        static SiteHostTabControlItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SiteHostTabControlItem), new FrameworkPropertyMetadata(typeof(SiteHostTabControlItem)));
            IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(SiteHostTabControlItem),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(SiteHostTabControlItem),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
        }

        SolidColorBrush _mouseOverCloseBrush;
        SolidColorBrush _borderBrush;
        GeometryDrawing _closeGeometry;
        Rectangle _closeButton;




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
            if (CloseCommand != null)
            {
                CloseCommand.Execute(this);
            }
        }
    }

    [ContentProperty("Elements")]
    public class SiteHostTabControl : ContentControl
    {
        /// <summary>
        /// Previously selected tab instance.
        /// </summary>
        SiteHostTabControlItem _previouslySelectedTab = null;
        public static DependencyProperty IsMultiRowsProperty;
        public static DependencyProperty ElementsProperty;
        public static DependencyProperty HasOverflowItemsProperty;

        public bool HasOverflowItems
        {
            get { return (bool)GetValue(HasOverflowItemsProperty); }
            set { SetValue(HasOverflowItemsProperty, value); }
        }

        public bool IsMultiRows
        {
            get { return (bool)GetValue(IsMultiRowsProperty); }
            set { SetValue(IsMultiRowsProperty, value); }
        }

        public ObservableCollection<SiteHostTabControlItem> Elements
        {
            get { return GetValue(ElementsProperty) as ObservableCollection<SiteHostTabControlItem>; }
            set { SetValue(ElementsProperty, value); }
        }


        Canvas _canvas;
        static SiteHostTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SiteHostTabControl), new FrameworkPropertyMetadata(typeof(SiteHostTabControl)));
            HasOverflowItemsProperty = DependencyProperty.Register("HasOverflowItems", typeof(bool), typeof(SiteHostTabControl),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));

            ElementsProperty = DependencyProperty.Register("Elements", typeof(ObservableCollection<SiteHostTabControlItem>), typeof(SiteHostTabControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));

            IsMultiRowsProperty = DependencyProperty.Register("IsMultiRows", typeof(bool), typeof(SiteHostTabControl),
             new FrameworkPropertyMetadata(
        false,
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,fff,null));



                //new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, fff, swss));


        }
        public SiteHostTabControl()
        {
            Elements = new ObservableCollection<SiteHostTabControlItem>();
            MouseDown += _mouseDownHandler;
            _closeTabCommand = new ButtonCommand<SiteHostTabControlItem>((x) => _closeTabHandler(x));
        }

        static object swss(DependencyObject d, object baseValue)
        {
            return baseValue;
        }

        static void fff(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as SiteHostTabControl;
            if (self.ActualWidth > 0)
            {
                self.MeasureOverride(new Size(self.ActualWidth, self.ActualHeight));
            }
        }




        Menu _actionMenu;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = Template.FindName("Canvas", this) as Canvas;
            _actionMenu = Template.FindName("ActionMenu", this) as Menu;

            var dropDownListElements = _actionMenu.Items[0] as MenuItem;
            dropDownListElements.ItemsSource = new List<int>() { 4 };
            dropDownListElements.SubmenuOpened += (d, f) =>
            {
                var elements = Elements.Select(x => new MenuItem()
                {
                    Header = x.Header,
                    Tag = x,
                    Command = new ButtonCommand<SiteHostTabControlItem>(x => _setActiveTab(x)),
                    CommandParameter = x
                }).ToList();
                dropDownListElements.ItemsSource = elements;
            };
            foreach (var element in Elements)
            {
                element.DataContext = this;
                _canvas.Children.Add(element);
            }
        }




        ICommand _closeTabCommand;

        protected override Size MeasureOverride(Size constraint)
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
            foreach (var element in Elements)
            {
                var sss = element.Header;
                element.Measure(size);
                height = element.DesiredSize.Height;
                var width = element.DesiredSize.Width;
                if (left + width +actionMenuWidth > actualWidth)
                {
                    top += element.ActualHeight + 2;
                    left = 0;
                }
                element.Visibility = Visibility.Visible;
                element.SetValue(Canvas.LeftProperty, left);
                element.SetValue(Canvas.TopProperty, top);
                element.CloseCommand = _closeTabCommand;
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
            foreach (var element in Elements)
            {
                var sss = element.Header;
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
                element.CloseCommand = _closeTabCommand;
                left += width + 2;
            }

            Height = height + 2;
            if (HasOverflowItems != hasOverflowItems)
            {
                HasOverflowItems = hasOverflowItems;
            }
        }


        /// <summary>
        /// Set active docuemnt.
        /// </summary>
        /// <param name="item">The instance of document.</param>
        void _setActiveTab(SiteHostTabControlItem item)
        {
            if (_previouslySelectedTab != null)
            {
                _previouslySelectedTab.IsActive = false;
            }
            _previouslySelectedTab = item;
            _previouslySelectedTab.IsActive = true;
            var documentLeftPosition = (double)item.GetValue(Canvas.LeftProperty);
            if (documentLeftPosition + item.ActualWidth > ActualWidth)
            {
                var left = 0.0;
                for (var i = 0; i < Elements.Count; i++)
                {
                    var document = Elements[i];
                    if (left + document.ActualWidth > ActualWidth)
                    {
                        //stop
                        if (i > 0)
                        {
                            i--;
                        }
                        var documentIndex = Elements.IndexOf(item);
                        Elements[documentIndex] = Elements[i];
                        Elements[i] = item;
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
            if (element.TemplatedParent is SiteHostTabControlItem)
            {
                _setActiveTab(element.TemplatedParent as SiteHostTabControlItem);
            }
        }

        /// <summary>
        /// The handler for closing the tab.
        /// </summary>
        /// <param name="item">Closable tab instance.</param>
        void _closeTabHandler(SiteHostTabControlItem item)
        {
            var tabIndex = Elements.IndexOf(item);
            Elements.Remove(item);
            _canvas.Children.Remove(item);
            if (Elements.Count > 0 && item.IsActive)
            {
                SiteHostTabControlItem newActiveSiteHostTabControlItem = null;
                if (tabIndex < Elements.Count)
                {
                    newActiveSiteHostTabControlItem = Elements[tabIndex];
                }
                else
                {
                    newActiveSiteHostTabControlItem = Elements[tabIndex - 1];
                }
                newActiveSiteHostTabControlItem.IsActive = true;
            }
            InvalidateMeasure();
        }

    }
}
