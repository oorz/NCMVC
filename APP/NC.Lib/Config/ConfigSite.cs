using NC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.Lib
{
    public class ConfigSite
    {
        #region--站点配置--
        /// <summary>
        ///  读取配置文件
        /// </summary>
        public NC.Model.siteconfig loadConfig()
        {
            NC.Model.siteconfig model = CacheHelper.Get<NC.Model.siteconfig>(JHKeys.CACHE_SYS_CONFIG);
            if (model == null)
            {
                //netcore2.0文件依赖缓存没有，暂时只能修改文件后手动清空一次缓存
                 //Utils.GetXmlMapPath(JHKeys.FILE_SYS_XML_CONFING);
                CacheHelper.Set(JHKeys.CACHE_SYS_CONFIG, (NC.Model.siteconfig)SerializationHelper.Load(typeof(NC.Model.siteconfig), Utils.GetXmlMapPath(JHKeys.FILE_SYS_XML_CONFING)));
                model = CacheHelper.Get<NC.Model.siteconfig>(JHKeys.CACHE_SYS_CONFIG);
            }
            return model;
        }
        /// <summary>
        ///  保存配置文件
        /// </summary>
        public NC.Model.siteconfig saveConifg(NC.Model.siteconfig model)
        {
            return saveConifg(model, Utils.GetXmlMapPath(JHKeys.FILE_SYS_XML_CONFING));
        }
        private static object lockHelper = new object();
        /// <summary>
        /// 写入站点配置文件
        /// </summary>
        public NC.Model.siteconfig saveConifg(NC.Model.siteconfig model, string configFilePath)
        {
            CacheHelper.Remove(JHKeys.CACHE_SYS_CONFIG);
            lock (lockHelper)
            {
                SerializationHelper.Save(model, configFilePath);
            }
            return model;
        }
        #endregion
    }
}
