using System.Collections.Generic;

namespace VM
{
    public static class ListHelper
    {
        public static void Push<T>(this List<T> list, T item)
        {
            list.Add(item);
        }

        public static T Pop<T>(this List<T> list)
        {
            var lastIndex = list.Count - 1;
            T item = list[lastIndex];
            list.RemoveAt(lastIndex);
            return item;
        }
    }
}
