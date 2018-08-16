using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NC.Common;
using NC.Core;
using NC.Model.Models;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace NC.Lib
{
    /// <summary>
    /// 后台通用BASE CONTROLLER
    /// </summary>
    [AdminAuthorize]
    public class AdminBase : Controller
    {

        public Model.siteconfig siteConf = new Model.siteconfig();
        public NcManager siteAdminInfo = new NcManager();
        public ConfigSite configSite = new ConfigSite();
        public NCMVC_DBContext dblEf;//操作EF

        public AdminBase(NCMVC_DBContext _dblEf)
        {
            dblEf = _dblEf;
            siteConf = configSite.loadConfig();
        }
        public AdminBase()
        {

        }
        /// <summary>
        /// 判断管理员是否已经登录(解决Session超时问题)
        /// </summary>
        public bool IsAdminLogin()
        {
            var bSession = HttpContext.Session.Get(AdminAuthorizeAttribute.AdminAuthenticationScheme);
            if (bSession == null)
            {
                return false;
            }
            siteAdminInfo = ByteConvertHelper.Bytes2Object<NcManager>(bSession);
            //如果Session为Null
            if (siteAdminInfo != null)
            {
                return true;
            }
            else
            {
                //检查Cookies
                var cookieAdmin = HttpContext.AuthenticateAsync(AdminAuthorizeAttribute.AdminAuthenticationScheme);
                cookieAdmin.Wait();
                var adminname = cookieAdmin.Result.Principal.Claims.FirstOrDefault(x => x.Type == "AdminName")?.Value;
                var adminpwd = cookieAdmin.Result.Principal.Claims.FirstOrDefault(x => x.Type == "AdminPwd")?.Value;

                if (adminname != "" && adminpwd != "")
                {
                    NcManager model = dblEf.NcManager.Where(m => m.UserName == adminname && m.Password == adminpwd).FirstOrDefault();
                    if (model != null)
                    {
                        HttpContext.Session.Set(AdminAuthorizeAttribute.AdminAuthenticationScheme, ByteConvertHelper.Object2Bytes(model));//存储session
                        bSession = HttpContext.Session.Get(AdminAuthorizeAttribute.AdminAuthenticationScheme);
                        siteAdminInfo = ByteConvertHelper.Bytes2Object<NcManager>(bSession);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 创建过滤器：***全局过滤器*** 过滤除登录登出等操作权限验证
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //1.验证是否登录
            //2.验证菜单权限
            //3.验证按钮权限
            //在action执行之前

            //验证是否登录
            if (!IsAdminLogin())
            {
                string msg = string.Format("未登录或登录超时，请重新登录！");
                filterContext.Result = new RedirectResult("~/ad_min/login?msg=" + WebUtility.UrlEncode(msg));
                return;
            }

            //判断是否加有SkipAdmin标签
            var skipAuthorize = filterContext.ActionDescriptor.FilterDescriptors.Where(a => a.Filter is SkipAdminAuthorizeAttribute).Any();
            if (!skipAuthorize)
            {
                //是否系统管理文件夹里文件,Areas》ad_min
                var isPermission = false;
                //获取controller和action
                var route = filterContext.RouteData.Values;

                string strArea = route["area"].ToString();//获取区域的名字,ad_min区域下的都需要权限验证
                if (strArea != null && strArea.Equals("ad_min"))
                {
                    isPermission = true;
                }
                //需要验证权限
                if (isPermission)
                {
                    var currController = route["controller"].ToString();
                    var curraction = route["action"].ToString();
                    var exceptCtr = UtilConf.Configuration["Site:exceptCtr"].Replace("，", ",");//防止中文逗号
                    var exceptAction = UtilConf.Configuration["Site:exceptAction"].Replace("，", ",");//防止中文逗号
                    //判断是否有例外控制器或Action校验是否例外，跳过验证
                    if (!exceptCtr.Contains(currController.ToLower()) && !exceptAction.Contains(curraction.ToLower()))
                    {
                        //验证菜单权限
                        //验证按钮权限
                        //自定义方法属性
                        try
                        {
                            if (siteAdminInfo.RoleType != 1)//超管拥有所有权限
                            {
                                //获取属性
                                NavAttr actionAttr = filterContext.ActionDescriptor.FilterDescriptors.Where(a => a.Filter is NavAttr).Select(a => a.Filter).FirstOrDefault() as NavAttr;
                                string strNavName = string.Empty;
                                string strActionType = string.Empty;
                                if (actionAttr == null)
                                {
                                    actionAttr = filterContext.ActionDescriptor.FilterDescriptors.GetType().GetCustomAttributes<NavAttr>().FirstOrDefault() as NavAttr;
                                }
                                if (actionAttr != null)
                                {
                                    strNavName = actionAttr.NavName;
                                    strActionType = actionAttr.ActionType;
                                }
                                //获取参数,由于action在mvc中属于关键词，所以使用act当作操作方式参数
                                string paramAction = "";
                                if (Request.Method.ToLower() == "get" || Request.QueryString.HasValue)//判断是get请求
                                {
                                    paramAction = Request.Query["act"].ToString();
                                    if (string.IsNullOrEmpty(paramAction))
                                    {
                                        if (route["act"] != null)
                                        {
                                            paramAction = route["act"].ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    paramAction = Request.Form["act"].ToString();//post请求，只有post下且有参数才可以用写Request.Form，否则报异常。
                                    if (string.IsNullOrEmpty(paramAction))
                                    {
                                        if (route["act"] != null)
                                        {
                                            paramAction = route["act"].ToString();
                                        }
                                    }
                                }
                                if (!ChkPermission(siteAdminInfo.RoleId, currController, curraction, strNavName, strActionType, paramAction))
                                {
                                    TempData["Permission"] = "您没有管理" + "currController:" + currController + ";curraction:" + curraction + ";NavName:" + strNavName + ";strActionType:" + strActionType + ";paramAction:" + paramAction + "页面的权限，请联系管理员！";
                                    filterContext.Result = new RedirectResult("~/ad_min/Home/Index");
                                    return;
                                    //返回固定错误json
                                }
                                else
                                {
                                    TempData["Permission"] = null;
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
        }

        #region --页面权限验证--
        /// <summary>
        /// 判断页面
        /// </summary>
        /// <param name="role_id">角色id</param>
        /// <param name="currController">当前控制器</param>
        /// <param name="currAction">当前</param>
        /// <param name="navName">方法上的属性</param>
        /// <param name="actionType">操作类型</param>
        /// <param name="paramAction">当为操作方法是传递的参数</param>
        /// <returns>没有权限返回false</returns>
        public bool ChkPermission(int? role_id, string currController, string currAction, string navName, string actionType, string paramAction)
        {
            //1.未配置页面，在方法上加属性/ad_min/Settings/SysConfigSave
            //2.控制器+Action  /admin/sys_config/index,/admin/sys_config/add,/admin/sys_config/edit
            //3.先判断已配置页面/admin/settings/sys_config
            bool result = true;
            var url = HttpContext.Request.Path.Value;
            if (url.Contains("/ad_min/home/index"))//后台首页不验证
            {
                return result;
            }
            DataTable dt = chkPermission(role_id);
            var action_type = actionType;
            //属性不为空
            if (!string.IsNullOrEmpty(navName) && !string.IsNullOrEmpty(actionType))//属性全
            {
                DataRow[] dr = dt.Select("nav_name='" + navName + "' and action_type='" + action_type + "'");
                result = (dr.Count() > 0);
            }
            else if (!string.IsNullOrEmpty(navName) && string.IsNullOrEmpty(actionType))//属性只有nav_name
            {
                action_type = getActionType(currAction, paramAction);
                DataRow[] dr = dt.Select("nav_name='" + navName + "' and action_type='" + action_type + "'");
                result = (dr.Count() > 0);
            }
            else if (string.IsNullOrEmpty(currController) && !string.IsNullOrEmpty(actionType))//控制器名：nav_name,属性只有action_type
            {
                DataRow[] dr = dt.Select("nav_name='" + currController + "' and action_type='" + action_type + "'");
                result = (dr.Count() > 0);
            }
            else
            {
                //约定大于配置
                //控制器名：nav_name
                //Action：action_type
                if (!string.IsNullOrEmpty(currController) && !string.IsNullOrEmpty(currAction))
                {
                    //控制器+action
                    if (currAction.ToLower() == "index")//首页为展示
                    {
                        currAction = "View";
                    }
                    DataRow[] dr = dt.Select("nav_name='" + currController + "' and (action_type='" + currAction + "')");
                    result = (dr.Count() > 0);

                }
                //属性全空，控制器+Action验证不通过，参数不空
                if (!result && !string.IsNullOrEmpty(currController) && !string.IsNullOrEmpty(paramAction))//（控制器）+参数判断
                {
                    //参数可为Edit,Add,Del...
                    DataRow[] dr = dt.Select("nav_name='" + currController + "' and action_type='" + paramAction + "'");
                    result = (dr.Count() > 0);
                }
                if (!result)//控制器+Action验证未通过，根据url连接判断，如果url连接在数据库中配置的有则可以通过url获取此连接有哪些权限。
                {
                    //配置页面处理
                    DataTable dtNav = GetNavCacheList("link_url='" + url + "'");//根据菜单URL,从缓存中检索调用ID
                    if (dtNav.Rows.Count > 0)
                    {
                        DataRow drNav = dtNav.Rows[0];
                        string nav_name = drNav["name"].ToString();//nav_name
                        action_type = getActionType(currAction, paramAction);

                        DataRow[] dr = dt.Select("nav_name='" + nav_name + "' and action_type='" + action_type + "'");
                        result = (dr.Count() > 0);
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 判断是否有权限
        /// </summary>
        private DataTable chkPermission(int? role_id)
        {
            DataTable dt = CacheHelper.Get("permisson" + role_id) as DataTable;
            if (dt == null)
            {
                string strSql = "SELECT mrv.nav_name,mrv.action_type FROM nc_manager_role mr LEFT JOIN nc_manager_role_value mrv ON mr.id=mrv.role_id WHERE mr.id=@role_id";
                DbParameters p = new DbParameters();
                p.Add("@role_id", role_id);
                dt = Dbl.ZZCMS.CreateSqlDataTable(strSql, p);
                CacheHelper.Set("permisson" + role_id, dt);
            }
            //string strSql = "SELECT mr.role_type,mrv.role_id,mrv.nav_name,mrv.action_type FROM nc_manager_role mr LEFT JOIN nc_manager_role_value mrv ON mr.id=mrv.role_id WHERE mr.id=@role_id";
            //DbParameters p = new DbParameters();
            //p.Add("@role_id", role_id);
            //DataTable dt = Dbl.ZZCMS.CreateSqlDataTable(strSql, p);
            return dt;
        }
        /// <summary>
        /// 1.验证action是否标准约定
        /// 2.根据action=''参数获取操作类型
        /// </summary>
        private string getActionType(string currAction, string paramAction)
        {
            if (currAction.ToLower().Contains("index") || currAction.ToLower().Contains("list"))//首先判断是否首页/列表等展示
            {
                return "View";
            }
            if (currAction.ToLower().Contains("save"))//如果包含保存save关键字，默认返回add
            {
                return string.IsNullOrEmpty(paramAction) ? "Add" : paramAction;
            }
            else if (currAction.ToLower().Contains("edit") || currAction.ToLower().Contains("update"))
            {
                return string.IsNullOrEmpty(paramAction) ? "Edit" : paramAction;
            }
            else if (currAction.ToLower().Contains("del"))
            {
                return string.IsNullOrEmpty(paramAction) ? "Delete" : paramAction;
            }
            //判断Action
            if (!string.IsNullOrEmpty(currAction))
            {
                if (Utils.ActionType().ContainsKey(currAction))//首字母要大写，约定
                    return currAction;
            }
            return string.IsNullOrEmpty(paramAction) ? "View" : paramAction;
        }
        #endregion

        #region --*Cache*--
        /// <summary>
        /// 使用系统缓存
        /// </summary>
        public DataTable GetNavCacheList()
        {
            CacheHelper.Remove("GetNavCacheList");
            DataTable dt = CacheHelper.Get("GetNavCacheList") as DataTable;
            if (dt == null)
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("select id,nav_type,name,title,sub_title,icon_url,link_url,sort_id,is_lock,remark,parent_id,channel_id,action_type,is_sys ");
                strSql.Append(" FROM nc_navigation order by sort_id asc,id asc");
                dt = Dbl.ZZCMS.CreateSqlDataTable(strSql.ToString());
                CacheHelper.Set("GetNavCacheList", dt, 30);//缓存30分钟
            }
            return dt;
        }

        /// <summary>
        /// 根据条件检索缓存数据
        /// </summary>
        public DataTable GetNavCacheList(string strWhere)
        {
            //DataTable dt = GetCacheList();
            DataTable dt = GetNavList();
            return Dbl.GetNewTable(dt, strWhere);
        }

        #endregion

        #region --菜单管理--
        /// <summary>
        /// 取得所有类别列表(已经排序好)
        /// </summary>
        /// <param name="parent_id">父ID</param>
        public DataTable GetNavList()
        {
            DataTable dt = GetNavCacheList();
            DataTable oldData = dt as DataTable;
            if (oldData == null)
            {
                return null;
            }
            //创建一个新的DataTable增加一个深度字段
            DataTable newData = oldData.Clone();
            newData.Columns.Add("class_layer", typeof(int));
            //调用迭代组合成DAGATABLE
            GetNavChilds(oldData, newData, 0, 0);
            return newData;
        }
        #region 私有方法================================
        /// <summary>
        /// 从内存中取得所有下级类别列表（自身迭代）
        /// </summary>
        public void GetNavChilds(DataTable oldData, DataTable newData, int parent_id, int class_layer)
        {
            //DataRow[] dr = oldData.Select("parent_id=" + parent_id);
            //for (int i = 0; i < dr.Length; i++)
            //{
            //    //添加一行数据
            //    DataRow row = newData.NewRow();
            //    row["id"] = int.Parse(dr[i]["id"].ToString());
            //    row["nav_type"] = dr[i]["nav_type"].ToString();
            //    row["name"] = dr[i]["name"].ToString();
            //    row["title"] = dr[i]["title"].ToString();
            //    row["sub_title"] = dr[i]["sub_title"].ToString();
            //    row["icon_url"] = dr[i]["icon_url"].ToString();
            //    row["link_url"] = dr[i]["link_url"].ToString();
            //    row["sort_id"] = int.Parse(dr[i]["sort_id"].ToString());
            //    row["is_lock"] = int.Parse(dr[i]["is_lock"].ToString());
            //    row["remark"] = dr[i]["remark"].ToString();
            //    row["parent_id"] = int.Parse(dr[i]["parent_id"].ToString());
            //    row["channel_id"] = int.Parse(dr[i]["channel_id"].ToString());
            //    row["action_type"] = dr[i]["action_type"].ToString();
            //    row["is_sys"] = int.Parse(dr[i]["is_sys"].ToString());
            //    newData.Rows.Add(row);
            //    //调用自身迭代
            //    this.GetNavChilds(oldData, newData, int.Parse(dr[i]["id"].ToString()));
            //}

            class_layer++;
            DataRow[] dr = oldData.Select("parent_id=" + parent_id);
            for (int i = 0; i < dr.Length; i++)
            {
                DataRow row = newData.NewRow();//创建新行
                //循环查找列数量赋值
                for (int j = 0; j < dr[i].Table.Columns.Count; j++)
                {
                    row[dr[i].Table.Columns[j].ColumnName] = dr[i][dr[i].Table.Columns[j].ColumnName];
                }
                row["class_layer"] = class_layer;//赋值深度字段
                newData.Rows.Add(row);//添加新行
                //调用自身迭代
                this.GetNavChilds(oldData, newData, int.Parse(dr[i]["id"].ToString()), class_layer);
            }
        }
        #endregion
        #endregion

        ///// <summary>
        ///// 单点登录功能：进行用户登录验证
        ///// </summary>
        ///// <param name="filterContext">当前过滤器上下文</param>
        ///// <param name="ssoLocal">单点登录验证服务器地址</param>
        //private void SsoTransf(ActionExecutingContext filterContext, string ssoLocal, string cooksso)
        //{
        //    if (cooksso != "")
        //    {
        //        //根据用户名获取用户信息和菜单权限等相关内容
        //        NcManager userInfo = (new libmanager()).getUserInfoByName(cooksso);
        //        if (userInfo != null && userInfo.user_name == cooksso)
        //        {
        //            //存储session，用户基本信息
        //            filterContext.HttpContext.Session["admin_info"] = userInfo;
        //        }
        //        else
        //        {
        //            string msg = string.Format("不明用户,请确认“{0}”用户是否在系统中注册！", cooksso);
        //            filterContext.Result = new RedirectResult("~/ad_min/Login/IllegalUser?msg=" + msg);
        //        }
        //    }
        //}
    }
}
