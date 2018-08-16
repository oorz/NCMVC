/* --*
* 作者：wyd
* 时间：2017-11-15 星期三 10:46:34
* 版本：v1.0.0
* GUID: 161c0662-d7c9-4332-b448-1842084d7198 
* 备注：
* Powered by wyd
* --*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NC.Core
{
    public class Dbl
    {
        #region ** 属性 ** 
        /// <summary>
        /// 默认命令对象属性 : Dbl.Command.CreateSqlDataTable(sql, p); 
        /// </summary>
        public static DbCommand Command
        {
            get { return new DbCommand(); }
        }
        //自定义实例 : Dbl.DsnUsr.CreateSqlDataTable(sql, p); 
        public static DbCommand ZZCMS { get { return new DbCommand("ZZCMS"); } }
        #endregion

        #region ** 生成 Command 命令对象 ** 
        /// <summary>
        /// 生成数据库操作命令对象
        /// </summary>
        public static DbCommand CreateCommand()
        {
            return new DbCommand();
        }
        public static DbCommand CreateCommand(string dsn)
        {
            return new DbCommand(dsn);
        }
        /// <summary>
        /// CreateComand的另一种表达方式DSN
        /// </summary>
        /// <param name="dsn"></param>
        /// <returns></returns>
        public static DbCommand DSN(string dsn)
        {
            return new DbCommand(dsn);
        }
        #endregion

        #region ** 常用函数 **
        /// <summary>
        /// 创建新表格 
        /// </summary>
        public static DataTable CreateNewTable(params string[] cols)
        {
            DataTable dt = new DataTable();

            for (int i = 0; i < cols.Length; i++)
            {
                dt.Columns.Add(cols[i]);
            }
            dt.Clear();

            return dt;
        }
        /// <summary>
        /// 修改DataTable的某行数据
        /// </summary>
        public static DataTable UpdateTable(DataTable dt, int rowNum, string colName, string val)
        {
            //需要修改的行            
            DataRow dr = dt.Rows[rowNum];

            dr.BeginEdit();
            dr[colName] = val;   //对需要修改的列赋值
            dr.EndEdit();
            dt.AcceptChanges();

            return dt;

        }

        /// <summary>
        /// 向DataTable增加一行数据
        /// </summary>
        public static DataTable InsertTable(DataTable dt, string[] flds, string[] values)
        {
            //新行            
            //DataRow dr = dt.NewRow();
            //dr[colName] = val;        

            dt.AcceptChanges();

            return dt;
        }

        /// <summary>
        /// 获取新DataTable 
        /// </summary>
        public static DataTable GetNewTable(DataTable dt, string condition)
        {
            if (dt == null) return null;
            DataTable newdt = new DataTable();
            newdt = dt.Clone();
            DataRow[] dr = dt.Select(condition);
            for (int i = 0; i < dr.Length; i++)
            {
                newdt.ImportRow(dr[i]);
            }

            return newdt;
        }

        /// <summary> 
        /// 获取总页数 
        /// </summary> 
        public static int GetPageCount(int pSize, int fldCount)
        {
            int iCount = 0;

            if (fldCount % pSize == 0)
            {
                iCount = (fldCount / pSize);
            }
            else
            {
                iCount = (int)(fldCount / pSize) + 1;
            }

            return iCount;
        }
        #endregion
    }
}
