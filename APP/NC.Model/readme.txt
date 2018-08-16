在core中，微软去掉了.edmx
通过程序包管理器控制台：
Scaffold-DbContext "Server=192.168.2.66\MSSQL2017;uid=sa;pwd=123456;Database=NCMVC_DB;Integrated Security=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
生成model类。
-- 2018-06-14 --
注意事项：
	执行语句前解决方案必须要能够生成成功。
	如果有现有models用下面语句更新
Scaffold-DbContext -Force "Server=192.168.2.66\MSSQL2017;uid=sa;pwd=123456;Database=NCMVC_DB;Integrated Security=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models