#region --Copyright (C) 2017 wyd  邮箱：604401109@qq.com--
/* --*
* 作者：wyd
* 时间：2017-11-16
* 版本：v1.0.0
* GUID: 8bd50800-d834-4459-bbee-489050b498eb 
* 备注：
* Powered by wyd
* --*/
#endregion

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NC.Common
{
    public class FileLogger : ILogger
    {
        private string name;
        private bool IsOpen;
        private string WPath;
        
        public FileLogger(string _name)
        {
            name = _name;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
        /// <summary>
        /// 是否禁用
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }
        /// <summary>
        /// 实现接口ILogger
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            IsOpen = UtilConf.GetSection("WLogger")["IsOpen"] == "true" ? true : false;
            if (IsOpen)
            {
                //获取日志信息
                var message = formatter?.Invoke(state, exception);
                //日志写入文件
                LogToFile(logLevel, message);
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="level">等级</param>
        /// <param name="message">日志内容</param>
        private void LogToFile(LogLevel level, string message)
        {
            var filename = GetFilename();
            var logContent = GetLogContent(level, message);
            File.AppendAllLines(filename, new List<string> { logContent }, Encoding.UTF8);
        }
        /// <summary>
        /// 获取日志内容
        /// </summary>
        /// <param name="level">等级</param>
        /// <param name="message">日志内容</param>
        /// <returns></returns>
        private string GetLogContent(LogLevel level, string message)
        {
            return $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.h3")}]{level}|{name}|{message}";
        }

        private string DirectorySeparatorChar = Path.DirectorySeparatorChar.ToString();//目录分隔符
        /// <summary>
        /// 获取文件名
        /// </summary>
        private string GetFilename()
        {
            var dir = "";
            WPath = UtilConf.GetSection("WLogger")["WPath"];
            if (WPath.IndexOf(":") > -1)
            {
                dir = WPath;
            }
            else
            {
                //此方法不是真正的获取应用程序的当前方法，而是执行dotnet命令所在目录
                dir = Directory.GetCurrentDirectory() + WPath;
                
            }
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var result = $"{dir}/WLog-{DateTime.Now.ToString("yyyy-MM-dd")}.txt".Replace("/",DirectorySeparatorChar);

            return result;
        }
    }
}
