using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace CommonBE.Services;

public interface ICacheProvider
{
    const int CacheSeconds = 300; //5 min
    T GetFromCache<T>(string key) where T : class;
    T GetFromCache<T>(string key, string id) where T : class;
    void SetCache<T>(string key, T value) where T : class;
    void SetCache<T>(string key, T value, int seconds = CacheSeconds) where T : class;
    void SetCache<T>(string key, string id, T value) where T : class;
    void SetCache<T>(string key, string id, T value, int seconds = CacheSeconds) where T : class;
    void ClearCache(string key);
    void ClearCache(string key, string id);
    Task<T> GetOrAddAsync<T>(string key, int seconds, Func<Task<T>> taskFactory);
    Task<T> GetOrAddAsync<T>(string key, string id, int seconds, Func<Task<T>> taskFactory);

    /// <summary>
    /// null, ToString(), Auto
    /// </summary>
    /// <param name="userId">userId can by null</param>
    /// <param name="className">ToString() recomended</param>
    /// <param name="caller">optional, is automatic</param>
    /// <returns></returns>
    static string GetCacheKey(string userId, string className, [CallerMemberName] string caller = null) => $"{caller}||{userId}||{className}";
}

public class CacheProvider : ICacheProvider
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheProvider> _logger;

    public CacheProvider(IMemoryCache cache, ILogger<CacheProvider> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public T GetFromCache<T>(string key) where T : class
    {
        var cachedResponse = _cache.Get(key);
        return cachedResponse as T;
    }

    public T GetFromCache<T>(string key, string id) where T : class
    {
        var cachedResponse = _cache.Get($"{key}-{id}");
        return cachedResponse as T;
    }

    public void SetCache<T>(string key, T value) where T : class
    {
        SetCache(key, value, ICacheProvider.CacheSeconds);
    }

    public void SetCache<T>(string key, T value, int seconds = ICacheProvider.CacheSeconds) where T : class
    {
        _cache.Set(key, value, DateTimeOffset.Now.AddSeconds(seconds));
    }

    public void SetCache<T>(string key, string id, T value) where T : class
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));

        SetCache($"{key}-{id}", value);
    }

    public void SetCache<T>(string key, string id, T value, int seconds = ICacheProvider.CacheSeconds) where T : class
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));

        SetCache($"{key}-{id}", value, seconds);
    }

    public void ClearCache(string key)
    {
        _cache.Remove(key);
    }

    public void ClearCache(string key, string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));

        _cache.Remove($"{key}-{id}");

    }

    public async Task<T> GetOrAddAsync<T>(string key, int seconds, Func<Task<T>> taskFactory)
    {
        //bug would always get from cache
        //return _cache.GetOrCreateAsync<T>(key, async entry => await new AsyncLazy<T>(async () =>
        //{
        //    entry.SlidingExpiration = TimeSpan.FromSeconds(seconds);
        //    return await taskFactory.Invoke();
        //}).Value);

        var res = await _cache.GetOrCreateAsync(key, async entry => await new AsyncLazy<T>(async () =>
        {
            entry.SlidingExpiration = TimeSpan.FromSeconds(seconds);
            return await taskFactory.Invoke();
        }).Value);
        if (res == null) //remove from cache if null
            _cache.Remove(key);
#if DEBUG
        _logger?.LogDebug($"{nameof(GetOrAddAsync)} {key} getting from cache");
#endif
        return res;
    }

    public async Task<T> GetOrAddAsync<T>(string key, string id, int seconds, Func<Task<T>> taskFactory)
    {
        var comKey = $"{key}-{id}";
        var res = await _cache.GetOrCreateAsync(comKey, async entry => await new AsyncLazy<T>(async () =>
        {
            entry.SlidingExpiration = TimeSpan.FromSeconds(seconds);
            return await taskFactory.Invoke();
        }).Value);
        if (res == null) //remove from cache if null     
            _cache.Remove(comKey);
#if DEBUG
        _logger?.LogDebug($"{nameof(GetOrAddAsync)} {comKey} getting from cache");
#endif            
        return res;
    }

}

public class AsyncLazy<T> : Lazy<Task<T>>
{
    public AsyncLazy(Func<T> valueFactory) :
        base(() => Task.Factory.StartNew(valueFactory))
    { }
    public AsyncLazy(Func<Task<T>> taskFactory) :
        base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap())
    { }
}

