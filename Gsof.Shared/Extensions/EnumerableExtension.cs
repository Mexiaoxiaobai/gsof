using System;
using System.Collections.Generic;
using System.Text;

namespace Gsof.Shared.Extensions
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> Take<T>(this IList<T> list, int start, int length)
        {
            for (int i = start; i < Math.Min(list.Count, start + length); i++)
            {
                yield return list[i];
            }
        }

        public static void Apply<T>(this IEnumerable<T> p_enumerable, Action<T> p_action)
        {
            if (p_enumerable == null || p_action == null)
            {
                return;
            }

            foreach (T t in p_enumerable)
            {
                p_action(t);
            }
        }
    }
}
