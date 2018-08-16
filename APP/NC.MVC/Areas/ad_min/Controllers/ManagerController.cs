using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

using Microsoft.AspNetCore.Mvc;
using NC.Common;
using NC.Core;
using NC.Lib;
using NC.Model;
using NC.Model.Models;

namespace NC.MVC.Areas.ad_min.Controllers
{
    /// <summary>
    /// 管理
    /// </summary>
    [Area(AreaNames.Admin)]
    public class ManagerController : AdminBase
    {

        public ManagerController(NCMVC_DBContext _dblEf) : base(_dblEf)
        {
        }
        public ManagerController()
        {
        }

        private string action = JHEnums.ActionEnum.Add.ToString(); //操作类型

        protected string strStatus = string.Empty;
        protected string strMsg = string.Empty;

        #region--菜单处理--

        /// <summary>
        /// 菜单列表
        /// </summary>
        [NavAttr(NavName = "sys_nav")]
        public ActionResult Nav_List()
        {
            //获得列表
            ViewBag.AllNav = "";
            return View();
        }
        /// <summary>
        /// 菜单列表-编辑
        /// </summary>
        [NavAttr(NavName = "sys_nav")]
        public ActionResult Nav_Edit()
        {
            action = Request.Query["act"];
            int id = Utils.StrToInt(Request.Query["id"]);
            NcNavigation model = new NcNavigation();
            if (action.ToLower() == JHEnums.ActionEnum.Edit.ToString().ToLower() && id != 0) //修改
            {
                model = BindNav(id);
            }
            else
            {
                model.SortId = 99;
                if (id != 0)
                {
                    model.ParentId = id;
                }
            }
            ViewBag.action = action;
            ViewBag.id = id;
            return View(model);
        }
        /// <summary>
        /// 绑定菜单
        /// </summary>
        private NcNavigation BindNav(int id)
        {
            NcNavigation model = dblEf.Find<NcNavigation>(id);
            return model;
        }
        /// <summary>
        /// 验证菜单是否包含
        /// </summary>
        [NavAttr(NavName = "sys_nav")]
        public JsonResult nav_validate()
        {
            string navname = Request.Form["param"].ToString();
            string old_name = Request.Query["old_name"].ToString();

            string strStatus = "y";
            string strMsg = "该导航菜单ID可使用！";
            if (string.IsNullOrEmpty(navname))
            {
                strStatus = "n";
                strMsg = "该导航菜单ID不可为空！";
            }
            else if (navname.ToLower() == old_name.ToLower())//未改
            {
                //strMsg = "该导航菜单ID可使用！";
            }
            //检查保留的名称开头
            else if (navname.ToLower().StartsWith("channel_"))
            {
                strStatus = "n";
                strMsg = "该导航菜单ID系统保留，请更换！";
            }
            else
            {
                NcNavigation model = dblEf.NcNavigation.Where(n => n.Name == navname).FirstOrDefault();
                if (model != null)
                {
                    strStatus = "n";
                    strMsg = "该导航菜单ID已被占用，请更换！";
                }
            }
            return Json(new { status = strStatus, info = strMsg });
        }

        /// <summary>
        /// 保存导航菜单
        /// </summary>
        /// <param name="model"></param>
        [NavAttr(NavName = "sys_nav")]
        public JsonResult NavSave()
        {
            NcNavigation model = new NcNavigation();
            action = Request.Form["act"];
            if (action.ToLower() == JHEnums.ActionEnum.Edit.ToString().ToLower())//修改
            {
                int id = Utils.StrToInt(Request.Form["id"]);
                model = dblEf.Find<NcNavigation>(id);
                model = NavSetModel(model);

                if (NavUpdate(model))
                {
                    CacheHelper.Remove("NC_nav");
                    strStatus = "1";
                    strMsg = "保存成功";
                }
            }
            else//新增
            {
                model = NavSetModel(model);

                model.ChannelId = 0;
                model.IsSys = 0;
                if (NavAdd(model) > 0)
                {
                    CacheHelper.Remove("NC_nav");
                    strStatus = "1";
                    strMsg = "保存成功";
                }
            }
            return Json(new { status = strStatus, msg = strMsg });
        }

        private NcNavigation NavSetModel(NcNavigation model)
        {
            model.NavType = JHEnums.NavigationEnum.System.ToString();
            model.Name = Request.Form["txtName"];
            model.Title = Request.Form["txtTitle"];
            model.SubTitle = Request.Form["txtSubTitle"];
            model.LinkUrl = Request.Form["txtLinkUrl"];
            model.SortId = Utils.StrToInt(Request.Form["SortId"]);
            model.IsLock = 0;
            if (Request.Form["is_lock"] == "on")
            {
                model.IsLock = 1;
            }
            model.Remark = Request.Form["remark"];
            int parentId = Utils.StrToInt(Request.Form["parent_id"]);
            //如果选择的父ID不是自己,则更改
            if (parentId != model.Id || model.Id == 0)
            {
                model.ParentId = parentId;
            }

            //添加操作权限类型
            model.ActionType = Request.Form["cblActionType"];

            return model;
        }
        /// <summary>
        /// 增加一条数据
        /// </summary>
        [HttpPost]
        public int NavAdd(NcNavigation model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into nc_Navigation(");
            strSql.Append("nav_type,name,title,sub_title,link_url,Sort_Id,is_lock,remark,Parent_Id,channel_id,action_type,is_sys)");
            strSql.Append(" values (");
            strSql.Append("@nav_type,@name,@title,@sub_title,@link_url,@SortId,@is_lock,@remark,@ParentId,@channel_id,@action_type,@is_sys)");
            strSql.Append(";select @ReturnValue= @@IDENTITY ");
            strSql.Append(@"if @@error<>0 begin rollback tran
                                    select N'fail' as result
                                    return
                                end");
            string sql = @"                
                            Begin Tran
                                declare @ReturnValue as int
                                " + strSql.ToString() + @"
                                select N'success' as result
                            Commit Tran
                        ";

            DbParameters p = new DbParameters();
            p.Add("@ParentId", model.ParentId);
            p.Add("@channel_id", model.ChannelId);
            p.Add("@action_type", model.ActionType);
            p.Add("@is_sys", model.IsSys);
            p.Add("@nav_type", model.NavType);
            p.Add("@name", model.Name);
            p.Add("@title", model.Title);
            p.Add("@sub_title", model.SubTitle);
            p.Add("@link_url", model.LinkUrl);
            p.Add("@SortId", model.SortId);
            p.Add("@is_lock", model.IsLock);
            p.Add("@remark", model.Remark);
            string result = Utils.ObjectToStr(Dbl.ZZCMS.CreateSqlScalar(sql, p));
            return result == "success" ? 1 : 0;
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>

        [HttpPost]
        public bool NavUpdate(NcNavigation model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update nc_Navigation set ");
            strSql.Append("nav_type=@nav_type,");
            strSql.Append("name=@name,");
            strSql.Append("title=@title,");
            strSql.Append("sub_title=@sub_title,");
            strSql.Append("link_url=@link_url,");
            strSql.Append("Sort_Id=@SortId,");
            strSql.Append("is_lock=@is_lock,");
            strSql.Append("remark=@remark,");
            strSql.Append("Parent_Id=@ParentId,");
            strSql.Append("channel_id=@channel_id,");
            strSql.Append("action_type=@action_type,");
            strSql.Append("is_sys=@is_sys");
            strSql.Append(" where id=@id;");
            strSql.Append(@"if @@error<>0 begin rollback tran
                                    select N'fail' as result
                                    return
                                end");
            //
            StringBuilder strChild = new StringBuilder();
            StringBuilder strN = new StringBuilder();
            //string sC = UpdateChilds(strChild, model.ParentId);
            //string sn = UpdateChilds(strN, model.id);
            //先判断选中的父节点是否被包含
            //查找旧父节点数据
            //" + sC + @"
            //" + sn + @"
            string sql = @"                
                            Begin Tran
                                declare @ReturnValue as int
                                " + strSql.ToString() + @"
                                select N'success' as result
                            Commit Tran
                        ";

            DbParameters p = new DbParameters();
            p.Add("@id", model.Id);
            p.Add("@ParentId", model.ParentId);
            p.Add("@channel_id", model.ChannelId);
            p.Add("@action_type", model.ActionType);
            p.Add("@is_sys", model.IsSys);
            p.Add("@nav_type", model.NavType);
            p.Add("@name", model.Name);
            p.Add("@title", model.Title);
            p.Add("@sub_title", model.SubTitle);
            p.Add("@link_url", model.LinkUrl);
            p.Add("@SortId", model.SortId);
            p.Add("@is_lock", model.IsLock);
            p.Add("@remark", model.Remark);

            string result = Utils.ObjectToStr(Dbl.ZZCMS.CreateSqlScalar(sql, p));
            return result == "success";
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        [NavAttr(NavName = "sys_nav")]
        public ActionResult DelNav(string ids)
        {
            bool res = true;
            if (string.IsNullOrEmpty(ids))
            {
                res = false;
                strMsg = "删除参数异常！";//
            }
            else
            {
                string[] arrId = ids.Split(',');
                try
                {
                    for (int i = 0; i < arrId.Length; i++)
                    {
                        string id = arrId[i];
                        //dblEf.NcNavigation.Remove(nv => nv.Id.Contains("," + id + ","));//

                        //1、创建要删除的对象  
                        NcNavigation modelDel = new NcNavigation() { Id = Utils.StrToInt(id) };
                        //2、将对象添加到EF管理容器中  
                        dblEf.NcNavigation.Attach(modelDel);
                        //3、将对象包装类的状态标识为删除状态  
                        dblEf.NcNavigation.Remove(modelDel);
                        //4、更新到数据库  
                        dblEf.SaveChanges();
                    }
                    res = true;
                    strMsg = "删除成功！";

                }
                catch (Exception ex)//循环删除，异常才报删除错误
                {
                    res = false;
                    strMsg = "删除过程中出现异常！";//调试过程中+ex.ToString();
                }
            }
            return Json(new { status = (res ? 1 : 0), message = strMsg });
        }
        /// <summary>
        /// 更新菜单排序
        /// </summary>
        [NavAttr(NavName = "sys_nav")]
        public JsonResult UpdateSort_Nav(string ids, string sorts)
        {
            bool res = true;
            if (string.IsNullOrEmpty(ids) || string.IsNullOrEmpty(sorts))
            {
                res = false;
            }
            else
            {
                string[] arrId = ids.Split(',');
                string[] arrSort = sorts.Split(',');
                for (int i = 0; i < arrId.Length; i++)
                {
                    NcNavigation model = dblEf.Find<NcNavigation>(Utils.StrToInt(arrId[i]));
                    if (model != null && Utils.StrToInt(arrSort[i]) != model.SortId)//减少数据库访问次数
                    {
                        model.SortId = Utils.StrToInt(arrSort[i]);
                        res = dblEf.SaveChanges() > 0;
                        //res = dblEf.Update<NcNavigation>(model);
                    }
                }
            }
            return Json(new { status = (res ? 1 : 0), message = "保存成功！" });
        }
        #endregion

        #region--部门方法--
        /// <summary>
        /// 部门-用户管理
        /// </summary>
        /// <returns></returns>
        [NavAttr(NavName = "sys_deptuser")]
        public ActionResult Dept_Manager()
        {
            string strSql = "SELECT Dept_ID AS id,Parent_ID AS pId,Dept_Name AS name FROM nc_manager_dept";
            DataTable dt = Dbl.ZZCMS.CreateSqlDataTable(strSql);
            ViewBag.initJson = JsonHelper.DataTableToJSON(dt);
            return View();
        }
        /// <summary>
        /// 保存导航菜单
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        [NavAttr(NavName = "sys_deptuser")]
        public JsonResult Dept_Save(long id, long pId, string name)
        {
            int add = 0;
            NcManagerDept model = new NcManagerDept();
            if (id > 0)//修改
            {
                model = dblEf.Find<NcManagerDept>(id);
                model.DeptName = name;

                if (Dept_Update(model))
                {
                    strStatus = "1";
                    strMsg = "保存成功";
                }
                add = 1;
            }
            else//新增
            {
                model.ParentId = pId;
                model.DeptName = name;

                model.ClassList = "";
                model.ClassLayer = 1;
                model.SortId = 99;
                model.DeptDesc = "";
                model.Ostatus = 0;
                if (Dept_Add(model) > 0)
                {
                    strStatus = "1";
                    strMsg = "保存成功";
                }
            }
            return Json(new { status = strStatus, msg = strMsg, add = add });
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        [NavAttr(NavName = "sys_deptuser")]
        public int Dept_Add(Model.Models.NcManagerDept model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into nc_manager_dept(");
            strSql.Append("Parent_ID,Class_List,Class_Layer,Sort_ID,Dept_Name,Dept_Desc,Created_ID,OStatus)");
            strSql.Append(" values (");
            strSql.Append("@Parent_ID,@Class_List,@Class_Layer,@Sort_ID,@Dept_Name,@Dept_Desc,@Created_ID,@OStatus)");
            strSql.Append(";select @ReturnValue= @@IDENTITY ");
            strSql.Append(@"if @@error<>0 begin rollback tran
                                    select N'fail' as result
                                    return
                                end");
            string sql = @"                
                            Begin Tran
                                declare @ReturnValue as int,@pClass_list as nvarchar(500),@pClass_layer as int
                                " + strSql.ToString() + @"
                                if @parent_id>0 begin
                                    select @pClass_list=class_list,@pClass_layer=class_layer from nc_manager_dept where dept_id=@parent_id
                                    set @pClass_list=@pClass_list+cast(@ReturnValue as nvarchar(max))+','
                                    set @pClass_layer=@pClass_layer+1
                                end 
                                else begin 
                                    set @pClass_list=','+cast(@ReturnValue as nvarchar(max))+','
                                    set @pClass_layer=1
                                end
                                update nc_manager_dept set class_list=@pClass_list, class_layer=@pClass_layer where dept_id=@ReturnValue
                                if @@error<>0 begin rollback tran
                                    select N'fail' as result
                                    return
                                end
                                select N'success' as result
                                --select N'parent_id_error'+cast(@pClass_layer as nvarchar(max))+cast(@ReturnValue as nvarchar(max)) as result
                            Commit Tran
                        ";

            DbParameters p = new DbParameters();
            p.Add("@parent_id", model.ParentId);
            p.Add("@class_list", model.ClassList);
            p.Add("@class_layer", model.ClassLayer);
            p.Add("@Dept_Name", model.DeptName);
            p.Add("@Dept_Desc", model.DeptDesc);
            p.Add("@sort_id", model.SortId);
            p.Add("@Created_ID", siteAdminInfo.Id);
            p.Add("@OStatus", model.Ostatus);
            string result = Utils.ObjectToStr(Dbl.ZZCMS.CreateSqlScalar(sql, p));
            return result == "success" ? 1 : 0;
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        [NavAttr(NavName = "sys_deptuser")]
        public bool Dept_Update(Model.Models.NcManagerDept model)
        {
            dblEf.Update<NcManagerDept>(model);
            return dblEf.SaveChanges() > 0;
        }
        /// <summary>
        /// 删除菜单
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_deptuser")]
        public ActionResult Dept_Del(long id)
        {
            bool res = true;
            if (id <= 0)
            {
                res = false;
                strMsg = "删除参数异常！";//
            }
            else
            {
                try
                {
                    ////dblEf.Delete<Model.manager_dept>(md => md.Dept_ID == id);//删除,linq to entities不识别xx[index]这种格式，需要先赋值临时变量
                    //res = true;
                    //strMsg = "删除成功！";

                    NcManagerDept model = dblEf.Find<NcManagerDept>(id);
                    if (model != null)
                    {
                        dblEf.Remove<NcManagerDept>(model);
                        res = dblEf.SaveChanges() > 0;
                        strMsg = "删除成功！";
                    }

                }
                catch (Exception ex)//循环删除，异常才报删除错误
                {
                    res = false;
                    strMsg = "删除过程中出现异常！";//调试过程中+ex.ToString();
                }
            }
            return Json(new { status = (res ? 1 : 0), msg = strMsg });
        }
        #endregion

        #region--用户管理--
        /// <summary>
        /// 用户管理
        /// </summary>
        [NavAttr(NavName = "sys_deptuser")]
        public ActionResult Managers()
        {
            int dept_id = Utils.StrToInt(Request.Query["dept_id"]);//部门id
            int Rid = Utils.StrToInt(Request.Query["rid"]);//角色id
            string keyword = Request.Query["keyword"];//搜索关键字

            string strWhere = string.Empty;
            strWhere += " and id>1 ";
            if (Rid > 0)
            {
                strWhere += " and role_id='" + Rid + "'";
            }
            if (dept_id > 1)
            {
                strWhere += " and dept_id='" + dept_id + "'";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                strWhere += " and (user_name like  '%" + keyword + "%' or real_name like '%" + keyword + "%' or email like '%" + keyword + "%')";
            }
            string strSql = "Select Count(*) From Nc_Manager where role_type>=1 " + strWhere;
            ViewBag.TotalCount = Dbl.ZZCMS.CreateSqlScalar(strSql);
            ViewBag.rid = Rid;
            ViewBag.dept_id = dept_id;
            ViewBag.keyword = keyword;
            return View();
        }
        /// <summary>
        /// 管理员列表
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_deptuser")]
        public string Manager_List()
        {
            int pageSize = Utils.StrToInt(Request.Query["page_size"]);
            int pageIndex = Utils.StrToInt(Request.Query["page_index"], 0) + 1;

            int dept_id = Utils.StrToInt(Request.Query["dept_id"]);//部门id
            int Rid = Utils.StrToInt(Request.Query["rid"]);//角色id
            string keyword = Request.Query["keyword"];//搜索关键字

            string strWhere = Request.Query["sw"].ToString() ?? string.Empty;
            strWhere += " and m.id>1 ";

            if (Rid > 0)
            {
                strWhere += " and mr.id='" + Rid + "'";
            }
            if (dept_id > 1)
            {
                strWhere += " and m.dept_id='" + dept_id + "'";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                strWhere += " and (user_name like  '%" + keyword + "%' or real_name like '%" + keyword + "%' or email like '%" + keyword + "%')";
            }
            string filedOrder = " m.id desc";
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT m.*,mr.role_name FROM Nc_Manager m
                            LEFT JOIN nc_manager_role mr ON m.role_id=mr.id
                        ");
            if (strWhere != "")
            {
                strSql.Append(" where 1=1 " + strWhere);
            }

            int recordCount = Convert.ToInt32(Dbl.ZZCMS.CreateSqlScalar(PagingHelper.CreateCountingSql(strSql.ToString())));
            DataTable dt = Dbl.ZZCMS.CreateSqlDataTable(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            var result = JsonHelper.DataTableToJSON(dt);
            return result;
        }
        /// <summary>
        /// 用户管理-编辑
        /// </summary>
        [NavAttr(NavName = "sys_deptuser")]
        public ActionResult Manager_Edit()
        {
            action = Request.Query["act"];
            int id = Utils.StrToInt(Request.Query["id"]);
            int dept_id = Utils.StrToInt(Request.Query["dept_id"]);
            NcManager model = new NcManager();
            string defaultpassword = "0|0|0|0"; //默认显示密码 
            if (action.ToLower() == JHEnums.ActionEnum.Edit.ToString().ToLower() && id != 0) //修改
            {
                model = dblEf.Find<NcManager>(id);
                model.Password = defaultpassword;
            }
            else
            {
                model.Password = defaultpassword;
            }
            NcManager adminModel = siteAdminInfo; //获得当前登录管理员信息
            ViewBag.action = action;
            ViewBag.id = id;
            ViewBag.dept_id = dept_id;
            ViewBag.AdminModel = adminModel;
            return View(model);
        }

        #region --用户增改删--
        /// <summary>
        /// 验证用户名是否包含
        /// </summary>
        [NavAttr(NavName = "sys_deptuser")]
        public JsonResult manager_validate()
        {
            string user_name = Request.Form["param"];

            string strStatus = "y";
            string strMsg = "用户名可使用！";
            if (string.IsNullOrEmpty(user_name))
            {
                strStatus = "n";
                strMsg = "请输入用户名！";
            }

            NcManager model = dblEf.NcManager.Where(n => n.UserName == user_name).FirstOrDefault();
            if (model != null)
            {
                strStatus = "n";
                strMsg = "用户名已被占用，请更换！";
            }
            return Json(new { status = strStatus, info = strMsg });
        }
        /// <summary>
        /// 保存用户
        /// </summary>
        /// <param name="model"></param>
        [NavAttr(NavName = "sys_deptuser")]
        public JsonResult ManagerSave()
        {
            NcManager model = new NcManager();
            action = Request.Form["act"];
            if (action.ToLower() == JHEnums.ActionEnum.Edit.ToString().ToLower())//修改
            {
                int id = Utils.StrToInt(Request.Form["id"]);
                model = dblEf.Find<NcManager>(id);
                model = ManagerSetModel(model);

                if (ManagerUpdate(model))
                {
                    strStatus = "1";
                    strMsg = "保存成功";
                }
            }
            else//新增
            {
                model = ManagerSetModel(model);
                model.AddTime = DateTime.Now;
                if (ManagerAdd(model))
                {
                    strStatus = "1";
                    strMsg = "保存成功";
                }
            }
            return Json(new { status = strStatus, msg = strMsg });
        }
        /// <summary>
        /// 角色获取赋值
        /// </summary>
        private NcManager ManagerSetModel(NcManager model)
        {
            model.DeptId = Utils.StrToInt(Request.Form["dept_id"]);
            model.RoleId = Utils.StrToInt(Request.Form["role_id"]);
            model.RoleType = 2;
            model.IsLock = 0;

            model.UserName = Request.Form["username"];
            string pwd = Request.Form["password"];
            //判断密码是否更改
            if (Request.Form["password"] != "0|0|0|0")
            {
                //获得6位的salt加密字符串
                model.Salt = Utils.GetCheckCode(6);
                //以随机生成的6位字符串做为密钥加密
                //获取用户已生成的salt作为密钥加密
                model.Password = DESEncrypt.Encrypt(Request.Form["password"], model.Salt);
            }
            model.RealName = Request.Form["realname"];
            model.Telephone = Request.Form["Telephone"];

            return model;
        }
        /// <summary>
        /// 增加一条数据
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_deptuser")]
        public bool ManagerAdd(NcManager model)
        {
            dblEf.Add<NcManager>(model);
            return dblEf.SaveChanges() > 0;
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_deptuser")]
        public bool ManagerUpdate(NcManager model)
        {
            //return dblEf.Update<NcManager>(model);
            dblEf.Update<NcManager>(model);
            return dblEf.SaveChanges() > 0;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_deptuser")]
        public JsonResult Manager_Del(string ids)
        {
            bool res = true;
            if (string.IsNullOrEmpty(ids))
            {
                res = false;
                strMsg = "删除参数异常！";//
            }
            else
            {
                string[] arrId = ids.Split(',');
                string result = string.Empty;
                try
                {
                    for (int i = 0; i < arrId.Length; i++)
                    {
                        int id = Utils.StrToInt(arrId[i]);
                        NcManager model = dblEf.Find<NcManager>(id);
                        if (model != null)
                        {
                            //dblEf.Delete<NcManager>(m => m.id == id);//删除,linq to entities不识别xx[index]这种格式，需要先赋值临时变量
                            dblEf.Remove<NcManager>(model);
                            res = dblEf.SaveChanges() > 0;
                            strMsg = "删除成功！";
                        }
                    }
                }
                catch (Exception ex)//循环删除，异常才报删除错误
                {
                    res = false;
                    strMsg = "删除过程中出现异常！";//调试过程中+ex.ToString();
                }
            }
            return Json(new { status = (res ? 1 : 0), message = strMsg });
        }
        #endregion

        #endregion

        #region --角色管理--

        /// <summary>
        /// 角色管理
        /// </summary>
        [NavAttr(NavName = "sys_role")]
        public ActionResult Role_List()
        {
            string keywords = string.Empty;
            string strWhere = " and role_type>1";
            keywords = Request.Query["keywords"];

            if (!string.IsNullOrEmpty(keywords))
            {
                strWhere += " and role_name like '%" + keywords + "%'";
            }
            ViewBag.RoleList = BindRole(strWhere);
            return View();
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        [NavAttr(NavName = "sys_role")]
        public DataTable BindRole(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select id,role_name,role_type,is_sys ");
            strSql.Append(" FROM nc_manager_role ");
            if (strWhere != "")
            {
                strSql.Append(" where 1=1 " + strWhere);
            }
            return Dbl.ZZCMS.CreateSqlDataTable(strSql.ToString());
        }

        /// <summary>
        /// 角色-编辑
        /// </summary>
        [NavAttr(NavName = "sys_role")]
        public ActionResult Role_Edit()
        {
            action = Request.Query["act"];
            int id = Utils.StrToInt(Request.Query["id"]);
            CacheHelper.Remove("permisson" + id);//角色编辑清空缓存
            NcManagerRole model = new NcManagerRole();
            List<NcManagerRoleValue> modelRoleValue = new List<NcManagerRoleValue>();
            if (action.ToLower() == JHEnums.ActionEnum.Edit.ToString().ToLower() && id != 0) //修改
            {
                model = BindRoleById(id);
                modelRoleValue = dblEf.NcManagerRoleValue.Where(mrv => mrv.RoleId == id).ToList();
            }

            NcManager adminModel = siteAdminInfo; //获得当前登录管理员信息

            ViewBag.action = action;
            ViewBag.id = id;
            ViewBag.DtRoleNav = RoleNavBind((int)adminModel.RoleType, (int)adminModel.RoleId);
            ViewBag.AdminModel = adminModel;
            ViewBag.ListRoleValue = modelRoleValue;
            return View(model);
        }
        /// <summary>
        /// 根据角色id获取角色model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private NcManagerRole BindRoleById(int id)
        {
            NcManagerRole model = dblEf.Find<NcManagerRole>(id);
            return model;
        }
        /// <summary>
        /// 角色管理-管理权限
        /// </summary>
        private DataTable RoleNavBind(int roleType, int roleId)
        {
            if (roleType == 1)//超管加载全部菜单权限
            {
                DataTable dt = GetNavCacheList("nav_type='" + JHEnums.NavigationEnum.System.ToString() + "'");
                //DataTable oldData = dt as DataTable;
                //if (oldData == null)
                //{
                //    return null;
                //}
                ////复制结构
                //DataTable newData = oldData.Clone();
                ////调用迭代组合成DAGATABLE
                //new libmanager().GetNavChilds(oldData, newData, 0);
                //return newData;
                return dt;

            }
            else//加载本地区的权限菜单,只有地区管理员有权限
            {
                DataTable dtMR = GetNavByRoleId(roleId);
                return dtMR;
            }
        }
        /// <summary>
        /// 根据角色ID,返回要展示的数据
        /// </summary>
        public DataTable GetNavByRoleId(int id)
        {
            string sql = @"select * from nc_Navigation dn left join (select dmr.id,dmrv.nav_name from nc_manager_role_value dmrv left join nc_manager_role dmr on dmrv.role_id=dmr.id where dmr.id=@id group by dmr.id,dmrv.nav_name) rt on dn.name=rt.nav_name where rt.id=@id order by dn.Sort_Id asc,dn.id asc";
            DbParameters p = new DbParameters();
            p.Add("@id", id);
            DataTable dt = Dbl.ZZCMS.CreateSqlDataTable(sql, p);
            DataTable oldData = dt as DataTable;
            if (oldData == null)
            {
                return null;
            }
            //复制结构
            DataTable newData = oldData.Clone();
            newData.Columns.Add("class_layer", typeof(int));
            //调用迭代组合成DAGATABLE
            GetNavChilds(oldData, newData, 0, 0);
            return newData;
        }

        /// <summary>
        /// 不同角色按钮权限显示不同
        /// </summary>
        public string GetActionTypeByNavName(int roleType, int roleId, string navName, string action_type)
        {
            if (roleType != 1)//超管加载全部菜单权限
            {
                return GetActionTypeByNavName(roleId, navName);
            }
            return action_type;
        }

        /// <summary>
        /// 获取展示权限，根据角色ID，navName
        /// </summary>
        public string GetActionTypeByNavName(int roleID, string navName)
        {
            string sql = @"
                    Begin Tran
                        declare @s as varchar(max)
                        select @s=isnull(@s +',','')+dmrv.action_type from  nc_manager_role_value dmrv left join nc_manager_role dmr on dmrv.role_id=dmr.id where dmr.id=@ID and dmrv.nav_name=@navName
                        if @@error<>0 begin rollback tran
                            select N'fail' as result
                            return
                        end
                        select @s as result
                    Commit Tran        
";
            DbParameters p = new DbParameters();
            p.Add("@ID", roleID);
            p.Add("@navName", navName);
            string result = Utils.ObjectToStr(Dbl.ZZCMS.CreateSqlScalar(sql, p));
            return result;
        }

        #region --增改删--
        /// <summary>
        /// 保存角色用户
        /// </summary>
        /// <param name="model"></param>
        [NavAttr(NavName = "sys_role")]
        public JsonResult RoleSave()
        {
            NcManagerRole model = new NcManagerRole();
            List<NcManagerRoleValue> modelRoleValue = new List<NcManagerRoleValue>();

            action = Request.Form["act"];
            if (action.ToLower() == JHEnums.ActionEnum.Edit.ToString().ToLower())//修改
            {
                int id = Utils.StrToInt(Request.Form["id"]);
                model = dblEf.Find<NcManagerRole>(id);

                model = RoleSetModel(model, out modelRoleValue);

                if (RoleUpdate(model, modelRoleValue))
                {
                    strStatus = "1";
                    strMsg = "保存成功";
                }
            }
            else//新增
            {
                model = RoleSetModel(model, out modelRoleValue);

                if (RoleAdd(model, modelRoleValue) > 0)
                {
                    strStatus = "1";
                    strMsg = "保存成功";
                }
            }
            return Json(new { status = strStatus, msg = strMsg });
        }
        /// <summary>
        /// 角色获取赋值
        /// </summary>
        private NcManagerRole RoleSetModel(NcManagerRole model, out List<NcManagerRoleValue> modelRoleValue)
        {
            model.RoleType = 2;
            model.RoleName = Request.Form["RoleName"];
            model.IsSys = 0;
            modelRoleValue = new List<NcManagerRoleValue>();
            string[] nav_name = Request.Form["hidName"].ToString().Split(',');
            for (int i = 0; i < nav_name.Length; i++)
            {
                string action_type = Request.Form[nav_name[i]];
                if (!string.IsNullOrEmpty(action_type))
                {
                    for (int j = 0; j < action_type.Split(',').Length; j++)
                    {
                        string s = action_type.Split(',')[j];
                        modelRoleValue.Add(new NcManagerRoleValue { RoleId = model.Id, NavName = nav_name[i], ActionType = s });
                    }
                }
            }
            return model;
        }
        /// <summary>
        /// 增加一条数据
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_role")]
        public int RoleAdd(NcManagerRole model, List<NcManagerRoleValue> modelRoleValue)
        {
            StringBuilder strSql2 = new StringBuilder();
            if (modelRoleValue != null)
            {
                foreach (NcManagerRoleValue modelt in modelRoleValue)
                {
                    strSql2.Append("insert into nc_manager_role_value(");
                    strSql2.Append("role_id,nav_name,action_type)");
                    strSql2.Append(" values (");
                    strSql2.Append("@ReturnValue,'" + modelt.NavName + "','" + modelt.ActionType + "');");
                }
                strSql2.Append(@"if @@error<>0 begin rollback tran
                                    select N'fail' as result
                                    return
                                end"
                              );
            }
            //1.添加角色
            //2.添加对应的权限
            string sql = @"                
                            Begin Tran
                                declare @ReturnValue as int
                                insert into nc_manager_role(role_name,role_type,is_sys) values(@role_name,@role_type,@is_sys);set @ReturnValue= @@IDENTITY
                                if @@error<>0 begin rollback tran
                                    select N'fail' as result
                                    return
                                end
                                " + strSql2.ToString() + @"
                                select N'success' as result
                            Commit Tran
                        ";

            DbParameters p = new DbParameters();
            p.Add("@role_name", model.RoleName);
            p.Add("@role_type", model.RoleType);
            p.Add("@is_sys", model.IsSys);

            string result = Utils.ObjectToStr(Dbl.ZZCMS.CreateSqlScalar(sql, p));

            return result == "success" ? 1 : 0;
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_role")]
        public bool RoleUpdate(NcManagerRole model, List<NcManagerRoleValue> modelRoleValue)
        {
            StringBuilder strSql3 = new StringBuilder();
            //添加权限
            if (modelRoleValue != null)
            {
                foreach (NcManagerRoleValue modelt in modelRoleValue)
                {
                    strSql3.Append("insert into nc_manager_role_value(");
                    strSql3.Append("role_id,nav_name,action_type)");
                    strSql3.Append(" values (");
                    strSql3.Append("" + model.Id + ",'" + modelt.NavName + "','" + modelt.ActionType + "');");
                }
                strSql3.Append(@"if @@error<>0 begin rollback tran
                                    select N'fail' as result
                                    return
                                end"
                              );
            }
            //1.先删除该角色所有权限
            //2.添加权限
            string sql = @"                
                            Begin Tran
                                update nc_manager_role set role_name=@role_name,role_type=@role_type,is_sys=@is_sys where id=@id
                                if @@error<>0 begin rollback tran
                                    select N'fail' as result
                                    return
                                end
                                delete from nc_manager_role_value where role_id=@role_id
                                if @@error<>0 begin rollback tran
                                    select N'fail' as result
                                    return
                                end
                                " + strSql3.ToString() + @"
                                select N'success' as result
                            Commit Tran
                        ";

            DbParameters p = new DbParameters();
            p.Add("@role_name", model.RoleName);
            p.Add("@role_type", model.RoleType);
            p.Add("@is_sys", model.IsSys);
            p.Add("@id", model.Id);
            p.Add("@role_id", model.Id);

            string result = Utils.ObjectToStr(Dbl.ZZCMS.CreateSqlScalar(sql, p));
            return result == "success";
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        [NavAttr(NavName = "sys_role")]
        public ActionResult RoleDel(string ids)
        {
            bool res = true;
            if (string.IsNullOrEmpty(ids))
            {
                res = false;
                strMsg = "删除参数异常！";//
            }
            else
            {
                string[] arrId = ids.Split(',');
                string result = string.Empty;
                try
                {
                    for (int i = 0; i < arrId.Length; i++)
                    {
                        string id = arrId[i];
                        //删除管理角色权限
                        string sql = @"                
                            Begin Tran
                                --declare @role_id as int
                                --declare @id as int
                                delete from nc_manager_role_value  where role_id=@role_id
                                if @@error<>0 begin rollback tran
                                    select N'fail' as result
                                    return
                                end
                                delete from nc_manager_role where id=@role_id
                                if @@error<>0 begin rollback tran
                                    select N'fail' as result
                                    return
                                end
                                select N'success' as result
                            Commit Tran
                        ";
                        DbParameters p = new DbParameters();
                        p.Add("@role_id", id);

                        result = Utils.ObjectToStr(Dbl.ZZCMS.CreateSqlScalar(sql, p));

                    }
                    if (result == "success")
                    {
                        res = true;
                        strMsg = "删除成功！";
                    }
                    else
                    {
                        res = false;
                        strMsg = "删除失败！";
                    }

                }
                catch (Exception ex)//循环删除，异常才报删除错误
                {
                    res = false;
                    strMsg = "删除过程中出现异常！";//调试过程中+ex.ToString();
                }
            }
            return Json(new { status = (res ? 1 : 0), message = strMsg });
        }
        #endregion

        #endregion

        #region --管理日志--
        [NavAttr(NavName = "sys_log")]
        public ActionResult Manager_Log()
        {
            string keyword = Request.Query["keyword"];//搜索关键字
            //string strWhere = string.Empty;
            //if (!string.IsNullOrEmpty(keyword))
            //{
            //    strWhere += " and (user_name like  '%" + keyword + "%' or action_type like '%" + keyword + "%')";
            //}
            //string strSql = "Select Count(*) From nc_manager_log where 1=1 " + strWhere;
            //ViewBag.TotalCount = Dbl.ZZCMS.CreateSqlScalar(strSql);
            //ViewBag.TotalCount = 1;

            ViewBag.keyword = keyword;
            return View();
        }
        /// <summary>
        /// 管理日志列表
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_log")]
        public string Manager_Log_List()
        {
            int pageSize = Utils.StrToInt(Request.Query["page_size"]);
            int pageIndex = Utils.StrToInt(Request.Query["page_index"], 0) + 1;

            string keyword = Request.Query["keyword"];//搜索关键字
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(keyword))
            {
                strWhere += " and (user_name like  '%" + keyword + "%' or action_type like '%" + keyword + "%')";
            }
            string filedOrder = " add_time desc,id desc";
            string fields = " id, user_id, user_name, action_type, remark, user_ip, add_time  ";

            DbParameters p = new DbParameters();
            p.Add("@Tables", "nc_manager_log");
            p.Add("@PrimaryKey", "id");
            p.Add("@Fields", fields);
            p.Add("@Filter", strWhere);
            p.Add("@Sort", filedOrder);
            p.Add("@PageSize", pageSize);
            p.Add("@CurrentPage", pageIndex);
            p.AddOut("@TotalCount", "int", 4);

            DataTable dt = Dbl.ZZCMS.CreateDataTable("Pr_PageView", p);
            int recordCount = Utils.StrToInt(p[p.Length - 1].Value.ToString(), 0);

            //StringBuilder strSql = new StringBuilder();
            //strSql.Append(@"SELECT id, user_id, user_name, action_type, remark, user_ip, add_time FROM nc_manager_log");
            //if (strWhere != "")
            //{
            //    strSql.Append(" where 1=1 " + strWhere);
            //}

            //int recordCount = Convert.ToInt32(Dbl.ZZCMS.CreateSqlScalar(PagingHelper.CreateCountingSql(strSql.ToString())));
            //DataTable dt = Dbl.ZZCMS.CreateSqlDataTable(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));

            //return Json(dt.ToJson());
            string result = "{\"rows\":" + JsonHelper.DataTableToJSON(dt) + ",\"total\":" + recordCount + "}";
            return result;
        }
        /// <summary>
        /// 删除管理日志
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_log")]
        public JsonResult Manager_Log_Del()
        {
            bool res = true;
            string result = string.Empty;
            try
            {
                int sucCount = Manager_Log_Delete(7);
                res = true;
                strMsg = "删除管理日志" + sucCount + "条";
            }
            catch (Exception ex)//循环删除，异常才报删除错误
            {
                res = false;
                strMsg = "删除过程中出现异常！" + ex.ToString();//调试过程中+ex.ToString();
            }
            return Json(new { status = (res ? 1 : 0), message = strMsg });
        }
        ///// <summary>
        ///// 删除7天前的日志数据
        ///// </summary>
        [NavAttr(NavName = "sys_log")]
        public int Manager_Log_Delete(int dayCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from nc_manager_log ");
            strSql.Append(" where DATEDIFF(day, add_time, getdate()) > " + dayCount);

            return Dbl.ZZCMS.ExecuteSql(strSql.ToString());
        }
        #endregion

    }
}