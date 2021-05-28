using System.Threading.Tasks;

namespace MyApp.Admin.Security.Public.PermissionControl.Cookie
{
    public interface IAuthChanges
    {
        /// <summary>
        /// This returns true if there is no ticksToCompare or the ticksToCompare is earlier than the AuthLastUpdated time
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="ticksToCompareString"></param>
        /// <returns></returns>
        Task<bool> IsOutOfDateOrMissingAsync(string cacheKey, string ticksToCompareString);


        /// <summary>
        /// This adds or updates the CacheControl entry with the cacheKey with the cachedValue (datetime as ticks) 
        /// </summary>
        //Task AddOrUpdateAsync();
    }
}