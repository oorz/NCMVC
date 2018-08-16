using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NC.Lib;

namespace NC.MVC.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
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
        public IActionResult Login(string userName, string passWord, string rememberMe, string txtCode)
        {
            var user = new ClaimsPrincipal(
             new ClaimsIdentity(new[]
             {
                        new Claim("UserName","UserNameValue"),
                        new Claim("UserPwd","UserPwdValue"),
             }, UserAuthorizeAttribute.UserAuthenticationScheme)
            );
            HttpContext.SignInAsync(UserAuthorizeAttribute.UserAuthenticationScheme, user, new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60),// 有效时间
                //ExpiresUtc = DateTimeOffset.Now.Add(TimeSpan.FromDays(7)), // 有效时间
                IsPersistent = true,
                AllowRefresh = false
            });
            return new RedirectResult("~/Home/Index");//登录成功
        }
    }
}