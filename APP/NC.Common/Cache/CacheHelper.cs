/* --
 * 作者：魏亚东
 * 日期：2017-11-24
 * 描述说明：
 * Web站应用缓存 HttpContext.Cache
 * 更改历史：
 * 
 * --*/

using System;
//using System.Web.Caching;//netcore2.0不再提供支持

using Microsoft.Extensions.Caching.Memory;
namespace NC.Common
{
    /// <summary>
    /// 为当前 HTTP 请求获取 Cache 对象。
    /// </summary>
    public class CacheHelper
    {
        static MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        /// <summary>
        /// 创建缓存项的文件
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="obj">object对象</param>
        public static void Set(string key, object value)
        {
            if (key != null)
            {
                cache.Set(key, value);
            }
        }
        /// <summary>
        /// 创建缓存项过期
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="obj">object对象</param>
        /// <param name="expires">过期时间(秒)</param>
        public static void Set(string key, object value, int expires)
        {
            if (key != null)
            {
                cache.Set(key, value, new MemoryCacheEntryOptions()
                    //设置缓存时间，如果被访问重置缓存时间。设置相对过期时间x秒
                    .SetSlidingExpiration(TimeSpan.FromSeconds(expires)));
            }
        }
        //dotnetcore2.0 文件依赖缓存好像没有，暂未找到。
        ///// <summary>
        ///// 创建缓存项的文件依赖
        ///// </summary>
        ///// <param name="key">缓存Key</param>
        ///// <param name="obj">object对象</param>
        ///// <param name="fileName">文件绝对路径</param>
        //public static void Set(string key, object obj, string fileName)
        //{
        //    //创建缓存依赖项
        //    CacheDependency dep = new CacheDependency(fileName);
        //    //创建缓存
        //    HttpContext.Current.Cache.Insert(key, obj, dep);
        //}

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>object对象</returns>
        public static object Get(string key)
        {
            object val = null;
            if (key != null && cache.TryGetValue(key, out val))
            {

                return val;
            }
            else
            {
                return default(object);
            }
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <typeparam name="T">T对象</typeparam>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            object obj = Get(key);
            return obj == null ? default(T) : (T)obj;
        }


        /// <summary>
        /// 移除缓存项的文件
        /// </summary>
        /// <param name="key">缓存Key</param>
        public static void Remove(string key)
        {
            cache.Remove(key);
        }

    }
}
