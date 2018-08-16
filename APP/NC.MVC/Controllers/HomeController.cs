using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NC.Core;
using System.Data;
using Microsoft.Extensions.Logging;
using NC.Common;
using Enyim.Caching;
using Microsoft.AspNetCore.Authorization;
using NC.Lib;

using NLog;
using NC.Model.Models;

namespace NC.MVC.Controllers
{
    [UserAuthorize]
    public class HomeController : Controller
    {
        static Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly ILogger<HomeController> _logger;

        private IMemcachedClient _memcachedClient;//调用memcachedcore3
        public HomeController(ILogger<HomeController> logger,
        IMemcachedClient memcachedClient)
        {
            _logger = logger;
            _memcachedClient = memcachedClient;//赋值memcachedcore4
        }
        //[Authorize(AuthenticationSchemes = "user_info")]
        public IActionResult Index()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "UserName")?.Value;
            ViewData["user"] = userId;

            //throw new System.Exception("Throw Exception");
            string s = Request.Query["s"];
            var path = AppContext.BaseDirectory;
            #region --测试日志--
            // Logger.Info("普通信息日志-----------");
            // Logger.Debug("调试日志-----------");
            // Logger.Error("错误日志-----------");
            // Logger.Fatal("异常日志-----------");
            // Logger.Warn("警告日志-----------");
            // Logger.Trace("跟踪日志-----------");
            // Logger.Log(NLog.LogLevel.Warn, "Log日志------------------");


            // //_logger.LogInformation("你访问了首页");
            // //_logger.LogWarning("警告信息");
            // //_logger.LogError("错误信息"); 
            #endregion

            #region --测试redis--
            RedisHelper.Default.StringSet("redis", "redis" + DateTime.Now, TimeSpan.FromSeconds(1000000));
            ViewData["redis"] = RedisHelper.Default.StringGet("redis");
            //RedisHelper.Default.KeyDelete("redis");
            #endregion

            #region --测试sql--
            DataTable dt = Dbl.ZZCMS.CreateSqlDataTable("select * from nc_manager");
            ViewData["mng"] = dt;
            #endregion

            #region --测试memcachedcore--


            //MCached.Set("depend2", "core", "memcached-core" + DateTime.Now, 59);
            //var re = MCached.Get("depend", "core");
            //ViewData["mhelper1"] = re;

            //MemcachedHelper.Remove("core");
            MemcachedHelper.Set("core", "memcachedcore" + DateTime.Now, 10);
            var mh = MemcachedHelper.Get("core");
            ViewData["mhelper"] = mh;

            //这种方式暂未找到合适赋值，待研究，赋值赋不上。
            //删/增/取memcachedcore5
            //var cacheKey = "memcachedcore";
            //await _memcachedClient.AddAsync(cacheKey, "memcachedcore" + DateTime.Now, 60);

            ////_memcachedClient.Add(cacheKey, "memcachedcore" + DateTime.Now, 60);//此方法赋不上值
            ////var result = _memcachedClient.Get(cacheKey);
            //var result = _memcachedClient.Get(cacheKey);
            //_memcachedClient.Remove(cacheKey);
            //ViewData["memcachedcore"] = result.ToString();
            #endregion

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
