#region --Copyright (C) 2017 wyd  邮箱：604401109@qq.com--
/* --*
* 作者：wyd
* 时间：2017-11-30 星期四 16:21:22
* 版本：v1.0.0
* GUID: 003e75da-de4f-4d40-8546-8597df28dd1f 
* 备注：memcached缓存帮助类
* Powered by wyd
* --*/
#endregion

using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace NC.Common
{
    /// <summary>
    /// 1.由于get提示【弃用的】，过时了，后续采用async/await  Task异步模式完善。
    /// 2.如果多台memcached服务器,服务器时间一定要调整一致。如果不一致就会出现存不上值或过期日期不准确。
    /// </summary>
    public class MemcachedHelper
    {
        private static string _DEPEND_ = "depend";
        private static int _EXP_ = 10 * 60; //默认缓存10分钟 
        private static int HH = 3600; //1小时=3600秒

        static readonly object mlock = new object();
        private static readonly ILoggerFactory _loggerFacotry = new LoggerFactory();
        /// <summary>  
        /// 定义一个静态MemcachedClient客户端,它随类一起加载，所有对象共用  
        /// </summary>  
        private static MemcachedClient mclient;
        /// <summary>
        /// 构造函数，连接memcachedcore并为KEYS字典开辟储存空间
        /// </summary>
        static MemcachedHelper()
        {

            if (mclient == null)
            {
                lock (mlock)
                {
                    var options = new MemcachedClientOptions();
                    UtilConf.Configuration.GetSection("enyimMemcached").Bind(options);
                    mclient = new MemcachedClient(_loggerFacotry, new MemcachedClientConfiguration(_loggerFacotry, options));
                }
            }
        }

        #region -- 获取缓存 --
        /// <summary>
        /// 获取缓存 
        /// </summary>
        public static object Get(string key)
        {
            try
            {
                key = _DEPEND_ + "_" + key.ToLower();
                return mclient.Get(key);
            }
            catch (Exception ex)
            {

                return "";
            }
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<object> GetAsnyAsync(string key)
        {
            try
            {
                key = _DEPEND_ + "_" + key.ToLower();
                return await mclient.GetValueAsync<object>(key);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>  
        /// 通过key 来得到一个对象  
        /// </summary>  
        /// <param name="depend">组名，用来区分不同的服务或应用场景</param>  
        /// <param name="key">键</param>  
        /// <returns>对象</returns>  
        public static object Get(string depend, string key)
        {
            key = depend + "_" + key;
            return mclient.Get(key);
        }
        /// <summary>  
        /// 通过key 来得到一个对象(前类型)
        /// </summary>  
        /// <typeparam name="T">类型</typeparam>  
        /// <param name="depend">组名，用来区分不同的服务或应用场景</param>  
        /// <param name="key">键</param>  
        /// <returns></returns>  
        public static T GetT<T>(string depend, string key)
        {
            key = depend + "_" + key;
            return mclient.Get<T>(key);
        }
        #endregion

        #region -- 添加缓存 --
        /// <summary>  
        /// 向Memcached缓存中添加一条数据  
        /// </summary>  
        /// <param name="depend">组名，用来区分不同的服务或应用场景</param>  
        /// <param name="key">键</param>  
        /// <param name="value">值</param>  
        /// <param name="expiry">过期时间(秒)</param>  
        public static void Set(string depend, string key, object value, int expiry)
        {
            key = depend + "_" + key;
            mclient.Add(key, value, expiry);
        }

        #region ++ Set的多种实现方式
        /// <summary>
        /// 默认时间
        /// </summary>
        public static void SetD(string depend, string key, object obj)
        {
            Set(depend, key, obj, _EXP_);
        }
        /// <summary>
        /// 默认Depend和时间
        /// </summary>
        public static void Set(string key, object obj)
        {
            Set(_DEPEND_, key, obj, _EXP_);
        }
        /// <summary>
        /// 默认Depend
        /// </summary>
        public static void Set(string key, object obj, int exp)
        {
            Set(_DEPEND_, key, obj, exp);
        }
        /// <summary>
        /// 长时间缓存
        /// </summary>
        public static void SetLong(string depend, string key, object obj)
        {
            int t = 31536000; //1年 = 10 * 365 * 24 * 60 * 60; 
            Set(depend, key, obj, t);
        }
        /// <summary>
        /// 长时间默认depend
        /// </summary>
        public static void SetLong(string key, object obj)
        {
            int t = 31536000; //365 * 24 * 60 * 60; //1年
            Set(_DEPEND_, key, obj, t);
        }
        public static void SetAllLong(string key, object obj)
        {
            int t = 315360000; //365 * 24 * 60; //10年
            Set(_DEPEND_, key, obj, t);
        }
        #endregion

        #endregion

        #region -- 删除缓存 --
        /// <summary>  
        /// 清除指定key的cache  
        /// </summary>  
        /// <param name="depend">组名，用来区分不同的服务或应用场景</param>  
        /// <param name="key">键</param>  
        /// <returns></returns>  
        public static bool Remove(string depend, string key)
        {
            key = depend + "_" + key;
            return mclient.Remove(key);
        }
        public static bool Remove(string key)
        {
            return Remove(_DEPEND_, key);
        }
        /// <summary>  
        /// 清除所有cache  
        /// </summary>  
        public static void RemoveAll()
        {
            mclient.FlushAll();
        }
        #endregion
    }
}