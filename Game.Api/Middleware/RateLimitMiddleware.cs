using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Game.Api.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RateLimitMiddleware> _logger;
        private readonly int _limit = 120; // requests per minute per key

        public RateLimitMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<RateLimitMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var key = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var entry = _cache.GetOrCreate(key, e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return 0;
            });

            if ((int)entry >= _limit)
            {
                _logger.LogWarning("Rate limit exceeded for {Key}", key);
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Rate limit exceeded");
                return;
            }

            _cache.Set(key, (int)entry + 1);
            await _next(context);
        }
    }
}