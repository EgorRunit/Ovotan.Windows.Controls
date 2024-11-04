using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Ovotan.Windows.Controls
{
    public static class UIElementCollectionExtension
    {
        public static List<T> ToList<T>(this UIElementCollection collection) where T : class
        {
            var result = new List<T>(collection.Count);
            foreach (var item in collection)
            {
                if (item is T)
                {
                    result.Add(item as T);
                }
            }
            return result;
        }
    }
}
