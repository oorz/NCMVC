using System;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Mvc;

using NC.Common;
using NC.Core;
using NC.Model.Models;
using NC.Lib;
using NC.Model;

namespace NC.MVC.Areas.ad_min.Controllers
{
    [Area(AreaNames.Admin)]
    public class SettingsController : AdminBase
    {
        public SettingsController(NCMVC_DBContext _dblEf) : base(_dblEf)
        {
        }

        private string action = JHEnums.ActionEnum.Add.ToString(); //操作类型
        protected string strStatus = string.Empty;
        protected string strMsg = string.Empty;
        #region --系统设置--
        [NavAttr(NavName = "site_config", ActionType = "View")]
        public IActionResult Sys_Config()
        {
            return View(siteConf);
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="model"></param>
        [NavAttr(NavName = "site_config", ActionType = "Edit")]
        public JsonResult SysConfigSave()
        {
            siteconfig model = configSite.loadConfig();
            try
            {
                model.IsEmail = Utils.StrToInt(Request.Form["rblEmail"], 0);
                model.webname = Request.Form["webname"];
                model.weburl = Request.Form["weburl"];
                model.weblogo = Request.Form["weblogo"];
                model.webcompany = Request.Form["webcompany"];
                model.webaddress = Request.Form["webaddress"];
                model.webtel = Request.Form["webtel"];
                model.webfax = Request.Form["webfax"];
                model.webmail = Request.Form["webmail"];
                model.webcrod = Request.Form["webcrod"];
                model.webtitle = Request.Form["webtitle"];
                model.webkeyword = Request.Form["webkeyword"];
                model.webdescription = Utils.DropHTML(Request.Form["webdescription"]);
                model.webcopyright = Request.Form["webcopyright"];

                model.webpath = Request.Form["webpath"];
                model.webmanagepath = Request.Form["webmanagepath"];
                model.staticstatus = Utils.StrToInt(Request.Form["staticstatus"], 0);
                model.staticextension = Request.Form["staticextension"];

                model.memberstatus = !string.IsNullOrEmpty(Request.Form["memberstatus"]) ? 1 : 0;
                model.commentstatus = !string.IsNullOrEmpty(Request.Form["commentstatus"]) ? 1 : 0;
                model.logstatus = !string.IsNullOrEmpty(Request.Form["logstatus"]) ? 1 : 0;
                model.webstatus = !string.IsNullOrEmpty(Request.Form["webstatus"]) ? 1 : 0;

                model.webclosereason = Request.Form["webclosereason"];
                model.webcountcode = Request.Form["webcountcode"];

                model.smsusername = Request.Form["smsusername"];
                string defaultpassword = "0|0|0|0"; //默认显示密码
                //判断密码是否更改
                if (Request.Form["smspassword"].ToString() != "" && Request.Form["smspassword"].ToString() != defaultpassword)
                {
                    model.smspassword = MD5Comm.Get32MD5One(Request.Form["smspassword"].ToString());
                }

                model.emailsmtp = Request.Form["emailsmtp"];
                model.emailport = Utils.StrToInt(Request.Form["emailport"].ToString(), 25);
                model.emailfrom = Request.Form["emailfrom"];
                model.emailusername = Request.Form["emailusername"];
                //判断密码是否更改
                if (Request.Form["emailpassword"] != defaultpassword)
                {
                    model.emailpassword = DESEncrypt.Encrypt(Request.Form["emailpassword"], model.sysencryptstring);
                }
                model.emailnickname = Request.Form["emailnickname"];

                model.filepath = Request.Form["filepath"];
                model.filesave = Utils.StrToInt(Request.Form["filesave"], 2);
                model.fileextension = Request.Form["fileextension"];
                model.attachsize = Utils.StrToInt(Request.Form["attachsize"], 0);
                model.imgsize = Utils.StrToInt(Request.Form["imgsize"], 0);
                model.imgmaxheight = Utils.StrToInt(Request.Form["imgmaxheight"], 0);
                model.imgmaxwidth = Utils.StrToInt(Request.Form["imgmaxwidth"], 0);
                model.thumbnailheight = Utils.StrToInt(Request.Form["thumbnailheight"], 0);
                model.thumbnailwidth = Utils.StrToInt(Request.Form["thumbnailwidth"], 0);
                model.watermarktype = Utils.StrToInt(Request.Form["watermarktype"], 0);
                model.watermarkposition = Utils.StrToInt(Request.Form["watermarkposition"], 9);
                model.watermarkimgquality = Utils.StrToInt(Request.Form["watermarkimgquality"], 80);
                model.watermarkpic = Request.Form["watermarkpic"];
                model.watermarktransparency = Utils.StrToInt(Request.Form["watermarktransparency"], 5);
                model.watermarktext = Request.Form["watermarktext"];
                model.watermarkfont = Request.Form["watermarkfont"];
                model.watermarkfontsize = Utils.StrToInt(Request.Form["watermarkfontsize"], 12);

                configSite.saveConifg(model);
                strStatus = "1";
                strMsg = "修改系统配置成功";
            }
            catch
            {
                strStatus = "0";
                strMsg = "文件写入失败，请检查是否有权限！";
            }
            return Json(new { status = strStatus, msg = strMsg });
        }
        #endregion

        #region --基础字典列表--
        /// <summary>
        /// 字典列表
        /// </summary>
        [NavAttr(NavName = "sys_dictbase")]
        public ActionResult DictBase_List()
        {
            string keyword = Request.Query["keyword"];//搜索关键字
            ViewBag.keyword = keyword;
            return View();
        }
        /// <summary>
        /// 基础字典列表
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_dictbase")]
        public string DictBase_Bind()
        {
            int pageSize = Utils.StrToInt(Request.Query["page_size"]);
            int pageIndex = Utils.StrToInt(Request.Query["page_index"], 0) + 1;
            string keyword = Request.Query["keyword"];//搜索关键字
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(keyword))
            {
                strWhere += " and (Key_Type like  '%" + keyword + "%' or Key_Value like '%" + keyword + "%')";
            }
            string filedOrder = " id Desc,Sort_ID asc";
            string fields = " id , Key_Type , Key_Code , Key_Value , Code_Value , Sort_ID , OStatus  ";

            DbParameters p = new DbParameters();
            p.Add("@Tables", "nc_dictbase");
            p.Add("@PrimaryKey", "id");
            p.Add("@Fields", fields);
            p.Add("@Filter", strWhere);
            p.Add("@Sort", filedOrder);
            p.Add("@PageSize", pageSize);
            p.Add("@CurrentPage", pageIndex);
            p.AddOut("@TotalCount", "int", 4);

            DataTable dt = Dbl.ZZCMS.CreateDataTable("Pr_PageView", p);
            int recordCount = Utils.StrToInt(p[p.Length - 1].Value.ToString(), 0);
            //objecttojson转，不可以直接rows=dt会转成xml格式
            //JsonHelper.ObjectToJSON(new { rows = JsonHelper.DataTableToJSON(dt), total = recordCount });
            string result = "{\"rows\":" + JsonHelper.DataTableToJSON(dt) + ",\"total\":" + recordCount + "}";
            return result;
        }
        /// <summary>
        /// 删除字典
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_dictbase")]
        public JsonResult DictBase_Del(string ids)
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
                        NcDictbase model = dblEf.Find<NcDictbase>(id);
                        if (model != null)
                        {
                            //dblEf.NcDictbase.Remove(m => m.id);//.Delete<NcDictbase>(m => m.id == id);//删除,linq to entities不识别xx[index]这种格式，需要先赋值临时变量
                            dblEf.Remove<NcDictbase>(model);
                            dblEf.SaveChanges();
                            res = true;
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

        /// <summary>
        /// 字典-编辑
        /// </summary>
        [NavAttr(NavName = "sys_dictbase")]
        public ActionResult DictBase_Edit()
        {
            action = Request.Query["act"];
            int id = Utils.StrToInt(Request.Query["id"]);
            NcDictbase model = new NcDictbase();
            if (action.ToLower() == JHEnums.ActionEnum.Edit.ToString().ToLower() && id != 0) //修改
            {
                model = dblEf.Find<NcDictbase>(id);
            }
            else
            {
                model.SortId = 1;
            }
            ViewBag.action = action;
            ViewBag.id = id;
            return View(model);
        }
        /// <summary>
        /// 字典保存方法
        /// </summary>
        /// <returns></returns>
        [NavAttr(NavName = "sys_dictbase")]
        public JsonResult DictBaseSave()
        {
            NcDictbase model = new NcDictbase();
            action = Request.Form["act"];
            if (action.ToLower() == JHEnums.ActionEnum.Edit.ToString().ToLower())//修改
            {
                int id = Utils.StrToInt(Request.Form["id"]);
                model = dblEf.Find<NcDictbase>(id);
                model = DictBaseSetModel(model);

                if (DictBaseUpdate(model))
                {
                    strStatus = "1";
                    strMsg = "保存成功";
                }
            }
            else//新增
            {
                model = DictBaseSetModel(model);

                if (DictBaseAdd(model))
                {
                    strStatus = "1";
                    strMsg = "保存成功";
                }
            }
            return Json(new { status = strStatus, msg = strMsg });
        }
        /// <summary>
        /// 增加一条数据
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_dictbase")]
        public bool DictBaseAdd(NcDictbase model)
        {
            //return false;
            dblEf.NcDictbase.Add(model);
            return dblEf.SaveChanges() > 0;
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_dictbase")]
        public bool DictBaseUpdate(NcDictbase model)
        {
            //return dblEf.Update<dictbase>(model);
            dblEf.Update<NcDictbase>(model);
            return dblEf.SaveChanges() > 0;
        }
        /// <summary>
        /// 字典获取赋值
        /// </summary>
        private NcDictbase DictBaseSetModel(NcDictbase model)
        {
            model.KeyType = Request.Form["txtKey_Type"];
            model.KeyCode = Request.Form["txtKey_Code"];
            model.KeyValue = Request.Form["txtKey_Value"];
            model.SortId = Utils.StrToInt(Request.Form["txtSortId"]);
            return model;
        }
        #endregion

        #region --缓存维护--
        /// <summary>
        /// 缓存列表展示
        /// </summary>
        [NavAttr(NavName = "sys_dictcache")]
        public ActionResult DictCache_List()
        {
            string keyword = Request.Query["keyword"];//搜索关键字
            ViewBag.keyword = keyword;
            return View();
        }
        /// <summary>
        /// 缓存列表绑定
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_dictcache")]
        public string DictCache_Bind()
        {
            int pageSize = Utils.StrToInt(Request.Query["page_size"]);
            int pageIndex = Utils.StrToInt(Request.Query["page_index"], 0) + 1;
            string keyword = Request.Query["keyword"];//搜索关键字
            string strWhere = string.Empty;
            if (!string.IsNullOrEmpty(keyword))
            {
                strWhere += " and (Title like  '%" + keyword + "%' or Depend like '%" + keyword + "%' or Cache_Key like '%" + keyword + "%')";
            }
            string filedOrder = " sort_id asc,cache_id asc";
            string fields = " Cache_ID, Cache_Desc, Created_Name,convert(nvarchar(20),Created_Time,120) as Created_Time, Updated_Name, Updated_Time, OStatus, Parent_ID, Class_List, Class_Layer, Sort_ID, Title, Depend, Cache_Key, Cache_Exp ";

            DbParameters p = new DbParameters();
            p.Add("@Tables", "nc_dictcache");
            p.Add("@PrimaryKey", "cache_id");
            p.Add("@Fields", fields);
            p.Add("@Filter", strWhere);
            p.Add("@Sort", filedOrder);
            p.Add("@PageSize", pageSize);
            p.Add("@CurrentPage", pageIndex);
            p.AddOut("@TotalCount", "int", 4);

            DataTable dt = Dbl.ZZCMS.CreateDataTable("Pr_PageView", p);
            int recordCount = Utils.StrToInt(p[p.Length - 1].Value.ToString(), 0);
            //string result = JsonHelper.ObjectToJSON(new { rows = dt, total = recordCount });
            string result = "{\"rows\":" + JsonHelper.DataTableToJSON(dt) + ",\"total\":" + recordCount + "}";
            return result;
        }
        /// <summary>
        /// 删除缓存字典
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_dictcache")]
        public JsonResult DictCache_Del(string ids)
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
                        long id = Utils.StrToInt(arrId[i]);
                        //dblEf.Delete<NcDictcache>(m => m.Cache_ID == id);//删除,linq to entities不识别xx[index]这种格式，需要先赋值临时变量
                        NcDictcache model = dblEf.Find<NcDictcache>(id);
                        if (model != null)
                        {
                            dblEf.Remove<NcDictcache>(model);
                            dblEf.SaveChanges();
                            res = true;
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
        /// <summary>
        /// 更新字典排序
        /// </summary>
        [NavAttr(NavName = "sys_dictcache", ActionType = "Edit")]
        public JsonResult UpdateSort_DictCache(string ids, string sorts)
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
                    long id = Utils.StrToInt(arrId[i]);
                    NcDictcache model = dblEf.Find<NcDictcache>(id);
                    if (model != null && Utils.StrToInt(arrSort[i]) != model.SortId)//减少数据库访问次数
                    {
                        model.SortId = Utils.StrToInt(arrSort[i]);
                        ////var res = dblEf.NcDictcache.Update(model);
                        //res = dblEf.SaveChanges() > 0;
                        //dblEf.NcDictcache.Attach(model);
                        //var ress = (dblEf.Entry<NcDictcache>(model).Property<int?>(v => v.SortId).IsModified = true);

                        res = dblEf.SaveChanges() > 0;
                    }
                }
            }
            return Json(new { status = (res ? 1 : 0), message = "保存成功！" });
        }
        /// <summary>
        /// 缓存字典-编辑
        /// </summary>
        [NavAttr(NavName = "sys_dictcache")]
        public ActionResult DictCache_Edit()
        {
            action = Request.Query["act"];
            long id = Utils.StrToInt(Request.Query["id"]);
            NcDictcache model = new NcDictcache();
            if (action.ToLower() == JHEnums.ActionEnum.Edit.ToString().ToLower() && id != 0) //修改
            {
                model = dblEf.Find<NcDictcache>(id);
            }
            else
            {
                model.SortId = 99;
            }
            ViewBag.action = action;
            ViewBag.id = id;
            return View(model);
        }
        /// <summary>
        /// 缓存键值对编辑保存
        /// </summary>
        /// <returns></returns>
        [NavAttr(NavName = "sys_dictcache")]
        public JsonResult DictCacheSave()
        {
            NcDictcache model = new NcDictcache();
            action = Request.Form["act"];
            if (action.ToLower() == JHEnums.ActionEnum.Edit.ToString().ToLower())//修改
            {
                long id = Utils.StrToInt(Request.Form["id"]);
                model = dblEf.Find<NcDictcache>(id);
                model = DictCacheSetModel(model);
                model.UpdatedTime = DateTime.Now;

                if (DictCacheUpdate(model))
                {
                    strStatus = "1";
                    strMsg = "保存成功";
                }
            }
            else//新增
            {
                model = DictCacheSetModel(model);
                model.CreatedTime = DateTime.Now;
                if (DictCacheAdd(model))
                {
                    strStatus = "1";
                    strMsg = "保存成功";
                }
            }
            return Json(new { status = strStatus, msg = strMsg });
        }
        /// <summary>
        /// 增加一条数据
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_dictcache")]
        public bool DictCacheAdd(NcDictcache model)
        {
            dblEf.Add<NcDictcache>(model);
            return dblEf.SaveChanges() > 0;
            //return dblEf.Insert<NcDictcache>(model) != null;
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_dictcache")]
        public bool DictCacheUpdate(NcDictcache model)
        {
            dblEf.Update<NcDictcache>(model);
            return dblEf.SaveChanges() > 0;
            //return dblEf.Update<NcDictcache>(model);
        }
        /// <summary>
        /// 角色获取赋值
        /// </summary>
        private NcDictcache DictCacheSetModel(NcDictcache model)
        {
            model.Title = Request.Form["txtTitle"];
            model.Depend = Request.Form["txtDepend"];
            model.CacheKey = Request.Form["txtCache_Key"];
            model.CacheExp = Request.Form["txtCache_Exp"];
            model.CacheDesc = Request.Form["txtCache_Desc"];
            model.SortId = Utils.StrToInt(Request.Form["txtSortId"]);
            model.CreatedName = Request.Form["txtCreated_Name"];
            model.UpdatedName = Request.Form["txtUpdated_Name"];
            model.Ostatus = !string.IsNullOrEmpty(Request.Form["OStatus"]) ? 1 : 0;
            return model;
        }
        #endregion

        #region--生成JSON串--
        /// <summary>
        /// 生成JSON串页面
        /// </summary>
        public ActionResult CreateJson()
        {
            return View();
        }
        /// <summary>
        /// 生成JSON按钮
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_json", ActionType = "View")]
        public JsonResult CreateJsonSave()
        {
            try
            {
                string sql = Request.Form["txtSql"];
                string dsn = Request.Form["txtDsn"];
                string dbName = "$db." + Request.Form["txtName"];

                DataTable dt = Dbl.CreateCommand(dsn).CreateSqlDataTable(sql);

                //生成JSON数据
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[");

                //循环行
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    sb.Append("[");

                    //循环列
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string fldName = dr[j].ToString();
                        sb.AppendFormat("'{0}'", fldName);

                        if (j < (dt.Columns.Count - 1)) sb.Append(",");
                    }

                    if (i < (dt.Rows.Count - 1))
                        sb.AppendLine("],");
                    else
                        sb.AppendLine("]");
                }

                sb.AppendLine("];");
                strStatus = "1";
                strMsg = dbName + "=" + sb.ToString();

                return Json(new { status = strStatus, msg = strMsg });
            }
            catch (System.Exception ex)
            {
                strStatus = "0";
                strMsg = "生成异常！" + ex.Message;
                return Json(new { status = strStatus, msg = strMsg });
            }
        }
        /// <summary>
        /// 生成分类JSON
        /// </summary>
        [HttpPost]
        [NavAttr(NavName = "sys_json", ActionType = "View")]
        public JsonResult CreateCateJsonSave()
        {
            try
            {
                string dsn = Request.Form["txtDsn"];
                string dbName = "$db.category";

                DataTable dt = Dbl.CreateCommand(dsn).CreateSqlDataTable("select category_id,ParentId,catename from category where ParentId>0");

                //生成JSON数据
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[");

                //循环行
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    sb.Append("[");

                    //循环列
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string fldName = dr[j].ToString();
                        sb.AppendFormat("'{0}'", fldName);

                        if (j < (dt.Columns.Count - 1)) sb.Append(",");
                    }

                    if (i < (dt.Rows.Count - 1))
                        sb.AppendLine("],");
                    else
                        sb.AppendLine("]");
                }

                sb.AppendLine("];");
                strStatus = "1";
                strMsg = dbName + "=" + sb.ToString();

                return Json(new { status = strStatus, msg = strMsg });
            }
            catch (System.Exception ex)
            {
                strStatus = "0";
                strMsg = "生成异常！" + ex.Message;
                return Json(new { status = strStatus, msg = strMsg });
            }
        }
        #endregion

        #region --备份数据库--
        protected string dbname = "";
        string filepath = UtilConf.Configuration["Site:DbBackupPath"];// UtilConf.GetConnectionString("DbBackupPath");//"/dback/";
        protected DataTable dt;

        /// <summary>
        /// 备份数据库
        /// </summary>
        [NavAttr(NavName = "sys_databak")]
        public ActionResult DataBackUp()
        {
            dt = new DataTable();
            dt.Columns.Add("filename");
            dt.Columns.Add("createdate");

            var dbPath = Utils.GetMapPath(filepath);
            System.IO.DirectoryInfo oDir = new System.IO.DirectoryInfo(dbPath);
            System.IO.FileInfo[] aFiles = oDir.GetFiles();
            for (int i = 0; i < aFiles.Length; i++)
            {
                DataRow nrow = dt.NewRow();
                nrow["filename"] = aFiles[i].Name;
                nrow["createdate"] = aFiles[i].CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
                dt.Rows.Add(nrow);
            }
            ViewBag.DBack = dt;
            return View();
        }
        /// <summary>
        /// 备份操作
        /// </summary>
        /// <returns></returns>
        [NavAttr(NavName = "sys_databak")]
        public JsonResult Backfile()
        {
            string[] connstr = UtilConf.GetConnectionString("SqlDSN").Split(';');
            dbname = connstr[3].Split('=')[1];
            string filename = DateTime.Today.ToString("yyyy-MM-dd") + ".bak";
            string finame = filename;
            string oldpath = Utils.GetMapPath(filepath);
            int file_append = 0;
            while (System.IO.File.Exists(oldpath + filename))
            {
                file_append++;
                filename = System.IO.Path.GetFileNameWithoutExtension(finame) + "(" + file_append.ToString() + ")" + System.IO.Path.GetExtension(finame).ToLower();
            }
            try
            {
                //DataManager.Backup(dbname);
                DataManager.Backup(dbname, Utils.GetMapPath(filepath) + filename);
                strStatus = "1";
                strMsg = "备份成功！";
                return Json(new { status = strStatus, msg = strMsg });
            }
            catch (Exception ex)
            {
                strStatus = "0";
                strMsg = "生成异常！" + ex.Message;
                return Json(new { status = strStatus, msg = strMsg });
            }
        }
        /// <summary>
        /// 恢复备份
        /// </summary>
        /// <param name="fname"></param>
        [NavAttr(NavName = "sys_databak")]
        public JsonResult Restfile(string fname)
        {
            var path = Utils.GetMapPath(filepath + fname);
            try
            {
                string[] connstr = UtilConf.GetConnectionString("SqlDSN").Split(';');
                dbname = connstr[3].Split('=')[1];
                DataManager.Restore(dbname, path);
                strStatus = "1";
                strMsg = "恢复备份成功！";
                return Json(new { status = strStatus, msg = strMsg });
            }
            catch (Exception ex)
            {
                strStatus = "0";
                strMsg = "恢复备份异常！" + ex.Message;
                return Json(new { status = strStatus, msg = strMsg });
            }

        }
        /// <summary>
        /// 删除备份文件
        /// </summary>
        [NavAttr(NavName = "sys_databak")]
        public JsonResult DbackDel(string fname)
        {
            try
            {
                string filefullname = Utils.GetMapPath(filepath + fname);
                if (System.IO.File.Exists(filefullname))
                    System.IO.File.Delete(filefullname);

                strStatus = "1";
                strMsg = "删除备份成功！";
                return Json(new { status = strStatus, msg = strMsg });
            }
            catch (Exception ex)
            {
                strStatus = "0";
                strMsg = "删除备份异常！" + ex.Message;
                return Json(new { status = strStatus, msg = strMsg });
            }
        }
        #endregion

        #region --数据库信息运行状况查看--
        /// <summary>
        /// 备份数据库
        /// </summary>
        [NavAttr(NavName = "sys_datainfo")]
        public ActionResult DataInfo()
        {
            string[] connstr = UtilConf.GetConnectionString("SqlDSN").Split(';');
            dbname = connstr[3].Split('=')[1];
            DataTable DataStatus = DataManager.GetDataStatus(dbname);//状态信息
            DataTable FileInfo = DataManager.GetFileInfo(dbname);//状态信息
            DataTable Configure = DataManager.GetConfigure(dbname);//配置信息
            //DataTable TableStatus = DataManager.GetTableStatus(dbname);//表信息
            ViewBag.DataStatus = DataStatus;
            ViewBag.FileInfo = FileInfo;
            ViewBag.Configure = Configure;
            //ViewBag.TableStatus = TableStatus;
            return View();
        }
        #endregion
    }
}