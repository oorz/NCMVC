#region --Copyright (C) 2017 wyd  邮箱：604401109@qq.com--
/* --*
* 作者：wyd
* 时间：2017-11-02 星期四 10:45:02
* 版本：v1.0.0
* GUID: 8bd50800-d834-4459-bbee-489050b498eb 
* 备注：数据库操作类
* Powered by wyd
* --*/
#endregion

using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Collections;
using System.Reflection;

using NC.Common;
namespace NC.Core
{
    public class DbHelper
    {
        public static ILogger Log = UtilLogger<DbHelper>.Log;//日志记录

        #region --定义变量--
        public string dsn;
        //默认实例 ： DbCommand.SqlDSN.CraeteSqlDataTable(sql, p); 
        public static DbHelper SqlDSN { get { return new DbHelper(); } }

        #endregion

        #region --构造函数--
        /// <summary>
        /// 构造函数
        /// </summary>
        public DbHelper()
        {
            //dsn = Encrypt.Dec(dsn);  //解密 
            //dsn = Configuration.GetConnectionString("SqlDSN");
            dsn = UtilConf.GetConnectionString("SqlDSN");
        }
        /// <summary>
        /// 多数据库
        /// </summary>
        /// <param name="strDSN"></param>
        public DbHelper(string strDSN)
        {
            Log.LogInformation(strDSN);
            //dsn = Configuration.GetConnectionString(strDSN);
            dsn = UtilConf.GetConnectionString(strDSN);
        }
        #endregion

        #region ** 打开/关闭链接 **
        /// <summary>
        /// 打开链接
        /// </summary>
        private void ConnOpen(ref SqlCommand comd)
        {
            if (comd.Connection.State == ConnectionState.Closed)
                comd.Connection.Open();
        }

        /// <summary>
        /// 关闭链接
        /// </summary>
        private void ConnClose(ref SqlCommand comd)
        {
            if (comd.Connection.State == ConnectionState.Open)
            {
                comd.Connection.Close();
            }
            comd.Dispose();
        }
        #endregion

        #region ** 创建 SqlCommand 对象
        /// <summary>
        /// 生成comd对象
        /// </summary>
        public SqlCommand CreateComd(string spName)
        {
            try
            {
                SqlConnection conn = new SqlConnection(dsn);
                SqlCommand comd = conn.CreateCommand();
                comd.CommandText = spName;
                comd.CommandType = CommandType.StoredProcedure;

                return comd;
            }
            catch (System.Exception ex)
            {
                Log.LogError("DbCommand->CreateComd(sp) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public SqlCommand CreateComd(string spName, DbParameters p)
        {
            try
            {
                SqlCommand comd = CreateComd(spName);

                int len = p.Length;
                if (len > 0)
                {
                    for (int i = 0; i < len; i++)
                    {
                        comd.Parameters.Add(p[i]);
                    }
                }
                return comd;
            }
            catch (System.Exception ex)
            {
                Log.LogError("DbCommand->CreateComd(sp) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public SqlCommand CreateSqlComd(string strSql)
        {
            try
            {
                SqlConnection conn = new SqlConnection(dsn);
                SqlCommand comd = conn.CreateCommand();
                comd.CommandText = strSql;
                comd.CommandType = CommandType.Text;

                return comd;
            }
            catch (System.Exception ex)
            {
                Log.LogError("DbCommand->CreateSqlComd(s) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public SqlCommand CreateSqlComd(string strSql, DbParameters p)
        {
            try
            {
                SqlCommand comd = CreateSqlComd(strSql);

                int len = p.Length;
                if (len > 0)
                {
                    for (int i = 0; i < len; i++)
                    {
                        comd.Parameters.Add(p[i]);
                    }
                }
                return comd;
            }
            catch (System.Exception ex)
            {
                Log.LogError("DbCommand->CreateSqlcomd(s,p) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region ** 创建 SqlDataAdapter 对象
        /// <summary>
        /// 根据存储过程名，生成SqlDataAdapter对象
        /// </summary>
        public SqlDataAdapter CreateAdapter(string spName)
        {
            try
            {
                SqlConnection conn = new SqlConnection(dsn);
                SqlDataAdapter comdAdapter = new SqlDataAdapter(spName, conn);
                comdAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                return comdAdapter;
            }
            catch (System.Exception ex)
            {
                Log.LogError("DbCommand->CreateAdapter(s) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 根据存储过程名和参数，生成SqlDataAdapter对象
        /// </summary>
        public SqlDataAdapter CreateAdapter(string spName, DbParameters p)
        {
            try
            {
                SqlDataAdapter comdAdapter = CreateAdapter(spName);

                int len = p.Length;
                if (len > 0)
                {
                    for (int i = 0; i < len; i++)
                    {
                        comdAdapter.SelectCommand.Parameters.Add(p[i]);
                    }
                }

                return comdAdapter;
            }
            catch (System.Exception ex)
            {
                Log.LogError("DbCommand->CreateAdapter(s, p) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 根据SQL语句,生成DataAdapter对象
        /// </summary>
        public SqlDataAdapter CreateSqlAdapter(string strSql)
        {
            try
            {
                SqlConnection conn = new SqlConnection(dsn);
                SqlDataAdapter apter = new SqlDataAdapter(strSql, conn);
                apter.SelectCommand.CommandType = CommandType.Text;

                return apter;
            }
            catch (System.Exception ex)
            {
                Log.LogError("DbCommand->CreateSqlAdapter(s) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 根据SQL语句和参数,生成DataAdapter对象
        /// </summary>
        public SqlDataAdapter CreateSqlAdapter(string strSql, DbParameters p)
        {
            try
            {
                SqlDataAdapter apter = CreateSqlAdapter(strSql);

                int len = p.Length;
                if (len > 0)
                {
                    for (int i = 0; i < len; i++)
                    {
                        apter.SelectCommand.Parameters.Add(p[i]);
                    }
                }

                return apter;
            }
            catch (System.Exception ex)
            {
                Log.LogError("DbCommand->CreateSqlAdapter(s,p) 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region ** 创建 DataReader 对象
        /// <summary>
        /// 根据存储过程生成生SqlDataReader
        /// </summary>
        public SqlDataReader CreateDataReader(string spName)
        {
            SqlCommand comd = CreateComd(spName);
            return GetDataReader(comd);
        }
        /// <summary>
        /// 根据存储过程和参数生成SqlDataReader
        /// </summary>
        public SqlDataReader CreateDataReader(string spName, DbParameters p)
        {
            SqlCommand comd = CreateComd(spName, p);
            return GetDataReader(comd);
        }
        /// <summary>
        /// 根据SQL语句生成SqlDataReader
        /// </summary>
        public SqlDataReader CreateSqlDataReader(string strSql)
        {
            SqlCommand comd = CreateSqlComd(strSql);
            return GetDataReader(comd);
        }
        /// <summary>
        /// 根据SQL语句和参数生成SqlDataReader
        /// </summary>
        public SqlDataReader CreateSqlDataReader(string strSql, DbParameters p)
        {
            SqlCommand comd = CreateSqlComd(strSql, p);
            return GetDataReader(comd);
        }

        #region - GetDataReader()
        //获取DataReader
        private SqlDataReader GetDataReader(SqlCommand comd)
        {
            try
            {
                ConnOpen(ref comd);
                return comd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                Log.LogError("DbCommand->GetDataReader() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #endregion


        #region ** 创建 DataTable 对象
        /// <summary>
        /// 根据存储过程创建 DataTable 
        /// </summary>
        public DataTable CreateDataTable(string spName)
        {
            SqlDataAdapter adapter = CreateAdapter(spName);
            return GetDataTable(adapter);
        }
        /// <summary>
        /// 根据存储过程和参数创建 DataTable 
        /// </summary>
        public DataTable CreateDataTable(string spName, DbParameters p)
        {
            SqlDataAdapter adapter = CreateAdapter(spName, p);
            return GetDataTable(adapter);
        }
        /// <summary>
        /// 根据SQL语句,创建DataTable
        /// </summary>
        public DataTable CreateSqlDataTable(string strSql)
        {
            SqlDataAdapter adapter = CreateSqlAdapter(strSql);
            return GetDataTable(adapter);
        }
        /// <summary>
        /// 根据SQL语句和参数,创建DataTable
        /// </summary>
        public DataTable CreateSqlDataTable(string strSql, DbParameters p)
        {
            SqlDataAdapter adapter = CreateSqlAdapter(strSql, p);
            return GetDataTable(adapter);
        }

        #region  - GetDataTable()
        private DataTable GetDataTable(SqlDataAdapter adapter)
        {
            try
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
            catch (System.Exception ex)
            {
                Log.LogError("DbCommand->GetSqlDataTable() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
            finally
            {
                if (adapter.SelectCommand.Connection.State == ConnectionState.Open)
                {
                    adapter.SelectCommand.Connection.Close();
                }
                adapter.Dispose();
            }
        }
        #endregion

        #endregion

        #region ** 创建 Scalar 对象
        /// <summary>
        /// 创建无参数的 Scalar 对象
        /// </summary>
        public object CreateScalar(string spName)
        {
            SqlCommand comd = CreateComd(spName);
            return GetScalar(comd);
        }
        /// <summary>
        /// 有参数的 Scalar 对象
        /// </summary>
        public object CreateScalar(string spName, DbParameters p)
        {
            SqlCommand comd = CreateComd(spName, p);
            return GetScalar(comd);
        }
        /// <summary>
        /// 根据SQL语句，创建Scalar对象
        /// </summary>
        public object CreateSqlScalar(string strSql)
        {
            SqlCommand comd = CreateSqlComd(strSql);
            return GetScalar(comd);
        }
        /// <summary>
        /// 根据SQL语句和参数，创建Scalar对象
        /// </summary>
        public object CreateSqlScalar(string strSql, DbParameters p)
        {
            SqlCommand comd = CreateSqlComd(strSql, p);
            return GetScalar(comd);
        }

        #region - GetScalar()
        private object GetScalar(SqlCommand comd)
        {
            try
            {
                ConnOpen(ref comd);
                object o = comd.ExecuteScalar();
                ConnClose(ref comd);

                return o;
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                Log.LogError("DbCommand->GetScalar() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        #endregion
        #endregion

        #region ** 执行数据库操作 - ToExecute() **
        /// <summary>
        /// 执行数据库操作
        /// </summary>
        private int ToExecute(SqlCommand comd)
        {
            try
            {
                ConnOpen(ref comd);
                int iOk = comd.ExecuteNonQuery();
                ConnClose(ref comd);
                return iOk;
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                Log.LogError("DbCommand->ToExecute() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private int ToExecuteInt(SqlCommand comd)
        {
            try
            {
                ConnOpen(ref comd);
                int iOk = 0;
                int.TryParse(comd.ExecuteScalar().ToString(), out iOk);
                ConnClose(ref comd);
                return iOk;
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                Log.LogError("DbCommand->ToExecute() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region ** 仅执行，不返回输出参数 **
        /// <summary>
        /// 根据存储过程执行
        /// </summary>
        public int Execute(string spName)
        {
            SqlCommand comd = CreateComd(spName);
            return ToExecute(comd);
        }
        /// <summary>
        /// 根据存储过程和参数执行
        /// </summary>
        public int Execute(string spName, DbParameters p)
        {
            SqlCommand comd = CreateComd(spName, p);
            return ToExecute(comd);
        }
        /// <summary> 
        /// 执行sql语句
        /// </summary> 
        public int ExecuteSql(string sql)
        {
            SqlCommand comd = CreateSqlComd(sql);
            return ToExecute(comd);
        }

        /// <summary> 
        /// 执行带参数的SQL语句
        /// </summary> 
        public int ExecuteSqlInt(string sql, DbParameters p)
        {
            SqlCommand comd = CreateSqlComd(sql, p);
            return ToExecuteInt(comd);
        }
        public int ExecuteSql(string sql, DbParameters p)
        {
            SqlCommand comd = CreateSqlComd(sql, p);
            return ToExecute(comd);
        }

        #endregion

        #region ** 执行并返回输出参数 **
        /// <summary>
        /// 执行并返回输出参数
        /// </summary>
        public string ExecuteOut(string spName, DbParameters p, string outParamName)
        {
            SqlCommand comd = CreateComd(spName, p);
            //comd.Parameters.Add(new SqlParameter(outParamName, SqlDbType.VarChar, 50));
            //comd.Parameters[outParamName].Direction = ParameterDirection.Output;

            try
            {
                ConnOpen(ref comd);
                comd.ExecuteNonQuery();
                object o = comd.Parameters[outParamName].Value;
                ConnClose(ref comd);

                return (o == null) ? "" : o.ToString();
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                Log.LogError("DbCommand->ExecuteOut() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 执行并返回输出参数：默认输出参数 @Result Varchar(50)
        /// </summary>
        public string ExecuteOut(string spName, DbParameters p)
        {
            SqlCommand comd = CreateComd(spName, p);
            comd.Parameters.Add(new SqlParameter("@Result", SqlDbType.VarChar, 50));
            comd.Parameters["@Result"].Direction = ParameterDirection.Output;

            try
            {
                ConnOpen(ref comd);
                comd.ExecuteNonQuery();
                object o = comd.Parameters["@Result"].Value;
                ConnClose(ref comd);

                return (o == null) ? "" : o.ToString();
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                Log.LogError("DbCommand->ExecuteOut() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region ** 执行并返回输出参数 **
        /// <summary>
        /// 执行存储过程，并返回输出参数
        /// </summary>
        public string ExecuteReturn(string spName, DbParameters p, string retParam)
        {
            SqlCommand comd = CreateComd(spName, p);
            comd.Parameters.Add(new SqlParameter(retParam, SqlDbType.VarChar, 50));
            comd.Parameters[retParam].Direction = ParameterDirection.ReturnValue;

            //comd.Parameters.Add(new SqlParameter("ReturnValue",SqlDbType.Int,4, ParameterDirection.ReturnValue, false, 0, 0,String.Empty, DataRowVersion.Default, null));

            try
            {
                ConnOpen(ref comd);
                comd.ExecuteNonQuery();
                object o = comd.Parameters[retParam].Value;
                ConnClose(ref comd);

                return (o == null) ? "" : o.ToString();
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                Log.LogError("DbCommand->ExecuteReturn() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public string ExecuteReturn(string spName, DbParameters p)
        {
            SqlCommand comd = CreateComd(spName, p);
            comd.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.VarChar, 50));
            comd.Parameters["ReturnValue"].Direction = ParameterDirection.ReturnValue;

            //comd.Parameters.Add(new SqlParameter("ReturnValue",SqlDbType.Int,4, ParameterDirection.ReturnValue, false, 0, 0,String.Empty, DataRowVersion.Default, null));

            try
            {
                ConnOpen(ref comd);
                comd.ExecuteNonQuery();
                object o = comd.Parameters["ReturnValue"].Value;
                ConnClose(ref comd);

                return (o == null) ? "" : o.ToString();
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                Log.LogError("DbCommand->ExecuteReturn() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        /// <summary> 
        /// 执行Sql语句，并返回返回值
        /// </summary> 
        public string ExecuteSqlReturn(string sql, DbParameters p, string retParam)
        {
            SqlCommand comd = CreateSqlComd(sql, p);
            comd.Parameters.Add(new SqlParameter(retParam, SqlDbType.VarChar, 50));
            comd.Parameters[retParam].Direction = ParameterDirection.ReturnValue;

            //comd.Parameters.Add(new SqlParameter("ReturnValue",SqlDbType.Int,4, ParameterDirection.ReturnValue, false, 0, 0,String.Empty, DataRowVersion.Default, null));

            try
            {
                ConnOpen(ref comd);
                comd.ExecuteNonQuery();
                object o = comd.Parameters[retParam].Value;
                ConnClose(ref comd);

                return (o == null) ? "" : o.ToString();
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                Log.LogError("DbCommand->ExecuteReturn() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 根据Sql语句执行
        /// </summary>
        public string ExecuteSqlReturn(string sql, DbParameters p)
        {
            SqlCommand comd = CreateSqlComd(sql, p);
            comd.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.VarChar, 50));
            comd.Parameters["ReturnValue"].Direction = ParameterDirection.ReturnValue;

            //comd.Parameters.Add(new SqlParameter("ReturnValue",SqlDbType.Int,4, ParameterDirection.ReturnValue, false, 0, 0,String.Empty, DataRowVersion.Default, null));

            try
            {
                ConnOpen(ref comd);
                comd.ExecuteNonQuery();
                object o = comd.Parameters["ReturnValue"].Value;
                ConnClose(ref comd);

                return (o == null) ? "" : o.ToString();
            }
            catch (System.Exception ex)
            {
                ConnClose(ref comd);
                Log.LogError("DbCommand->ExecuteReturn() 出错\r\n" + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        #endregion

    }
}
