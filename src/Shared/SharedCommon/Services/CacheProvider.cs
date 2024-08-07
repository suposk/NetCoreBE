using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SharedCommon.Helpers;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace SharedCommon.Services;

public interface ICacheProvider
{
    const int CacheSeconds = 300; //5 min
    
    void ClearCache(string key);
    //void ClearCache(string key, string id);
    void ClearCache(string key, params string[] otherParams);
    void ClearCacheForAllKeysAndIds(string key);
    void ClearCacheOnlyKeyAndId(string key, string id);
    Task<T> GetOrAddAsync<T>(string key, int seconds, Func<Task<T>> taskFactory);
    //Task<T> GetOrAddAsync<T>(string key, string id, int seconds, Func<Task<T>> taskFactory);
    Task<T> GetOrAddAsync<T>(string key, int seconds, Func<Task<T>> taskFactory, params string[] otherParams);
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
    //public string? Value { get; set; } //not used

    #region GetHashCode and Equals

    public override int GetHashCode()
    {
        if (Key.HasValueExt() && Id.HasValueExt())
            return Key.GetHashCode() ^ Id.GetHashCode();
        if (Key.HasValueExt())
            return Key.GetHashCode();
        return base.GetHashCode();
    }

    public override string ToString() => $"Key={Key} Id={Id}";

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
    private const string _separator = "_";

    public CacheProvider(IMemoryCache cache, ILogger<CacheProvider> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Either {key}-{id} or just {key} if id is null or empty
    /// </summary>
    /// <param name="key"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    string GetCombinedKey(string key, string id) => $"{(!string.IsNullOrWhiteSpace(id) ? $"{key}{_separator}{id}" : key)}";
    string GetCombinedKey(string key, params string [] otherParams) 
    {
        if (otherParams == null || otherParams.Length == 0)
            return key;
        return $"{key}{_separator}{GetParamsKey}";
    }

    string GetParamsKey(params string[] otherParams)
    {
        if (otherParams == null || otherParams.Length == 0)
            return string.Empty;
        return $"{string.Join(_separator, otherParams)}";
        //return $"{string.Join(string.Empty, otherParams)}";
    }

    public void SetCache<T>(string key, T value) where T : class => SetCache(key, value, ICacheProvider.CacheSeconds);

    public void SetCache<T>(string key, T value, int seconds = ICacheProvider.CacheSeconds) where T : class
    {
        _Dic.AddOrUpdate(new KeyIdPair(key, null), value, (k, v) => value);
        _cache.Set(key, value, DateTimeOffset.Now.AddSeconds(seconds));
    }

    public void SetCache<T>(string key, string id, T value) where T : class
    {
        _Dic.AddOrUpdate(new KeyIdPair(key, id), value, (k, v) => value);
        //_cache.Set($"{key}-{id}", value, DateTimeOffset.Now.AddSeconds(ICacheProvider.CacheSeconds));
        _cache.Set(GetCombinedKey(key, id), value, DateTimeOffset.Now.AddSeconds(ICacheProvider.CacheSeconds));
    }

    public void ClearCache(string key)
    {
        _cache.Remove(key);
        var allPairs = _Dic.Where(x => x.Key.Key == key).Select(a => a.Key).ToList();
        foreach (var pair in allPairs)
        {
            if (pair.Id.IsNullOrEmptyExt())
                _Dic.TryRemove(pair, out _);
        }
    }

    public void ClearCache(string key, params string[] otherParams)
    {
        string id = GetParamsKey(otherParams);
        _cache.Remove(GetCombinedKey(key, id));
        var allPairs = _Dic.Where(x => x.Key.Key == key).Select(a => a.Key).ToList();
        foreach (var pair in allPairs)
        {
            if (pair.Id.HasValueExt())
                _Dic.TryRemove(pair, out _);
        }
    }

    public void ClearCacheForAllKeysAndIds(string key)
    {
        _cache.Remove(key);
        var allPairs = _Dic.Where(x => x.Key.Key == key).Select(a => a.Key).ToList();
        foreach (var pair in allPairs)
        {
            _Dic.TryRemove(pair, out _);
            _cache.Remove(GetCombinedKey(pair.Key, pair.Id));
        }
    }

    public void ClearCacheOnlyKeyAndId(string key, string id)
    {
        _cache.Remove(key);
        var allPairs = _Dic.Where(x => x.Key.Key == key && x.Key.Id == id).Select(a => a.Key).ToList();
        foreach (var pair in allPairs)
        {
            _Dic.TryRemove(pair, out _);
            _cache.Remove(GetCombinedKey(pair.Key, pair.Id));
        }
    }


    public async Task<T> GetOrAddAsync<T>(string key, int seconds, Func<Task<T>> taskFactory)
    {
        bool setCache = false;
        var res = await _cache.GetOrCreateAsync(key, async entry => await new AsyncLazy<T>(async () =>
        {
            setCache = true;
            _Dic.AddOrUpdate(new KeyIdPair(key, null), string.Empty, (k, v) => v);
            entry.SlidingExpiration = TimeSpan.FromSeconds(seconds);
            return await taskFactory.Invoke();
        }).Value);
        if (res == null) //remove from cache if null
            _cache.Remove(key);
        _logger?.LogDebug($"{nameof(GetOrAddAsync)} {key} {(setCache ? "note in cache" : "getting from cache")}");
        return res;
    }

    public async Task<T> GetOrAddAsync<T>(string key, int seconds, Func<Task<T>> taskFactory, params string[] otherParams)
    {
        bool setCache = false;
        string id = GetParamsKey(otherParams);
        var comKey = GetCombinedKey(key, id);
        var res = await _cache.GetOrCreateAsync(comKey, async entry => await new AsyncLazy<T>(async () =>
        {
            setCache = true;
            _Dic.AddOrUpdate(new KeyIdPair(key, id), id, (k, v) => v);
            entry.SlidingExpiration = TimeSpan.FromSeconds(seconds);
            return await taskFactory.Invoke();
        }).Value);
        if (res == null) //remove from cache if null     
            _cache.Remove(comKey);
        _logger?.LogDebug($"{nameof(GetOrAddAsync)} {comKey} {(setCache ? "note in cache" : "getting from cache")}");
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

