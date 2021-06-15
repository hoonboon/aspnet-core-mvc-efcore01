﻿using Microsoft.Extensions.Logging;
using MyApp.Admin.Security.Domains;
using MyApp.Admin.Security.Public.Data;
using MyApp.Admin.Security.Public.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Admin.Security.Services.Impl
{
    public class CacheControlService : ICacheControlService
    {
        private readonly ILogger<CacheControlService> _logger;
        private readonly SecurityDbContext _context;

        public CacheControlService(ILogger<CacheControlService> logger, SecurityDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<long> GetLastRefreshTimeUtcAsync(string cacheKey)
        {
            long result;
            try
            {
                var currentEntry = await _context.FindAsync<CacheControl>(cacheKey);
                if (currentEntry == null)
                {
                    currentEntry = new CacheControl
                    {
                        CacheKey = cacheKey,
                        LastRefreshTimeUtc = DateTime.UtcNow.Ticks
                    };
                    _context.Add(currentEntry);
                }

                await _context.SaveChangesAsync();

                result = currentEntry.LastRefreshTimeUtc;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get LastRefreshTimeUtc for CacheKey: {cacheKey}");
                throw ex;
            }

            return result;
        }

        public async Task UpdateLastRefreshTimeAsync(string cacheKey, bool commitChanges = true)
        {
            try
            {
                var currentEntry = await _context.FindAsync<CacheControl>(cacheKey);
                if (currentEntry != null)
                {
                    currentEntry.LastRefreshTimeUtc = DateTime.UtcNow.Ticks;
                }
                else
                {
                    _context.Add(new CacheControl
                    {
                        CacheKey = cacheKey,
                        LastRefreshTimeUtc = DateTime.UtcNow.Ticks
                    });
                }

                if (commitChanges)
                {
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update LastRefreshTimeUtc for CacheKey: {cacheKey}");
                throw ex;
            }
        }
    }
}