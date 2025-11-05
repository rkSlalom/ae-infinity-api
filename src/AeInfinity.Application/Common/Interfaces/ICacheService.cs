namespace AeInfinity.Application.Common.Interfaces;

/// <summary>
/// Service for caching data with expiration support
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached value by key
    /// </summary>
    /// <typeparam name="T">Type of cached value</typeparam>
    /// <param name="key">Cache key</param>
    /// <returns>Cached value or null if not found/expired</returns>
    Task<T?> GetAsync<T>(string key) where T : class;
    
    /// <summary>
    /// Sets a value in cache with expiration
    /// </summary>
    /// <typeparam name="T">Type of value to cache</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="value">Value to cache</param>
    /// <param name="expiration">Expiration time</param>
    Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class;
    
    /// <summary>
    /// Removes a value from cache
    /// </summary>
    /// <param name="key">Cache key</param>
    Task RemoveAsync(string key);
}

