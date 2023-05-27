using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelBot.Runtime.Util
{
    public static class Extensions
    {
        // Based on: https://stackoverflow.com/a/200584/419956 by @FredrikKalseth
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}
