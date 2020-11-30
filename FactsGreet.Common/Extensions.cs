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

        public static T Clone<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (source is null)
            {
                return default;
            }

            // var deserializeSettings = new JsonSerializerOptions {  = ObjectCreationHandling.Replace };
            // var serializeSettings = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(source));
        }
    }
}
