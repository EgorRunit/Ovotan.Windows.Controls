using Ovotan.Windows.Controls.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ovotan.Windows.Controls
{
    public class TabControl : ContentControl
    {
        public static DependencyProperty IsMultiRowsProperty;
        public static DependencyProperty HeadersProperty;

        /// <summary>
        /// get,set - Displaying mode for headers.
        /// </summary>
        public bool IsMultiRows
        {
            get { return (bool)GetValue(IsMultiRowsProperty); }
            set { SetValue(IsMultiRowsProperty, value); }
        }


        public TabHeaders Headers
        {
            get { return (TabHeaders)GetValue(HeadersProperty); }
            set { SetValue(HeadersProperty, value); }
        }

        static TabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControl), new FrameworkPropertyMetadata(typeof(TabControl)));
            HeadersProperty = DependencyProperty.Register("Headers", typeof(TabHeaders), typeof(TabControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            IsMultiRowsProperty = DependencyProperty.Register("IsMultiRows", typeof(bool), typeof(TabControl),
             new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, _isMultiRowChangedHandler, null));
        }

        /// <summary>
        /// handler for changing the tab display mode
        /// </summary>
        static void _isMultiRowChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as TabControl;
            if (self.ActualWidth > 0)
            {
                self.Headers.SetMultiRows((bool)e.NewValue);
            }
        }


        public TabControl()
        {
            var ddd = Headers;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var dropDownListElements = Template.FindName("DocumentDropDownList", this) as MenuItem;
            dropDownListElements.ItemsSource = new List<int>() { 4 };
            dropDownListElements.SubmenuOpened += (d, f) =>
            {
                var list = new List<TabHeader>(Headers.Children.Count);
                foreach (var header in Headers.Children)
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
                    //Command = new ButtonCommand<TabHeader>(x => Headers.SetActive(x)),
                    CommandParameter = x
                }).ToList();
                dropDownListElements.ItemsSource = elements;
            };
        }

        public void AddTab(TabHeader header)
        {
            if (Headers == null)
            {
                Headers = new TabHeaders();
            }
            Headers.AddHeader(header);
        }
    }
}
