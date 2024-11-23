using System.Windows;

namespace Ovotan.Windows.Controls
{
    public static class DependencyObjectExtension
    {
        public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject element) where T : class
        {
            if (element != null)
            {
                if (element is T)
                    yield return element as T;

                foreach (var child in LogicalTreeHelper.GetChildren(element).OfType<DependencyObject>())
                    foreach (T c in FindLogicalChildren<T>(child))
                        yield return c;
            }
        }

        public static IEnumerable<T> FindLogicalParent<T>(this FrameworkElement element) where T : class
        {
            if (element != null)
            {
                object node = element;
                while (node != null)
                {
                    if(node is T)
                    {
                        yield return node as T;
                    }
                    if (node is FrameworkElement frameworkElement)
                    {
                        var ss = frameworkElement.TemplatedParent;
                        node = frameworkElement.Parent;
                        //node = (node as FrameworkElement).Parent;
                    }
                }
            }
        }
        //public static IEnumerable<T> FindLogicalParent<T>(this FrameworkElement element) where T : class
        //{
        //    if (element != null)
        //    {
        //        object node = element;
        //        while (node != null)
        //        {
        //            if (node is T)
        //            {
        //                yield return node as T;
        //            }
        //            if (node is FrameworkElement frameworkElement)
        //            {
        //                var ss = frameworkElement.TemplatedParent;
        //                node = frameworkElement.Parent;
        //                //node = (node as FrameworkElement).Parent;
        //            }
        //        }
        //    }
        //}


        public static IEnumerable<T> FindLogicalParentTag<T>(this FrameworkElement element) where T : class
        {
            if (element != null)
            {
                while (element.Parent != null)
                {
                    var  ui = element.Parent as FrameworkElement;
                    if (ui != null && ui.Tag is T)
                    {
                        yield return ui.Tag as T;
                    }
                    element = element.Parent as FrameworkElement;
                }
            }
        }
    }
}
