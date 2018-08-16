#region --Copyright (C) 2017 wyd  邮箱：604401109@qq.com--
/* --*
* 作者：wyd
* 时间：2017-11-16 星期四 15:44:42
* 版本：v1.0.0
* GUID: 548c4bc6-d44f-4eab-a199-2c2cb2c17dbb 
* 备注：日志工具
* Powered by wyd
* --*/
#endregion

using Microsoft.Extensions.Logging;

namespace NC.Common
{
    public class UtilLogger<T>
    {
        private static ILogger iLog;
        public static ILogger Log
        {
            get
            {
                if (iLog != null) return iLog;

                ////第一种写法
                //ILoggerFactory loggerFactory = new LoggerFactory();
                //loggerFactory.AddFileLogger();
                //iLog = loggerFactory.CreateLogger<DbCommand>();

                //第二种写法
                iLog = new LoggerFactory().AddFileLogger().CreateLogger<T>();
                return iLog;
            }
            set => iLog = value;
        }
    }
}
