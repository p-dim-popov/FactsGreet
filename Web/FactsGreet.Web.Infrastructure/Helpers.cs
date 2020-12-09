namespace FactsGreet.Web.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;

    public abstract class Helpers
    {
        public static IEnumerable<string> GetRouteNames(Type controllerType, string actionName)
            => controllerType
                ?.GetMethod(actionName)
                ?.GetCustomAttributes(true)
                .OfType<IRouteTemplateProvider>()
                .Select(x => x.Name)
                .Where(x => x is not null);

        public static IEnumerable<string> GetRouteNames<T>(string actionName)
            where T : ControllerBase
            => GetRouteNames(typeof(T), actionName);
    }
}
