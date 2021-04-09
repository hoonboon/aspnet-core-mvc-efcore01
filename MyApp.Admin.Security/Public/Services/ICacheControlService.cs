using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Admin.Security.Public.Services
{
    public interface ICacheControlService
    {
        Task UpdateLastRefreshTimeAsync(string cacheKey, bool commitChanges = true);
    }
}
