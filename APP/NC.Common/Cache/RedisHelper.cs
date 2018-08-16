#region --Copyright (C) 2017 wyd  邮箱：604401109@qq.com--
/* --*
* 作者：wyd
* 时间：2017-11-24 星期五 17:29:43
* 版本：v1.0.0
* GUID: ac463856-137f-43f0-aa1d-9a020787a5b2 
* 备注：
* Powered by wyd
* --*/
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using StackExchange.Redis;
using Newtonsoft.Json;

namespace NC.Common
{
    public class RedisHelper
    {
        //单例模式
        public static RedisCommon Default{get { return new RedisCommon(); } }
        public static RedisCommon One { get { return new RedisCommon(1, "127.0.0.1:6379"); } }
        public static RedisCommon Two { get { return new RedisCommon(2, "127.0.0.1:6379"); } }
    }
    /// <summary>
    /// Redis操作类
    /// 老版用的是ServiceStack.Redis。
    /// Net Core使用StackExchange.Redis的nuget包
    /// </summary>
    public class RedisCommon
    {
        public static ILogger Log = UtilLogger<RedisCommon>.Log;//日志记录
        //redis数据库连接字符串
        private string _conn = UtilConf.Configuration["RedisConfig:ReadWriteHosts"] ?? "127.0.0.1:6379";
        private int _db = 0;
        //静态变量 保证各模块使用的是不同实例的相同链接
        private static ConnectionMultiplexer connection;
        public RedisCommon() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="db"></param>
        /// <param name="connectStr"></param>
        public RedisCommon(int db, string connectStr)
        {
            _conn = connectStr;
            _db = db;
        }
        /// <summary>
        /// 缓存数据库，数据库连接
        /// </summary>
        public ConnectionMultiplexer CacheConnection
        {
            get
            {
                try
                {
                    if (connection == null || !connection.IsConnected)
                    {
                        connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_conn)).Value;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError("RedisHelper->CacheConnection 出错\r\n" + ex.ToString());
                    return null;
                }
                return connection;
            }
        }
        /// <summary>
        /// 缓存数据库
        /// </summary>
        public IDatabase CacheRedis => CacheConnection.GetDatabase(_db);

        #region --KEY/VALUE存取--
        /// <summary>
        /// 单条存值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool StringSet(string key, string value)
        {
            return CacheRedis.StringSet(key, value);
        }
        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool StringSet(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            return CacheRedis.StringSet(key, value, expiry);
        }
        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="arr">key</param>
        /// <returns></returns>
        public bool StringSet(KeyValuePair<RedisKey, RedisValue>[] arr)
        {
            return CacheRedis.StringSet(arr);
        }
        /// <summary>
        /// 批量存值
        /// </summary>
        /// <param name="keysStr">key</param>
        /// <param name="valuesStr">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool StringSetMany(string[] keysStr, string[] valuesStr)
        {
            var count = keysStr.Length;
            var keyValuePair = new KeyValuePair<RedisKey, RedisValue>[count];
            for (int i = 0; i < count; i++)
            {
                keyValuePair[i] = new KeyValuePair<RedisKey, RedisValue>(keysStr[i], valuesStr[i]);
            }

            return CacheRedis.StringSet(keyValuePair);
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool SetStringKey<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
        {
            string json = JsonConvert.SerializeObject(obj);
            return CacheRedis.StringSet(key, json, expiry);
        }
        /// <summary>
        /// 追加值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void StringAppend(string key, string value)
        {
            ////追加值，返回追加后长度
            long appendlong = CacheRedis.StringAppend(key, value);
        }

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        public RedisValue GetStringKey(string key)
        {
            return CacheRedis.StringGet(key);
        }
        /// <summary>
        /// 根据Key获取值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>System.String.</returns>
        public string StringGet(string key)
        {
            try
            {
                return CacheRedis.StringGet(key);
            }
            catch (Exception ex)
            {
                Log.LogError("RedisHelper->StringGet 出错\r\n" + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public RedisValue[] GetStringKey(List<RedisKey> listKey)
        {
            return CacheRedis.StringGet(listKey.ToArray());
        }
        /// <summary>
        /// 批量获取值
        /// </summary>
        public string[] StringGetMany(string[] keyStrs)
        {
            var count = keyStrs.Length;
            var keys = new RedisKey[count];
            var addrs = new string[count];

            for (var i = 0; i < count; i++)
            {
                keys[i] = keyStrs[i];
            }
            try
            {

                var values = CacheRedis.StringGet(keys);
                for (var i = 0; i < values.Length; i++)
                {
                    addrs[i] = values[i];
                }
                return addrs;
            }
            catch (Exception ex)
            {
                Log.LogError("RedisHelper->StringGetMany 出错\r\n" + ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetStringKey<T>(string key)
        {
            return JsonConvert.DeserializeObject<T>(CacheRedis.StringGet(key));
        }

        #endregion

        #region --删除设置过期--
        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        public bool KeyDelete(string key)
        {
            return CacheRedis.KeyDelete(key);
        }
        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">rediskey</param>
        /// <returns>成功删除的个数</returns>
        public long KeyDelete(RedisKey[] keys)
        {
            return CacheRedis.KeyDelete(keys);
        }
        /// <summary>
        /// 判断key是否存储
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            return CacheRedis.KeyExists(key);
        }
        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public bool KeyRename(string key, string newKey)
        {
            return CacheRedis.KeyRename(key, newKey);
        }
        /// <summary>
        /// 删除hasekey
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public bool HaseDelete(RedisKey key, RedisValue hashField)
        {
            return CacheRedis.HashDelete(key, hashField);
        }
        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashRemove(string key, string dataKey)
        {
            return CacheRedis.HashDelete(key, dataKey);
        }
        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public void SetExpire(string key, DateTime datetime)
        {
            CacheRedis.KeyExpire(key, datetime);
        }
        #endregion

    }
}
