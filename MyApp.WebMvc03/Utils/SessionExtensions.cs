using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Utils
{
    public static class SessionExtensions
    {
        /// <summary>
        /// Reference: https://stackoverflow.com/a/66663987
        /// 
        /// Have sessions be asyncronous. This adaptation is needed to force the session provider to use async calls instead of syncronous ones for session. 
        /// Someone surprisingly for something that seems common, Microsoft didn't make this aspect super nice.
        /// </summary>
        /// <param name="app">App builder instance.</param>
        /// <returns>App builder instance for chaining.</returns>
        /// <remarks>
        /// From Microsoft Documentation (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-5.0):
        /// The default session provider in ASP.NET Core will only load the session record from the underlying IDistributedCache store asynchronously if the
        /// ISession.LoadAsync method is explicitly called before calling the TryGetValue, Set or Remove methods. 
        /// Failure to call LoadAsync first will result in the underlying session record being loaded synchronously,
        /// which could potentially impact the ability of an application to scale.
        /// 
        /// See also:
        /// https://github.com/dotnet/aspnetcore/blob/d2a0cbc093e1e7bb3e38b55cd6043e4e2a0a2e9a/src/Middleware/Session/src/DistributedSession.cs#L268
        /// https://github.com/dotnet/AspNetCore.Docs/issues/1840#issuecomment-454182594
        /// https://bartwullems.blogspot.com/2019/12/aspnet-core-load-session-state.html
        /// </remarks>
        public static IApplicationBuilder UseAsyncSession(this IApplicationBuilder app)
        {
            app.UseSession();
            
            // register middleware once
            app.Use(async (context, next) =>
            {
                // middleware to be called by each request
                await context.Session.LoadAsync();
                await next();
            });

            return app;
        }

        public static void Set<T>(this ISession session, string key, T value)
        {
            //Console.Out.WriteLine("SessionExtensions.Set() called.");
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            //Console.Out.WriteLine("SessionExtensions.Get() called.");
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
