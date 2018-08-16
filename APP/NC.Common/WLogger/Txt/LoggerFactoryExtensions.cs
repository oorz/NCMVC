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

namespace NC.Common
{
    public static class LoggerFactoryExtensions
    {
        public static ILoggerFactory AddFileLogger(this ILoggerFactory factory)
        {
            factory.AddProvider(new FileLoggerProvider());
            return factory;
        }
    }
}
