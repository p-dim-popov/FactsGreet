namespace FactsGreet.Common
{
    using System.Collections.Generic;

    public static class Extensions
    {
        public static string Join<T>(this IEnumerable<T> collection, string separator)
        {
            return string.Join(separator, collection);
        }
    }
}
