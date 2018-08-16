using Microsoft.AspNetCore.Mvc;
using NC.Common;
using NC.Core;
using NC.Lib;
using NC.Model.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NC.MVC.Areas.ad_min.Controllers
{
    [Area(AreaNames.Admin)]
    public class HomeController : AdminBase
    {
        public HomeController(NCMVC_DBContext _dblEf) : base(_dblEf)
        {
        }
        [SkipAdminAuthorizeAttribute]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 管理首页
        /// </summary>
        [SkipAdminAuthorizeAttribute]
        public ActionResult Center()
        {
            string strIp = "-";
            string strBackIp = "-";
            string strBackTime = "-";
            //登录信息
            if (siteAdminInfo != null)
            {
                //var model = (from r in NcManagerLog
                //             where r.user_name == admin_info.user_name && r.action_type == "Login"
                //             select r).Take(2).ToList();
                //if (model != null)
                //{
                //    if (model.Count > 0)
                //        strIp = model[0].user_ip;//本次登陆ip
                //    if (model.Count > 1)//上次登录
                //    {
                //        strBackIp = model[1].user_ip;
                //        strBackTime = model[1].add_time.ToString();
                //    }
                //}
            }
            ViewData["strIp"] = strIp;
            ViewData["BackIp"] = strBackIp;
            ViewData["BackTime"] = strBackTime;
            ViewBag.setConf = siteConf;
            return View();
        }



        #region--系统菜单--

        /// <summary>
        /// 获取系统菜单
        /// </summary>

        public JsonResult getSysMenu()
        {
            int strStatus = 0;
            string msg = string.Empty;
            if (!IsAdminLogin())
            {
                msg = "管理员信息异常";
                strStatus = 1;
                return Json(new { status = strStatus, message = msg });
            }
            NcManagerRole roleModel = dblEf.NcManagerRole.Where(mr => mr.Id == siteAdminInfo.RoleId).FirstOrDefault(); //获得管理角色信息
            if (roleModel == null)
            {
                msg = "获取角色异常";
                strStatus = 1;
                return Json(new { status = strStatus, message = msg });
            }
            List<NcManagerRoleValue> roleValueModel = null;
            if (siteAdminInfo.RoleType > 1)//排除超管
            {
                roleValueModel = dblEf.NcManagerRoleValue.Where(mrv => mrv.RoleId == siteAdminInfo.RoleId).ToList();//管理员值
                if (roleValueModel == null)
                {
                    msg = "获取角色异常";
                    strStatus = 1;
                    return Json(new { status = strStatus, message = msg });
                }
            }

            DataTable dt = base.GetNavCacheList("nav_type='" + JHEnums.NavigationEnum.System.ToString() + "'");//此处使用缓存
            StringBuilder strJson = new StringBuilder();
            msg = get_nav_tree(strJson, dt, 0, (int)roleModel.RoleType, roleValueModel);

            return Json(new { status = strStatus, message = msg });
        }
        private string get_nav_tree(StringBuilder strJson, DataTable oldData, int parent_id, int role_type, List<NcManagerRoleValue> ls)
        {
            DataRow[] dr = oldData.Select("parent_id=" + parent_id);
            bool isWrite = false;//是否输出开始标签
            for (int i = 0; i < dr.Length; i++)
            {
                //检查是否显示在界面上====================
                bool isActionPass = true;
                if (int.Parse(dr[i]["is_lock"].ToString()) == 1)
                {
                    isActionPass = false;
                }
                //检查管理员权限==========================
                if (isActionPass && role_type > 1)
                {
                    string[] actionTypeArr = dr[i]["action_type"].ToString().Split(',');
                    foreach (string action_type_str in actionTypeArr)
                    {
                        //如果存在显示权限资源，则检查是否拥有该权限
                        //想要显示菜单，权限里必须选中“显示Show”
                        if (action_type_str == "Show")
                        {
                            NcManagerRoleValue modelt = ls.Find(p => p.NavName == dr[i]["name"].ToString() && p.ActionType == "Show");
                            if (modelt == null)
                            {
                                isActionPass = false;
                            }
                        }
                    }
                }


                //如果没有该权限则不显示
                if (!isActionPass)
                {
                    if (i == (dr.Length - 1) && parent_id != 0)
                    {
                        strJson.Append("</ul>\n");
                    }
                    continue;//循环下一条
                }
                #region--拼接开始--
                string strIClass = string.Empty;
                //如果是顶级导航
                if (parent_id == 0)
                {
                    strJson.Append("<div class=\"side-menu-group active\">\n");
                    string iconStyle = "fa-gears";
                    if (!string.IsNullOrEmpty(dr[i]["icon_url"].ToString().Trim()))
                    {
                        iconStyle = dr[i]["icon_url"].ToString().Trim('.');//只用css
                    }
                    strJson.Append("<h1 title=\"" + dr[i]["sub_title"].ToString() + "\" main-nav-icon=\"fa "+ iconStyle + "\">");

                    strJson.Append("<i class=\"fa " + iconStyle + "\"></i></h1>\n");
                    strJson.Append("<div class=\"side-menu-wrap\">\n");
                    strJson.Append("<h2><i class=\"fa " + iconStyle + "\"></i><span>" + dr[i]["title"].ToString() + "</span><b class=\"fa fa-angle-double-down\"></b></h2>\n");
                    //调用自身迭代
                    this.get_nav_tree(strJson, oldData, int.Parse(dr[i]["id"].ToString()), role_type, ls);
                    strJson.Append("    </div>\n");
                    strJson.Append("</div>\n");
                }
                else//下级导航
                {
                    if (!isWrite)
                    {
                        isWrite = true;
                        strJson.Append("<ul class=\"nav\">\n");
                    }
                    strJson.Append("<li>\n");
                    strJson.Append("<a");
                    if (!string.IsNullOrEmpty(dr[i]["link_url"].ToString()))
                    {
                        if (int.Parse(dr[i]["channel_id"].ToString()) > 0)
                        {
                            strJson.Append(" class=\"J_menuItem\" href=\"" + dr[i]["link_url"].ToString() + "?channel_id=" + dr[i]["channel_id"].ToString() + "\"");
                        }
                        else
                        {
                            strJson.Append(" class=\"J_menuItem\" href=\"" + dr[i]["link_url"].ToString() + "\"");
                        }
                    }
                    strJson.Append(">");
                    string iconStyle = "fa-bars";
                    if (!string.IsNullOrEmpty(dr[i]["icon_url"].ToString().Trim()))
                    {
                        iconStyle = dr[i]["icon_url"].ToString().TrimStart('.');
                    }
                    strJson.Append("<i class=\"fa " + iconStyle + "\"></i>" + dr[i]["title"].ToString() + "</a>\n");
                    //调用自身迭代
                    this.get_nav_tree(strJson, oldData, int.Parse(dr[i]["id"].ToString()), role_type, ls);
                    strJson.Append("</li>\n");

                    if (i == (dr.Length - 1))
                    {
                        strJson.Append("</ul>\n");
                    }
                }
                #endregion

            }
            return strJson.ToString();
        }
        #endregion
    }
}