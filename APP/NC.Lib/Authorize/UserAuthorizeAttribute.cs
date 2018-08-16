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
    /// 跳过检查属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class SkipUserAuthorizeAttribute : Attribute, IFilterMetadata
    {
    }
    /// <summary>
    /// 前台登录验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class UserAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public const string UserAuthenticationScheme = "UserAuthenticationScheme";//自定义一个默认的登录方案
        public UserAuthorizeAttribute()
        {
            this.AuthenticationSchemes = UserAuthenticationScheme;
        }
        public virtual void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            //获取对应Scheme方案的登录用户呢？使用HttpContext.AuthenticateAsync
            var authenticate = filterContext.HttpContext.AuthenticateAsync(UserAuthorizeAttribute.UserAuthenticationScheme);
            //authenticate.Wait();
            if (authenticate.Result.Succeeded || this.SkipUserAuthorize(filterContext.ActionDescriptor))
            {
                return;
            }
            //if (filterContext.HttpContext.User.Identity.IsAuthenticated || this.SkipUserAuthorize(filterContext.ActionDescriptor))
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
                string url = filterContext.HttpContext.Content("~/login");
                url = string.Concat(url, "?returnUrl=", httpRequest.Path);

                RedirectResult redirectResult = new RedirectResult(url);
                filterContext.Result = redirectResult;
                return;
            }
        }

        protected virtual bool SkipUserAuthorize(ActionDescriptor actionDescriptor)
        {
            bool skipAuthorize = actionDescriptor.FilterDescriptors.Where(a => a.Filter is SkipUserAuthorizeAttribute).Any();
            if (skipAuthorize)
            {
                return true;
            }

            return false;
        }
    }
}
