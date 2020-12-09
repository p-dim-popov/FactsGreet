namespace FactsGreet.Web.Infrastructure
{
    using System;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

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

        public static string GetLocationHref(this HttpContext context)
            => context.Request.Host + context.Request.Path;
    }
}
