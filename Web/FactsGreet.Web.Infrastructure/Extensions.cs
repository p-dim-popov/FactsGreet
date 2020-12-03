namespace FactsGreet.Web.Infrastructure
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;

    public static class Extensions
    {
        public static IApplicationBuilder UseUnderscoreInsteadOfWhitespaceInUrl(this IApplicationBuilder app)
            => app?.Use(async (context, next) =>
            {
                var destination = context.Request.Path.Value ?? string.Empty;
                if (destination.Contains(' '))
                {
                    context.Response
                        .Redirect(Uri.EscapeUriString(destination
                            .Replace(' ', '_')));
                }

                context.Request.Path = destination.Replace('_', ' ');

                await next.Invoke();
            });

        public static string FirstOrDefaultNotNullRouteName(Type controllerType, string actionName)
            => controllerType
                ?.GetMethod(actionName)
                ?.GetCustomAttributes(true)
                .OfType<IRouteTemplateProvider>()
                .Select(x => x?.Name)
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        public static string FirstOrDefaultNotNullRouteName<T>(this T controller, string actionName)
            where T : ControllerBase
            => FirstOrDefaultNotNullRouteName(typeof(T), actionName);
    }
}
