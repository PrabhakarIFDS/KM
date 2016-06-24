using System;
using System.Web;

//using System.Runtime.Caching;   
namespace IFDS.KMPortal.Helper
{
    public static class CacheHelper
    {
        /// <summary>
        /// Save the data in cache in NoAbsoluteExpiration mode
        /// </summary>        
        public static void SaveTocache(string cacheKey, object savedItem, DateTime absoluteExpiration)
        {
            if (IsIncache(cacheKey))
            {
                HttpContext.Current.Cache.Remove(cacheKey);
            }

            HttpContext.Current.Cache.Add(cacheKey, savedItem, null,System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, 60), System.Web.Caching.CacheItemPriority.Default, null);
        }

        /// <summary>
        /// Save the data in cache in NoSlidingExpiration mode
        /// </summary>       
        public static void SaveTocache(string cacheKey, object savedItem)
        {
            if (IsIncache(cacheKey))
            {
                HttpContext.Current.Cache.Remove(cacheKey);
            }

            HttpContext.Current.Cache.Add(cacheKey, savedItem, null, DateTime.Now.AddMinutes(3), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);            
        }

        /// <summary>
        /// Get the data from Cache
        /// </summary>       
        public static object GetFromCache(string cacheKey) 
        {
            return HttpContext.Current.Cache[cacheKey];
        }

        /// <summary>
        /// Remove the data from Cache
        /// </summary>        
        public static void RemoveFromCache(string cacheKey)
        {
            HttpContext.Current.Cache.Remove(cacheKey);
        }

        /// <summary>
        /// Check if the Cache is not NULL.
        /// </summary>        
        public static bool IsIncache(string cacheKey)
        {
            return HttpContext.Current.Cache[cacheKey] != null;
        }
    }
}
