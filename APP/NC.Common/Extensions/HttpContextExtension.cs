using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Microsoft.AspNetCore.Http
{
    public static class HttpContextExtension
    {
        public static string Content(this HttpContext httpContext, string contentPath)
        {
            if (string.IsNullOrEmpty(contentPath))
            {
                return null;
            }
            else if (contentPath[0] == '~')
            {
                var segment = new PathString(contentPath.Substring(1));
                var applicationPath = httpContext.Request.PathBase;

                return applicationPath.Add(segment).Value;
            }

            return contentPath;
        }

        public static bool IsAjaxRequest(this HttpContext httpContext)
        {
            return httpContext.Request.IsAjaxRequest();
        }

        public static string GetClientIP(this HttpContext httpContext)
        {
            var ip = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = httpContext.Connection.RemoteIpAddress.ToString();
            }

            if (ip == "::1")
                ip = "127.0.0.1";

            return ip;
        }

        public static string GetRequestUrl(this HttpContext httpContext)
        {
            HttpRequest httpRequest = httpContext.Request;

            string scheme = httpRequest.Scheme;
            string host = httpRequest.Host.Value;
            string path = httpRequest.Path.Value;
            string queryString = httpRequest.QueryString.Value;

            string url = $"{scheme}://{host}{path}{queryString}";

            return url;
        }
        public static string GetUrlReferer(this HttpContext httpContext)
        {
            string referrer = httpContext.Request.Headers["Referer"];
            return referrer;
        }
    }
}
