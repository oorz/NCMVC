using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NC.Common;
using NC.Lib;
using NC.Model.Models;

namespace NC.MVC.Areas.ad_min.Controllers
{
    [Area(AreaNames.Admin)]
    public class LoginController : Controller
    {
        NCMVC_DBContext ef;
        public LoginController(NCMVC_DBContext _ef)
        {
            ef = _ef;
        }


        /// <summary>
        /// 登录界面
        /// </summary>
        [AllowAnonymous]
        public ActionResult Index()
        {
            var cookieAdmin = HttpContext.AuthenticateAsync(AdminAuthorizeAttribute.AdminAuthenticationScheme);
            cookieAdmin.Wait();
            var principal = cookieAdmin.Result.Principal;
            if (principal != null)
            {
                var AdminName = principal.Claims.FirstOrDefault(x => x.Type == "AdminName")?.Value;
                var adminpwd = principal.Claims.FirstOrDefault(x => x.Type == "AdminPwd")?.Value;
                if (AdminName != "")
                {
                    //先取得该用户的随机密钥
                    string salt = GetSalt(AdminName);
                    adminpwd = DESEncrypt.Decrypt(adminpwd, salt);
                    NcManager bUser = getUserInfoByNameAndPwd(AdminName, adminpwd, true);
                    if (bUser != null && bUser.UserName == AdminName && bUser.IsLock == 0)
                    {
                        HttpContext.Session.Set(AdminAuthorizeAttribute.AdminAuthenticationScheme, ByteConvertHelper.Object2Bytes(bUser));//存储session

                        //是否定制了中间页面
                        if (!String.IsNullOrEmpty(UtilConf.Configuration["Site:JumpPage"]))
                        {
                            return new RedirectResult(UtilConf.Configuration["Site:JumpPage"]);//登录成功// RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return new RedirectResult("~/ad_min/Home/Index");//登录成功// RedirectToAction("Index", "Home");
                        }
                    }
                }
            }

            return View();
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName">用户ID</param>
        /// <param name="passWord">用户密码</param>
        /// <param name="rememberMe">是否记住密码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(string userName, string passWord, string rememberMe, string txtCode)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
            {
                TempData["SessionLoseError"] = "用户名和密码不能为空!";
                return RedirectToAction("Index");
            }
            //if (txtCode.Equals(""))
            //{
            //    TempData["SessionLoseError"] = "请输入验证码!";
            //    return RedirectToAction("Index");
            //}
            //if (Session[JHKeys.SESSION_CODE] == null)
            //{
            //    TempData["SessionLoseError"] = "系统找不到验证码!";
            //    return RedirectToAction("Index");
            //}
            //if (txtCode.ToLower() != Session[JHKeys.SESSION_CODE].ToString().ToLower())
            //{
            //    TempData["SessionLoseError"] = "验证码输入不正确!";
            //    return RedirectToAction("Index");
            //}

            NcManager bUser = getUserInfoByNameAndPwd(userName, passWord, true);
            if (bUser != null && bUser.UserName == userName && bUser.IsLock == 0)
            {
                HttpContext.Session.Set(AdminAuthorizeAttribute.AdminAuthenticationScheme, ByteConvertHelper.Object2Bytes(bUser));//存储session
                //获取单点登录的Server
                string ssoserver = UtilConf.Configuration["Site:sso:server"];

                if (string.IsNullOrEmpty(ssoserver))
                {
                    var user = new ClaimsPrincipal(
                     new ClaimsIdentity(new[]
                     {
                        new Claim("AdminName",bUser.UserName),
                        new Claim("AdminPwd",bUser.Password),
                     },
                     AdminAuthorizeAttribute.AdminAuthenticationScheme)
                 );
                    HttpContext.SignInAsync(AdminAuthorizeAttribute.AdminAuthenticationScheme, user, new AuthenticationProperties
                    {
                        //ExpiresUtc = DateTime.UtcNow.AddMinutes(1),// 有效时间
                        ExpiresUtc = DateTimeOffset.Now.Add(TimeSpan.FromHours(1)), //有效时间1小时
                        IsPersistent = true,
                        AllowRefresh = false
                    });
                    string returnUrl = Request.Query["returnUrl"];
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return new RedirectResult(returnUrl);//登录成功，跳转到来源页面
                    }
                    //是否定制了中间页面
                    string jumpPage = UtilConf.Configuration["Site:JumpPage"];
                    if (!String.IsNullOrEmpty(jumpPage))
                    {
                        return new RedirectResult(jumpPage);//登录成功// RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return new RedirectResult("~/ad_min/Home/Index");//登录成功
                    }
                }
                else
                {
                    return new RedirectResult("~/SSO/RegisterSite");//跳转到注册同盟网站页面
                }
            }
            else if (bUser != null && bUser.UserName == userName && bUser.IsLock != 0)
            {
                TempData["SessionLoseError"] = "用户未授权，请联系管理员!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["SessionLoseError"] = "用户名或密码不正确!";
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(AdminAuthorizeAttribute.AdminAuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }


        #region --用户表操作--
        /// <summary>
        /// 通过用户名和用户密码获取用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public NcManager getUserInfoByNameAndPwd(string userName, string pwd, bool is_encrypt)
        {
            //不同的加密方式
            string encryType = UtilConf.Configuration["Site:PwdEncryType"];
            switch (encryType)
            {
                case "MD5":
                    {
                        pwd = MD5Comm.Get32MD5One(pwd);
                        break;
                    }
                case "DES":
                    {
                        //检查一下是否需要加密
                        if (is_encrypt)
                        {
                            //先取得该用户的随机密钥
                            string salt = GetSalt(userName);

                            if (string.IsNullOrEmpty(salt))
                            {
                                return null;
                            }
                            //把明文进行加密重新赋值
                            pwd = DESEncrypt.Encrypt(pwd, salt);
                        }
                        break;
                    }
            }
            return ef.NcManager.Where(E => E.UserName == userName && E.Password == pwd).FirstOrDefault();
        }
        /// <summary>
        /// 获取加密盐
        /// </summary>
        public string GetSalt(string userName)
        {
            var adminInfo = ef.NcManager.Where(E => E.UserName == userName).FirstOrDefault();
            if (adminInfo != null)
            {
                return adminInfo.Salt;
            }
            return "";
            //return ef.manager.Where(E => E.user_name == userName).FirstOrDefault().salt;
        }
        /// <summary>
        /// 通过用户名获取用户信息单点时候根据用户名查询
        /// </summary>
        /// <param name="userName">用户名</param>
        public NcManager getUserInfoByName(string userName)
        {
            return ef.NcManager.Where(E => E.UserName == userName).FirstOrDefault();
        }
        #endregion

        /// <summary>
        /// 单点登录失败的也面
        /// </summary>
        /// <returns></returns>
        [SkipAdminAuthorize]
        public ActionResult IllegalUser(string msg)
        {
            ViewBag.msg = msg;
            return View();
        }

    }
}