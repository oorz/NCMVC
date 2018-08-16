#region --Copyright (C) 2017 wyd  邮箱：604401109@qq.com--
/* --*
* 作者：wyd
* 时间：2017-12-11 星期一 11:47:46
* 版本：v1.0.0
* GUID: 63aeaeb3-73bb-47e4-ae4b-9912b5258a3d 
* 备注：
* Powered by wyd
* --*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace NC.Common
{
    public class JHKeys
    {
        //系统版本
        /// <summary>
        /// 版本号全称
        /// </summary>
        public const string ASSEMBLY_VERSION = "1.0.0";
        /// <summary>
        /// 版本年号
        /// </summary>
        public const string ASSEMBLY_YEAR = "2017";
        //File======================================================
        /// <summary>
        /// 插件配制文件名
        /// </summary>
        public const string FILE_PLUGIN_XML_CONFING = "plugin.config";
        /// <summary>
        /// 站点配置文件名
        /// </summary>
        public const string FILE_SYS_XML_CONFING = "Configpath";
        /// <summary>
        /// URL配置文件名
        /// </summary>
        public const string FILE_URL_XML_CONFING = "Urlspath";
        /// <summary>
        /// 用户配置文件名
        /// </summary>
        public const string FILE_USER_XML_CONFING = "Userpath";
        /// <summary>
        /// 订单配置文件名
        /// </summary>
        public const string FILE_ORDER_XML_CONFING = "Orderpath";
        /// <summary>
        /// 升级代码
        /// </summary>
        public const string FILE_URL_UPGRADE_CODE = "";
        /// <summary>
        /// 消息代码
        /// </summary>
        public const string FILE_URL_NOTICE_CODE = "";

        //Directory==================================================
        /// <summary>
        /// ASPX目录名
        /// </summary>
        public const string DIRECTORY_REWRITE_ASPX = "aspx";
        /// <summary>
        /// HTML目录名
        /// </summary>
        public const string DIRECTORY_REWRITE_HTML = "html";
        /// <summary>
        /// 插件目录名
        /// </summary>
        //public const string DIRECTORY_REWRITE_PLUGIN = "plugin";

        //Cache======================================================
        /// <summary>
        /// 站点配置
        /// </summary>
        public const string CACHE_SYS_CONFIG = "nc_cache_sys_config";
        /// <summary>
        /// 用户配置
        /// </summary>
        public const string CACHE_USER_CONFIG = "nc_cache_user_config";
        /// <summary>
        /// 订单配置
        /// </summary>
        public const string CACHE_ORDER_CONFIG = "nc_cache_order_config";
        /// <summary>
        /// HttpModule映射类
        /// </summary>
        public const string CACHE_SITE_HTTP_MODULE = "nc_cache_http_module";
        /// <summary>
        /// 绑定域名
        /// </summary>
        public const string CACHE_SITE_HTTP_DOMAIN = "nc_cache_http_domain";
        /// <summary>
        /// 站点一级目录名
        /// </summary>
        public const string CACHE_SITE_DIRECTORY = "nc_cache_site_directory";
        /// <summary>
        /// 站点ASPX目录名
        /// </summary>
        public const string CACHE_SITE_ASPX_DIRECTORY = "nc_cache_site_aspx_directory";
        /// <summary>
        /// URL重写映射表
        /// </summary>
        public const string CACHE_SITE_URLS = "nc_cache_site_urls";
        /// <summary>
        /// URL重写LIST列表
        /// </summary>
        public const string CACHE_SITE_URLS_LIST = "nc_cache_site_urls_list";
        /// <summary>
        /// 站点所有频道键值对
        /// </summary>
        public const string CACHE_SITE_CHANNEL_LIST = "nc_cache_site_channel_list";
        /// <summary>
        /// 升级通知
        /// </summary>
        public const string CACHE_OFFICIAL_UPGRADE = "nc_official_upgrade";
        /// <summary>
        /// 官方消息
        /// </summary>
        public const string CACHE_OFFICIAL_NOTICE = "nc_official_notice";

        //Session=====================================================
        /// <summary>
        /// 网页验证码
        /// </summary>
        public const string SESSION_CODE = "nc_session_code";
        /// <summary>
        /// 短信验证码
        /// </summary>
        public const string SESSION_SMS_CODE = "nc_session_sms_code";
        /// <summary>
        /// 后台管理员
        /// </summary>
        public const string SESSION_ADMIN_INFO = "nc_session_admin_info";
        /// <summary>
        /// 会员用户
        /// </summary>
        public const string SESSION_USER_INFO = "nc_session_user_info";

        //Cookies=====================================================
        /// <summary>
        /// 防重复顶踩KEY
        /// </summary>
        public const string COOKIE_DIGG_KEY = "nc_cookie_digg_key";
        /// <summary>
        /// 防重复评论KEY
        /// </summary>
        public const string COOKIE_COMMENT_KEY = "nc_cookie_comment_key";
        /// <summary>
        /// 记住会员用户名
        /// </summary>
        public const string COOKIE_USER_NAME_REMEMBER = "nc_cookie_user_name_remember";
        /// <summary>
        /// 记住会员密码
        /// </summary>
        public const string COOKIE_USER_PWD_REMEMBER = "nc_cookie_user_pwd_remember";
        /// <summary>
        /// 用户手机号码
        /// </summary>
        public const string COOKIE_USER_MOBILE = "nc_cookie_user_mobile";
        /// <summary>
        /// 用户电子邮箱
        /// </summary>
        public const string COOKIE_USER_EMAIL = "nc_cookie_user_email";
        /// <summary>
        /// 购物车
        /// </summary>
        public const string COOKIE_SHOPPING_CART = "nc_cookie_shopping_cart";
        /// <summary>
        /// 结账清单
        /// </summary>
        public const string COOKIE_SHOPPING_BUY = "nc_cookie_shopping_buy";
        /// <summary>
        /// 返回上一页
        /// </summary>
        public const string COOKIE_URL_REFERRER = "nc_cookie_url_referrer";

        //Table=======================================================
        /// <summary>
        /// 频道文章表前缀
        /// </summary>
        public const string TABLE_CHANNEL_ARTICLE = "channel_article_";
    }
}
