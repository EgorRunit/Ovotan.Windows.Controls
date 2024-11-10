using Ovotan.Windows.Controls.Controls;
using System.Windows;
using System.Windows.Controls;

namespace Ovotan.Windows.Controls
{
    public class TabControl : ContentControl
    {
        TabHeaders _tabHeaders;

        static TabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControl), new FrameworkPropertyMetadata(typeof(TabControl)));

            SchemaManager.AddResource("pack://application:,,,/Ovotan.Windows.Controls;component/Resources/TabControl.xaml", "");
        }


        public TabControl()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _tabHeaders = Template.FindName("TabHeaders", this) as TabHeaders;
            _tabHeaders.SelectedItemCommand = new ButtonCommand<TabControlItem>(x =>
            {
                Content = x?.Content;
                if (Content is FrameworkElement contentElement)
                {
                    contentElement.Focus();
                }
            });
            LostFocus += (s, e) =>
            {
                if (_tabHeaders.SelectedHeader != null)
                {
                    _tabHeaders.SelectedHeader.IsActive = false;
                }
            };
            GotFocus += (s, e) =>
            {
                if (_tabHeaders.SelectedHeader != null)
                {
                    _tabHeaders.SelectedHeader.IsActive = true;
                }
            };
        }

        public void AddTab(TabControlItem tabControl)
        {
            _tabHeaders.AddHeader(tabControl);
        }
    }
}
