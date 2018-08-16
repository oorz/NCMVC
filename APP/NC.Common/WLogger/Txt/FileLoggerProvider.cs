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
    public class FileLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// 默认构造函数，根据Provider进此构造函数
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(categoryName);
        }

        public void Dispose()
        {
        }
    }
}
