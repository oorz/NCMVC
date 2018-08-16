# NCMVC
NCMVC一个基于Net Core2.0搭建的角色权限管理开发框架


遇见netcore2.0问题，优先看官网上的案例。

##--发布--

	1.dotnet publish或右键发布
	2.window系统上部署iis7+或运行dotnet nc.mvc.dll
	3.centos下运行测试（dll文件名大小写不能写错）dotnet NC.MVC.dll

##--日志记录--

1.微软已经内置了日志支持，日志级别：Trace -》Debug-》 Information -》Warning-》 Error-》 Critical；案例参考HomeController
	引用Microsoft.Extensions.Logging
	使用dotnet run运行项目，可以看到输出效果

2.使用NLog，NLog也是扩展的Microsoft.Extensions.Logging,添加NuGet包Microsoft.NETCore.App然后引用
	using NLog.Extensions.Logging;
	using NLog.Web;
	配置NLog.config文件，设置日志等级参数以及日志路径等
	默认日志写入到项目根目录xxx\bin\Debug\netcoreapp2.0\
3.集成一个自己写的Logger
	appsettings.json配置是否记录日志，以及记录位置设置。
	参考NC.Core>MSSQL>DbCommand.cs

##--linux下大小写敏感--

	如NLog组件，Startup.cs写小写，那么对应的nlog.config一定要全部小写。

##--centos--
	1.centos联网必须开启服务VMware NAT Service和VMware DHCP Service（VMnetDHCP）
	2.centos部署的站点必须通过nginx代理，这样才可以访问到虚拟机运行的netcore程序
	3.https://www.cnblogs.com/hager/p/5689493.html

##--Directory.GetCurrentDirectory()坑点--
	windows 下是 "\"， Mac OS and Linux 下是 "/"，这个待验证。如果成立可解决linux下创建文件夹问题

	??linux下创建文件夹问题待解决

	Directory.GetCurrentDirectory();此方法不是真正的获取应用程序的当前方法，而是执行dotnet命令所在目录；如dotnet publishoutput/nc.mvc.dll,日志记录的位置就会跟着改变。
	//获取应用程序的当前目录：
	dynamic type = (new Program()).GetType();
	string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);
	Console.WriteLine(currentDirectory);

#
通过控制器右键添加视图，转到视图页等特性都没有了，包括添加区域，自动进行区域配置也都无法使用，导致我们必须自己手动创建区域以及配置

##--2018-03-21##
1.Session封装
2.cookie读取封装

##--2018-06-21--##
1.ef core操作数据库时，参数要一致，如：long不能传int，不识别。

##--2018-06-22--##
1.Request.Form必须确保必须传递参数才能使用，否则报错System.InvalidOperationException;只有post下且有参数才可以用写Request.Form，否则报异常。

##--2018-08-16--##
1.linux mac 或win下使用vscode开发，建议现在vs中把项目框架搭建起来，再用vscode打开后运行dotnet restore这样项目间引用默认处理好了，相对于vscode里项目引用就简单很多。
