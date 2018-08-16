using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NC.Model
{
    /// <summary>
    /// 登录页的配置信息model
    /// </summary>
    public class LoginInfoModel
    {
        //系统名称
        public string ProjName { get; set; }
        //系统版本
        public string ProjVersion { get; set; }
        //cokies中的登录名Key
        public string cookiesName { get; set; }
        //记录的登录名
        public string rememberName { get; set; }

        public string TransferUrl { get; set; }
    }
}