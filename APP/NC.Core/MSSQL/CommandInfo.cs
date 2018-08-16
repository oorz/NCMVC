/* --*
* 作者：wyd
* 时间：2017-11-15 星期三 9:25:15
* 版本：v1.0.0
* GUID: 5949caae-6830-4ce8-86b8-8c3b4a5fa2fb 
* 备注：
* Powered by wyd
* --*/
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace NC.Core
{
    public class CommandInfo
    {
        public object ShareObject = null;
        public object OriginalData = null;
        event EventHandler _solicitationEvent;
        public event EventHandler SolicitationEvent
        {
            add
            {
                _solicitationEvent += value;
            }
            remove
            {
                _solicitationEvent -= value;
            }
        }
        public void OnSolicitationEvent()
        {
            if (_solicitationEvent != null)
            {
                _solicitationEvent(this, new EventArgs());
            }
        }
        public string CommandText;
        public DbParameters Parameters;
        public EffentNextType EffentNextType = EffentNextType.None;
        public CommandInfo()
        {

        }
        public CommandInfo(string sqlText, DbParameters para)
        {
            this.CommandText = sqlText;
            this.Parameters = para;
        }
        public CommandInfo(string sqlText, DbParameters para, EffentNextType type)
        {
            this.CommandText = sqlText;
            this.Parameters = para;
            this.EffentNextType = type;
        }



        public System.Data.Common.DbParameter[] Parametersby;
        /// <summary>
        /// 事务下表
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="para"></param>
        public CommandInfo(string sqlText, SqlParameter[] para)
        {
            this.CommandText = sqlText;
            this.Parametersby = para;
        }
    }
    public enum EffentNextType
    {
        /// <summary>
        /// 对其他语句无任何影响 
        /// </summary>
        None,
        /// <summary>
        /// 当前语句必须为"select count(1) from .."格式，如果存在则继续执行，不存在回滚事务
        /// </summary>
        WhenHaveContine,
        /// <summary>
        /// 当前语句必须为"select count(1) from .."格式，如果不存在则继续执行，存在回滚事务
        /// </summary>
        WhenNoHaveContine,
        /// <summary>
        /// 当前语句影响到的行数必须大于0，否则回滚事务
        /// </summary>
        ExcuteEffectRows,
        /// <summary>
        /// 引发事件-当前语句必须为"select count(1) from .."格式，如果不存在则继续执行，存在回滚事务
        /// </summary>
        SolicitationEvent
    }
}
