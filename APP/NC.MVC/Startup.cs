using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NC.Lib;
using NC.Model.Models;
using NLog.Extensions.Logging;
using NLog.Web;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace NC.MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));//全局错误过滤日志
            }).AddControllersAsServices();

            //memcached(1)memcachedcore1
            services.AddEnyimMemcached(options => Configuration.GetSection("enyimMemcached").Bind(options));

            //添加以下代码以将上下文注册为服务,EF Core注入
            //var connection = Configuration.GetSection("ConnectionStrings")["ZZCMS"];
            var connection = Configuration.GetConnectionString("ZZCMS");
            services.AddDbContext<NCMVC_DBContext>(options => options.UseSqlServer(connection));

            //Session
            services.AddSession();


            #region --old--
            ////现只能添加一种，谁在后谁生效
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = "user_info";
            //})
            ////前台会员cookie服务
            //.AddCookie("user_info", options =>
            //{
            //    options.LoginPath = "/Login/LogIn";
            //    options.LogoutPath = "/Login/LogOff";
            //    options.Cookie.Path = "/";
            //});

            ////Cookie(1)添加 Cookie 服务
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = "admin_info";
            //})
            ////后台管理员cookie服务
            //.AddCookie("admin_info", options =>
            //{
            //    options.LoginPath = "/ad_min/Login/LogIn";
            //    options.LogoutPath = "/ad_min/Login/LogOff";
            //    options.Cookie.Path = "/";
            //}); 
            #endregion
            //Cookie(1)添加 Cookie 服务
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //后台管理员cookie服务
            .AddCookie(AdminAuthorizeAttribute.AdminAuthenticationScheme, options =>
            {
                options.LoginPath = "/ad_min/Login/Index";//登录路径
                options.LogoutPath = "/ad_min/Login/LogOff";//退出路径
                options.AccessDeniedPath = new PathString("/Error/Forbidden");//拒绝访问页面
                options.Cookie.Path = "/";
            })
            //前台用户cookie服务
            .AddCookie(UserAuthorizeAttribute.UserAuthenticationScheme, options =>
            {
                options.LoginPath = "/Login/Index";
                options.LogoutPath = "/Login/LogOff";
                options.AccessDeniedPath = new PathString("/Error/Forbidden");//拒绝访问页面
                options.Cookie.Path = "/";
            });

            //反转义，中文会被转义（如：浏览器上右键中文被编码）或使用@Html.Raw()反转义
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();//添加NLog  
            //读取Nlog配置文件，这里如果是小写，文件也一定是小写，否则linux下不识别  
            env.ConfigureNLog(Path.Combine("configs", "nlog.config"));
            //env.ConfigureNLog("nlog.config");//读取Nlog配置文件，这里如果是小写，文件也一定是小写，否则linux下不识别  

            //异常处理开发环境中使用详细异常页面
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //memcached(2)memcachedcore2
            app.UseEnyimMemcached();

            //发送友好状态码页面
            app.UseStatusCodePages();
            //添加静态文件中间件，静态文件路由到wwwroot目录，不进入MVC路由
            app.UseStaticFiles();

            //启用Session
            app.UseSession();
            //Cookie(2)使用Cookie的中间件
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                //域，配置
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
