using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace NC.Lib
{
    /// <summary>
    /// nav_name
    /// </summary>
    public class NavAttr : Attribute, IFilterMetadata
    {
        public NavAttr() { }
        public NavAttr(string navName, string actionType)
        {
            this.NavName = navName;
            this.ActionType = actionType;
        }
        public string NavName { set; get; }//菜单名称
        public string ActionType { set; get; }  //操作类型
    }
}
