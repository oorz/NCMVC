/* --
 * 作者：魏亚东
 * 日期：2014-06-13
 * 描述说明：
 * 数据管理类
 * 更改历史：
 * 
 * --*/

using System;
using System.Data;
using NC.Common;
namespace NC.Core
{
    /// <summary>
    /// 数据管理类
    /// </summary>
    public class DataManager
    {
        private static string _bakPath = UtilConf.Configuration["Site:DbBackupPath"];
        public static string BackUpPath { get { return _bakPath; } }

        #region DbCopmd 属性
        public static DbCommand DbComd
        {
            get { return Dbl.CreateCommand(); }
        }
        #endregion

        #region ** 备份数据库 **
        /// <summary>
        /// 备份数据库 DB_yyyy-MM-dd.bak
        /// </summary>        
        public static bool Backup(string dataName)
        {
            string path = _bakPath + dataName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".bak";
            return Backup(dataName, path);
        }
        public static bool Backup(string databaseName, string path)
        {
            try
            {
                string sql = @"backup database [" + databaseName + @"] to disk='" + path + @"' with checksum;";
                //System.Web.HttpContext.Current.Response.Write(sql);
                //System.Web.HttpContext.Current.Response.End();
                DbComd.ExecuteSql(sql);

                return true;
            }
            catch (System.Exception ex)
            {
                throw new Exception("\n---- " + DateTime.Now.ToString() + " DataManager.Backup()报错:----\n" + ex.Message + "--end--\n");
                return false;
            }
        }
        #endregion

        #region ** 恢复数据库 **
        /// <summary>
        /// 恢复数据库
        /// </summary>
        public static bool Restore(string databaseName, string path)
        {
            try
            {
                //先解除独占再进行恢复
                string sql = @"use master 
                            ALTER DATABASE [" + databaseName + @"] SET OFFLINE WITH ROLLBACK IMMEDIATE
                            restore database [" + databaseName + @"] from disk='" + path + @"' with replace 
                            ALTER  database  [" + databaseName + @"]  set   online  ";
                Dbl.CreateCommand().ExecuteSql(sql);

                return true;
            }
            catch (System.Exception ex)
            {
                throw new Exception("\n---- " + DateTime.Now.ToString() + " 报错:----\n" + ex.Message + "--end--\n");
                return false;
            }
        }
        #endregion

        #region ** 查看日志文件大小 **
        /// <summary>
        /// 查看所有数据库的日志文件大小，包括系统数据库
        /// </summary>
        public static DataTable GetLogSpaceAll()
        {
            string sql = "DBCC SQLPERF(logspace)";
            return DbComd.CreateSqlDataTable(sql);
        }
        /// <summary>
        /// 查看特定数据库的日志文件大小
        /// </summary>        
        public static DataTable GetLogSpace(string dataName)
        {
            try
            {
                string sql = @"
                Begin
                    --创建临时表--
                    CREATE TABLE tempdb.dbo.#tbl_DBLogSpaceUsage  
                    (  
                          DatabaseName NVARCHAR(128) ,  
                          LogSize NVARCHAR(25) ,  
                          LogSpaceUsed NVARCHAR(25) ,  
                          [Status] TINYINT  
                    )
                    INSERT INTO dbo.#tbl_DBLogSpaceUsage EXEC ('DBCC SQLPERF(LogSpace)');
                                
                    --查询特定数据库的结果-- 
                    SELECT  DatabaseName ,  
                            LogSize AS LogSizeInMB ,  
                            LogSpaceUsed LogspaceUsed_In_Percent ,  
                            [Status]  
                    FROM tempdb.dbo.#tbl_DBLogSpaceUsage  
                    WHERE Databasename = @DataBaseName   

                    --删除临时表--
                    DROP TABLE tempdb.dbo.#tbl_DBLogSpaceUsage  
                End             
            ";
                DbParameters p = new DbParameters();
                p.Add("@DataBaseName", dataName);

                return DbComd.CreateSqlDataTable(sql, p);
            }
            catch (System.Exception ex)
            {
                throw new Exception("\n---- " + DateTime.Now.ToString() + " GetLogSpace() 报错:----\n" + ex.Message + "--end--\n");
                return null;
            }
        }
        #endregion

        #region ** 查看数据库状态 **
        /// <summary>
        /// 查看数据库配置信息
        /// </summary>
        public static DataTable GetConfigure(string dataName)
        {
            try
            {
                string sql = @"
                    use [" + dataName + @"]


                    exec master.dbo.sp_configure
                

                ";
                return Dbl.Command.CreateSqlDataTable(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("\n--" + DateTime.Now.ToString() + "GetConfigure() 报错:----\n" + ex.Message + "--end--\n");
                return null;
            }
        }
        /// <summary>
        /// 查看全部数据库文件信息
        /// </summary>
        public static DataTable GetFileInfoAll()
        {
            return DbComd.CreateSqlDataTable("exec sp_helpdb");
        }
        /// <summary>
        /// 查看当前数据库文件信息
        /// </summary>
        public static DataTable GetFileInfo(string dataName)
        {
            return DbComd.CreateSqlDataTable("select * from [" + dataName + "].dbo.sysfiles");
        }
        /// <summary>
        /// 获取数据库状态
        /// </summary>
        public static DataTable GetDataStatus(string dataName)
        {
            try
            {
                return DbComd.CreateSqlDataTable(@"
                    use [" + dataName + @"]

                    SELECT 
                        @@SERVERNAME            N'服务器名称',
                        @@CONNECTIONS           N'总连接数' ,		
	                    @@MAX_CONNECTIONS       N'最大连接',	
	                    @@TIMETICKS             TimeTicks ,	
	                    @@CPU_BUSY              N'总工作时间' ,		    
	                    @@IDLE                  N'总空闲时间' ,			
	                    @@IO_BUSY               N'总IO输入输出' ,		
	                    @@PACK_RECEIVED         N'接收包' ,		
	                    @@PACK_SENT             N'发送包' ,			
	                    @@PACKET_ERRORS         N'错误包' ,		
	                    @@TOTAL_READ            N'读' ,				
	                    @@TOTAL_WRITE           N'写' ,				
	                    @@TOTAL_ERRORS          N'读写错误'		
                ");
            }
            catch (System.Exception ex)
            {
                //Log.Error("\n---- " + DateTime.Now.ToString() + " GetDataStatus() 报错:----\n" + ex.Message + "--end--\n");
                return null;
            }
        }
        #endregion

        #region ** 查看数据表状态统计 **
        /// <summary>
        /// 数据表统计信息
        /// </summary>
        /// <returns></returns>
        public static DataTable GetTableStatus(string dataName)
        {
            return DbComd.CreateDataTable("sp_toptables '" + dataName + "'");
        }
        #endregion

        #region ** 查看存储过程或函数 **
        /// <summary>
        /// 查看存储过程或函数
        /// </summary>
        /// <returns></returns>
        public static DataTable GetProcOrFunc()
        {
            string sql = "exec sp_stored_procedures";
            return DbComd.CreateSqlDataTable(sql);
        }
        /// <summary>
        /// 查看源代码
        /// </summary>
        public static DataTable GetProcOrFuncText(string pfName)
        {
            string sql = "exec sp_helptext @ProcFuncName";
            DbParameters p = new DbParameters();
            p.Add("@ProcFuncName", pfName);

            return DbComd.CreateSqlDataTable(sql, p);
        }
        #endregion

        #region ** 监测SQL语句 **
        /// <summary>
        /// 最耗时的前N条T-SQL语句
        /// </summary>
        public static DataTable GetSqlStatus(string dataName, int n)
        {
            try
            {
                string sql = @"
                    
                    use " + dataName + @"
                    GO

                    Begin
                        with T0 as   
                        (     
                            select top " + n.ToString() + @" 
                                plan_handle,  
                                sum(total_worker_time) as total_worker_time ,  
                                sum(execution_count) as execution_count ,  
                                count(1) as sql_count  
                            from sys.dm_exec_query_stats group by plan_handle  
                            order by sum(total_worker_time) desc  
                        )  
                        select  T1.text as N'SQL语句',  
                                T0.total_worker_time as N'总工作时间',  
                                T0.execution_count as N'总执行次数',  
                                T0.sql_count  
                        from T0 cross apply sys.dm_exec_sql_text(plan_handle) as T1 
                        --order by total_worker_time desc
                        order by execution_count desc
                    End
                ";

                return Dbl.Command.CreateSqlDataTable(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("\n--" + DateTime.Now.ToString() + " 报错:----\n" + ex.Message + "--end--\n");
                return null;
            }
        }
        #endregion

        #region ** 数据库监测 **
        /// <summary>
        /// 查看所有数据库的连接信息
        /// </summary>
        public DataTable GetDataProcessAll()
        {
            try
            {
                string sql = @"
                    SELECT * FROM master.dbo.sysprocesses
                ";

                return Dbl.Command.CreateSqlDataTable(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("\n--" + DateTime.Now.ToString() + " GetDataProcessAll() 报错:----\n" + ex.Message + "--end--\n");
                return null;
            }
        }

        /// <summary>
        /// 查看指定数据库的连接信息
        /// </summary>
        public static DataTable GetDataProcess(string dataName)
        {
            try
            {
                string sql = @"
                    SELECT * FROM master.dbo.sysprocesses WHERE DB_NAME(dbid) = @DataName
                ";

                DbParameters p = new DbParameters();
                p.Add("@DataName", dataName);

                return Dbl.Command.CreateSqlDataTable(sql, p);
            }
            catch (Exception ex)
            {
                throw new Exception("\n--" + DateTime.Now.ToString() + " GetDataProcess(string) 报错:----\n" + ex.Message + "--end--\n");
                return null;
            }

        }

        /// <summary>
        /// 查看数据库的锁Lock的情况
        /// </summary>
        public DataTable GetLockStatus()
        {
            try
            {
                string sql = @"
                    exec sp_lock
                ";

                return Dbl.Command.CreateSqlDataTable(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("\n--" + DateTime.Now.ToString() + " GetLockStatus()报错:----\n" + ex.Message + "--end--\n");
                return null;
            }
        }

        /// <summary>
        /// 查看进程正在执行的SQL语句
        /// </summary>
        public DataTable GetInputBuffer(int spid)
        {
            try
            {
                string sql = @"
                    DBCC InputBuffer(@SPID)
                ";

                DbParameters p = new DbParameters();
                p.Add("@SPID", spid);

                return Dbl.Command.CreateSqlDataTable(sql, p);
            }
            catch (Exception ex)
            {
                throw new Exception("\n--" + DateTime.Now.ToString() + " GetInputBuffer(int) 报错:----\n" + ex.Message + "--end--\n");
                return null;
            }
        }

        /// <summary>
        /// 查询服务器资源消耗情况、用户和进程的信息
        /// </summary>
        public static DataTable SPWho3(string spid)
        {
            try
            {
                string spName = "sp_who3";
                DbParameters p = new DbParameters();
                p.Add("@SessionID", spid);

                return Dbl.Command.CreateDataTable(spName, p);
            }
            catch (Exception ex)
            {
                throw new Exception("\n--" + DateTime.Now.ToString() + "SPWho3() 报错:----\n" + ex.Message + "--end--\n");
                return null;
            }
        }

        /// <summary>
        /// 数据库IO读写情况
        /// </summary>
        public static DataTable GetIOStatus()
        {
            try
            {
                string sql = @"
                    Begin
                        WITH DBIO AS
                        (
                          SELECT
                            DB_NAME(IVFS.database_id) AS db,
                            CASE WHEN MF.type = 1 THEN 'log' ELSE 'data' END AS file_type,
                            SUM(IVFS.num_of_bytes_read + IVFS.num_of_bytes_written) AS io,
                            SUM(IVFS.io_stall) AS io_stall
                          FROM sys.dm_io_virtual_file_stats(NULL, NULL) AS IVFS
                            JOIN sys.master_files AS MF
                              ON IVFS.database_id = MF.database_id
                              AND IVFS.file_id = MF.file_id
                          GROUP BY DB_NAME(IVFS.database_id), MF.type
                        )
                        SELECT db AS N'数据库', 
                            file_type AS N'文件类型', 
                            CAST(1. * io / (1024 * 1024) AS DECIMAL(12, 2)) AS N'总I/O读写(MB)',
                            CAST(io_stall / 1000. AS DECIMAL(12, 2)) AS N'I/O等待时间(秒)',
                            CAST(100. * io_stall / SUM(io_stall) OVER()
                               AS DECIMAL(10, 2)) AS N'I/O等待时间(百分比)',
                          ROW_NUMBER() OVER(ORDER BY io_stall DESC) AS N'I/O等待时间排序'
                        FROM DBIO
                        ORDER BY  io_stall DESC;
                    End
                ";

                return Dbl.Command.CreateSqlDataTable(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("\n--" + DateTime.Now.ToString() + " 报错:----\n" + ex.Message + "--end--\n");

                return null;
            }
        }

        /// <summary>
        /// 查看死锁 sp_who_lock 
        /// </summary>
        public static DataTable SPWhoLock()
        {
            try
            {
                string spName = "sp_who_lock";
                return Dbl.Command.CreateDataTable(spName);
            }
            catch (Exception ex)
            {
                throw new Exception("\n--" + DateTime.Now.ToString() + "SPWhoLock() 报错:----\n" + ex.Message + "--end--\n");
                return null;
            }
        }
        #endregion

    }
}
