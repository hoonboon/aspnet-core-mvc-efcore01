// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using MyApp.Admin.Security.Public.Constants;
using MyApp.Admin.Security.Public.PermissionControl.Cookie;
using MyApp.Admin.Security.Public.Services;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MyApp.Admin.Security.PermissionControl.Cookie.Impl
{
    public class AuthChanges : IAuthChanges
    {
        private ICacheControlService cacheControl;

        public AuthChanges(ICacheControlService cacheControl)
        {
            this.cacheControl = cacheControl;
        }

        /// <summary>
        /// This returns true if ticksToCompareString is null, or if its value is lower than the value in the TimeStore
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="ticksToCompareString"></param>
        /// <param name="cacheControl"></param>
        /// <returns></returns>
        public async Task<bool> IsOutOfDateOrMissingAsync(string cacheKey, string ticksToCompareString)
        {
            if (ticksToCompareString == null)
                // if there is no time claim then you do need to reset the claims
                return true;

            var ticksToCompare = long.Parse(ticksToCompareString);
            return await IsOutOfDateAsync(cacheKey, ticksToCompare);
        }

        private async Task<bool> IsOutOfDateAsync(string cacheKey, long ticksToCompare)
        {
            var cachedTicks = await cacheControl.GetLastRefreshTimeUtcAsync(cacheKey);
            
            return ticksToCompare < cachedTicks;
        }

        //public async Task AddOrUpdateAsync()
        //{
        //    await cacheControl.UpdateLastRefreshTimeAsync(CacheKeys.USER_PERMISSIONS, false);
        //}
    }
}
