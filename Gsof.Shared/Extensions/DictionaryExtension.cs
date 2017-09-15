using System.Collections.Generic;

namespace Gsof.Shared.Extensions
{
    public static class DictionaryExtension
    {
        public static void AddRange<T1, T2, T3>(this IDictionary<T1, T2> p_dic, IDictionary<T1, T3> p_source) where T3 : T2
        {
            var dic = p_dic;
            var source = p_source;
            if (dic == null
                || source == null)
            {
                return;
            }

            foreach (var kv in source)
            {
                dic.Add(kv.Key, kv.Value);
            }
        }
    }
}
