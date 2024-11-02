using Ovotan.Windows.Controls.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Ovotan.Windows.Controls
{
    public class ViewboxButton : ContentControl
    {
        public static readonly DependencyProperty ViewboxProperty;
        public static readonly DependencyProperty ViewboxTypeProperty;
        public static readonly DependencyProperty CommandProperty;


        public Viewbox Viewbox
        {
            get
            {

                return GetValue(ViewboxProperty) as Viewbox;
            }
            set
            {
                SetValue(ViewboxProperty, value);
            }
        }

        public ViewBoxButtonType ViewboxType
        {
            get
            {
                return (ViewBoxButtonType)GetValue(ViewboxTypeProperty);
            }
            set
            {
                SetValue(ViewboxTypeProperty, value);
            }
        }

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        static ViewboxButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ViewboxButton), new FrameworkPropertyMetadata(typeof(ContentControl)));

            CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ViewboxButton),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            ViewboxProperty = DependencyProperty.Register("Viewbox", typeof(Viewbox), typeof(ViewboxButton),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, null, null));
            ViewboxTypeProperty = DependencyProperty.Register("ViewboxType", typeof(ViewBoxButtonType), typeof(ViewboxButton),
                new PropertyMetadata(ViewBoxButtonType.None));
            SchemaManager.AddResource("pack://application:,,,/Ovotan.Windows.Controls;component/Resources/ViewboxButtonResource.xaml", "");
        }

        public ViewboxButton()
        {
            this.MouseDown += (x, d) => _mouseDown();
            MouseUp += (x, d) => _mouseUp();
            MouseLeave += (x, d) => _mouseLeave();
        }

        bool fl;
        void _mouseDown()
        {
            fl = true;
        }

        void _mouseUp()
        {
            if(fl)
            {
                if(Command != null)
                {
                    Command.Execute(this);
                }    
            }
        }
        void _mouseLeave()
        {
            fl = false;
        }

        public override void OnApplyTemplate()
        {

            base.OnApplyTemplate();
            var viewBox = Template.FindName("MainViewBox", this) as ContentControl;

            if (ViewboxType != ViewBoxButtonType.None)
            {
                switch (ViewboxType)
                {
                    case ViewBoxButtonType.Connection:
                        SetValue(ViewboxProperty, Application.Current.Resources["OVBT_Connection"] as Viewbox);

                        break;
                    case ViewBoxButtonType.NewConnection:
                        SetValue(ViewboxProperty, Application.Current.Resources["OVBT_NewConnection"] as Viewbox);
                        break;
                }
            }
            viewBox.SetValue(ContentProperty ,Viewbox);
            Viewbox.SetValue(DataContextProperty, this);
        }
    }
}
