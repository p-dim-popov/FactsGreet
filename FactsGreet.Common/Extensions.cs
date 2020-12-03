namespace FactsGreet.Common
{
    using System.Collections.Generic;
    using System.Text.Json;

    public static class Extensions
    {
        public static string Join<T>(this IEnumerable<T> collection, string separator)
        {
            return string.Join(separator, collection);
        }

        /// <summary>
        /// Clones an object by serializing it and then deserializing it using JsonSerializer class
        /// </summary>
        /// <param name="source">The object to clone.</param>
        /// <typeparam name="T">Object type.</typeparam>
        /// <returns>New cloned object.</returns>
        public static T Clone<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (source is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(source));
        }

        /// <summary>
        /// Transforms the values of the object into a dictionary of strings.
        /// </summary>
        /// <param name="object">The object instance to convert.</param>
        /// <returns>A dictionary mapping field names to values.</returns>
        public static Dictionary<string, string> ToDictionary<T>(this T @object)
        {
            var dictionary = new Dictionary<string, string>();
            if (@object == null)
            {
                return dictionary;
            }

            foreach (var property in @object.GetType().GetProperties())
            {
                var obj = property.GetValue(@object, null) ?? string.Empty;
                dictionary.Add(property.Name, obj.ToString());
            }

            return dictionary;
        }

        public static IDictionary<TKey, TValue> AppendElement<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            dictionary.Add(key, value);
            return dictionary;
        }
    }
}
