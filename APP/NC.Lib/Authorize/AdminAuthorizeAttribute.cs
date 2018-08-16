using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using NC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.Lib
{
    /// <summary>
    /// 后台登录验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AdminAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public const string AdminAuthenticationScheme = "AdminAuthenticationScheme";//自定义一个默认的登录方案
        public AdminAuthorizeAttribute()
        {
            this.AuthenticationSchemes = AdminAuthenticationScheme;
        }
        public virtual void OnAuthorization(AuthorizationFilterContext filterContext)
        {

            //获取对应Scheme方案的登录用户呢？使用HttpContext.AuthenticateAsync
            var authenticate = filterContext.HttpContext.AuthenticateAsync(AdminAuthorizeAttribute.AdminAuthenticationScheme);
            authenticate.Wait();
            if (authenticate.Result.Succeeded || this.SkipAdminAuthorize(filterContext.ActionDescriptor))
            {
                return;
            }
            //if (filterContext.HttpContext.User.Identity.IsAuthenticated || this.SkipAdminAuthorize(filterContext.ActionDescriptor))
            //{
            //    return;
            //}

            HttpRequest httpRequest = filterContext.HttpContext.Request;
            if (httpRequest.IsAjaxRequest())
            {
                string msg = "未登录或登录超时，请重新登录";

                string retResult = "{\"status\":" + JHEnums.ResultStatus.NotLogin + ",\"msg\":\"" + msg + "\"}";
                string json = JsonHelper.ObjectToJSON(retResult);
                ContentResult contentResult = new ContentResult() { Content = json };
                filterContext.Result = contentResult;
                return;
            }
            else
            {
                string url = filterContext.HttpContext.Content("~/ad_min/login");
                url = string.Concat(url, "?returnUrl=", httpRequest.Path);

                RedirectResult redirectResult = new RedirectResult(url);
                filterContext.Result = redirectResult;
                return;
            }
        }
        /// <summary>
        /// 如果控制器类上有跳过检查SkipCheckLoginAttribute特性标签，则直接return true 
        /// </summary>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        protected virtual bool SkipAdminAuthorize(ActionDescriptor actionDescriptor)
        {
            bool skipAuthorize = actionDescriptor.FilterDescriptors.Where(a => a.Filter is SkipAdminAuthorizeAttribute).Any();
            if (skipAuthorize)
            {
                return true;
            }

            return false;
        }
    }
    /// <summary>
    /// 如果控制器类上有SkipCheckLoginAttribute特性标签，则直接return  ,如果控action方法上有SkipCheckLoginAttribute特性标签，则直接return  
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class SkipAdminAuthorizeAttribute : Attribute, IFilterMetadata
    {
    }
}
