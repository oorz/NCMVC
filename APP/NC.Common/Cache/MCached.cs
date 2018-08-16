#region --Copyright (C) 2017 wyd  邮箱：604401109@qq.com--
/* --*
* 作者：wyd
* 时间：2017-12-01 星期五 17:39:36
* 版本：v1.0.0
* GUID: a436f6f7-9c8a-4013-8812-3602cac45d58 
* 备注：
* Powered by wyd
* --*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Net;
namespace NC.Common
{
    #region --用工厂模式解决ASP.NET Core中依赖注入的一个烦恼--
    //http://www.bubuko.com/infodetail-2080598.html
    public interface IMemcachedClientFactory
    {
        IMemcachedClientFactory Add(string keyOfConfiguration);
        IMemcachedClient Create(string keyOfConfiguration);
    }
    public class MemcachedClientFactory : IMemcachedClientFactory
    {
        private readonly ILoggerFactory _loggerFacotry;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, IMemcachedClient> _clients = new Dictionary<string, IMemcachedClient>();

        public MemcachedClientFactory(
            ILoggerFactory loggerFacotry,
            IConfiguration configuration)
        {
            _loggerFacotry = loggerFacotry;
            _configuration = configuration;
        }

        public IMemcachedClientFactory Add(string keyOfConfiguration)
        {
            var options = new MemcachedClientOptions();
            _configuration.GetSection(keyOfConfiguration).Bind(options);

            var memcachedClient = new MemcachedClient(
                _loggerFacotry,
                new MemcachedClientConfiguration(_loggerFacotry, options));

            _clients.Add(keyOfConfiguration, memcachedClient);

            return this;
        }

        public IMemcachedClient Create(string keyOfConfiguration)
        {
            return _clients[keyOfConfiguration];
        }
    }
    #endregion

    /// <summary>  
    /// MemcachedClient 配置类  
    /// </summary>  
    public sealed class MemCached
    {
        private static MemcachedClient MemClient;
        static readonly object padlock = new object();
        //线程安全的单例模式  
        public static MemcachedClient getInstance()
        {
            if (MemClient == null)
            {
                lock (padlock)
                {
                    if (MemClient == null)
                    {
                        MemClientInit();
                    }
                }
            }
            return MemClient;
        }
        private static readonly ILoggerFactory _loggerFacotry = new LoggerFactory();
        static void MemClientInit()
        {
            var options = new MemcachedClientOptions();
            UtilConf.Configuration.GetSection("enyimMemcached").Bind(options);

            MemClient = new MemcachedClient(
                _loggerFacotry,
                new MemcachedClientConfiguration(_loggerFacotry, options));
        }
    }
    public class MCached
    {  /// <summary>  
       /// 定义一个静态MemcachedClient客户端,它随类一起加载，所有对象共用  
       /// </summary>  
        private static MemcachedClient mclient;
        /// <summary>  
        /// 静态构造函数，初始化Memcached客户端  
        /// </summary>  
        static MCached()
        {
            mclient = MemCached.getInstance();
        }

        /// <summary>  
        /// 向Memcached缓存中添加一条数据  
        /// </summary>  
        /// <param name="groupName">组名，用来区分不同的服务或应用场景</param>  
        /// <param name="key">键</param>  
        /// <param name="value">值</param>  
        /// <param name="expiry">过期时间(秒)</param>  
        public static void Set(string groupName, string key, object value, int expiry)
        {
            key = groupName + "-" + key;
            mclient.Add(key, value, expiry);
        }
        /// <summary>  
        /// 通过key 来得到一个对象  
        /// </summary>  
        /// <param name="groupName">组名，用来区分不同的服务或应用场景</param>  
        /// <param name="key">键</param>  
        /// <returns>对象</returns>  
        public static object Get(string groupName, string key)
        {
            key = groupName + "_" + key;
            return mclient.Get(key);
        }
        /// <summary>  
        /// 通过key 来得到一个对象(前类型)
        /// </summary>  
        /// <typeparam name="T">类型</typeparam>  
        /// <param name="groupName">组名，用来区分不同的服务或应用场景</param>  
        /// <param name="key">键</param>  
        /// <returns></returns>  
        public static T GetT<T>(string groupName, string key)
        {
            key = groupName + "-" + key;
            return mclient.Get<T>(key);
        }
        /// <summary>  
        /// 清除指定key的cache  
        /// </summary>  
        /// <param name="groupName">组名，用来区分不同的服务或应用场景</param>  
        /// <param name="key">键</param>  
        /// <returns></returns>  
        public static bool Remove(string groupName, string key)
        {
            key = groupName + "-" + key;
            return mclient.Remove(key);
        }
        /// <summary>  
        /// 清除所有cache  
        /// </summary>  
        public static void RemoveAll()
        {
            mclient.FlushAll();
        }
    }
}
