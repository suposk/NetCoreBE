using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

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

public class KeyIdPair
{
    public KeyIdPair(string key, string id)
    {
        Key = key;
        Id = id;
    }
    public string? Key { get; set; }
    public string? Id { get; set; }

    #region GetHashCode and Equals

    public override int GetHashCode()
    {
        return (Key != null && Id != null) ? Key.GetHashCode() ^ Id.GetHashCode() : Key.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (ReferenceEquals(obj, this))
            return true;
        if (obj.GetType() != GetType())
            return false;

        KeyIdPair rhs = obj as KeyIdPair;
        return Key == rhs.Key && Id == rhs.Id;
    }

    public static bool operator ==(KeyIdPair lhs, KeyIdPair rhs) { return Equals(lhs, rhs); }
    public static bool operator !=(KeyIdPair lhs, KeyIdPair rhs) { return !Equals(lhs, rhs); }

    #endregion
}

public class CacheProvider : ICacheProvider
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheProvider> _logger;
    private readonly ConcurrentDictionary<KeyIdPair, object> _Dic = new();

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
        _Dic.AddOrUpdate(new KeyIdPair(key, value?.ToString()), value, (k, v) => value);
        _cache.Set(key, value, DateTimeOffset.Now.AddSeconds(seconds));
    }

    public void SetCache<T>(string key, string id, T value) where T : class
    {
        //if (string.IsNullOrWhiteSpace(id))
        //    throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));
        SetCache($"{key}-{id}", value);
    }

    public void SetCache<T>(string key, string id, T value, int seconds = ICacheProvider.CacheSeconds) where T : class
    {
        //if (string.IsNullOrWhiteSpace(id))
        //    throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));
        SetCache($"{key}-{id}", value, seconds);
    }

    public void ClearCache(string key)
    {
        
        var allPairs = _Dic.Where(x => x.Key.Key == key).ToList();
        if (allPairs.Count == 1 && allPairs.FirstOrDefault().Value == null)        
            _cache.Remove(key);        
        else
        {
            _cache.Remove(key);
            foreach (var pair in allPairs)
            {
                //_Dic.TryRemove(pair.Key, out _);
                _Dic.TryRemove(pair);
                _cache.Remove($"{pair.Key}-{pair.Value}");
            }
        }
    }

    public void ClearCache(string key, string id)
    {
        //if (string.IsNullOrWhiteSpace(id))
        //    throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));
        //_cache.Remove($"{key}-{id}");

        var allPairs = _Dic.Where(x => x.Key.Key == key).ToList();
        if (allPairs.Count == 1 && allPairs.FirstOrDefault().Value == null)
        {
            _cache.Remove(key);
            _cache.Remove($"{key}-{id}");
        }
        else
        {
            _cache.Remove(key);
            _cache.Remove($"{key}-{id}");
            foreach (var pair in allPairs)
            {
                //_Dic.TryRemove(pair.Key, out _);
                _Dic.TryRemove(pair);
                _cache.Remove($"{pair.Key}-{pair.Value}");
            }
        }
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
            _Dic.AddOrUpdate(new KeyIdPair(key, string.Empty), string.Empty, (k, v) => string.Empty);
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
            _Dic.AddOrUpdate(new KeyIdPair(key, id?.ToString()), id, (k, v) => id);
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

