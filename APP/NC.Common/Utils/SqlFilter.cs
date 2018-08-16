using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace NC.Common
{
    public class SqlFilter
    {
        //当前请求对象
        private HttpRequest request;
        //当前响应对象
        private HttpResponse response;
        //安全Url,当出现Sql注入时,将导向到的安全页面,如果没赋值,则停留在当前页面
        private string safeUrl = String.Empty;

        //Sql注入时,可能出现的sql关键字,可根据自己的实际情况进行初始化,每个关键字由'|'分隔开来
        //private const string StrKeyWord = @"select|insert|delete|from|count(|drop table|update|truncate|asc(|mid(|char(|xp_cmdshell|exec master|netlocalgroup administrators|:|net user|""|or|and";
        private const string StrKeyWord = @"select|insert|delete|from|drop table|update|truncate|exec master|netlocalgroup administrators|:|net user|or|and";
        //Sql注入时,可能出现的特殊符号,,可根据自己的实际情况进行初始化,每个符号由'|'分隔开来
        //private const string StrRegex = @"-|;|,|/|(|)|[|]|}|{|%|@|*|!|'";
        private const string StrRegex = @"=|!|'";
        public SqlFilter()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        /// <summary>
        /// 由此构造函数创建的对象,在验证Sql注入之后将停留在原来页面上
        /// </summary>
        /// <param name="_request">当前请求的 Request 对象</param>
        /// <param name="_response">当前请求的 Response 对象</param>
        public SqlFilter(HttpRequest _request, HttpResponse _response)
        {
            this.request = _request;
            this.response = _response;
        }
        /// <summary>
        /// 由此构造函数创建的对象,在验证Sql注入之后将请求将导向由 _safeUrl 指定的安全url页面上
        /// </summary>
        /// <param name="_request">当前请求的 Request 对象</param>
        /// <param name="_response">当前请求的 Response 对象</param>
        /// <param name="_safeUrl">验证Sql注入之后将导向的安全 url</param>
        public SqlFilter(HttpRequest _request, HttpResponse _response, string _safeUrl)
        {
            this.request = _request;
            this.response = _response;
            this.safeUrl = _safeUrl;
        }
        /// <summary>
        /// 只读属性 SQL关键字
        /// </summary>
        public string KeyWord
        {
            get
            {
                return StrKeyWord;
            }
        }
        /// <summary>
        /// 只读属性过滤特殊字符
        /// </summary>
        public string RegexString
        {
            get
            {
                return StrRegex;
            }
        }
        /// <summary>
        /// 当出现Sql注入时需要提示的错误信息(主要是运行一些客户端的脚本)
        /// </summary>
        public string Msg
        {
            get
            {
                string msg = "<script type='text/javascript'> "
                + " alert('请勿输入非法字符!'); ";

                if (this.safeUrl == String.Empty)
                    //msg += " window.location.href = '" + request.RawUrl + "'";
                    msg += "";
                else
                    msg += " window.location.href = '" + safeUrl + "'";

                msg += "</script>";
                return msg;
            }
        }
        /// <summary>
        /// 检查URL参数中是否带有SQL注入的可能关键字。
        /// </summary>
        /// <returns>存在SQL注入关键字时返回 true，否则返回 false</returns>
        public bool CheckRequestQuery()
        {
            bool result = false;
            if (request.Query.Count != 0)
            {
                //若URL中参数存在，则逐个检验参数。
                foreach (var queryName in this.request.Query)
                {
                    //过虑一些特殊的请求状态值,主要是一些有关页面视图状态的参数
                    if (queryName.Value == "__VIEWSTATE" || queryName.Value == "__EVENTVALIDATION")
                        continue;
                    //开始检查请求参数值是否合法
                    if (CheckKeyWord(request.Query[queryName.Value]))
                    {
                        //只要存在一个可能出现Sql注入的参数,则直接退出
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 检查提交表单中是否存在SQL注入的可能关键字
        /// </summary>
        /// <returns>存在SQL注入关键字时返回 true，否则返回 false</returns>
        public bool CheckRequestForm()
        {
            bool result = false;
            if (request.Form.Count > 0)
            {
                //若获取提交的表单项个数不为0,则逐个比较参数
                foreach (var queryName in this.request.Form)
                {
                    //过虑一些特殊的请求状态值,主要是一些有关页面视图状态的参数
                    if (queryName.Value == "__VIEWSTATE" || queryName.Value == "__EVENTVALIDATION")
                        continue;
                    //开始检查提交的表单参数值是否合法
                    if (CheckKeyWord(request.Form[queryName.Value]))
                    {
                        //只要存在一个可能出现Sql注入的参数,则直接退出
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 检查_sword是否包涵SQL关键字
        /// </summary>
        /// <param name="_sWord">需要检查的字符串</param>
        /// <returns>存在SQL注入关键字时返回 true，否则返回 false</returns>
        public bool CheckKeyWord(string _sWord)
        {
            bool result = false;
            //模式1 : 对应Sql注入的可能关键字
            string[] patten1 = StrKeyWord.Split('|');
            //模式2 : 对应Sql注入的可能特殊符号
            string[] patten2 = StrRegex.Split('|');
            //开始检查 模式1:Sql注入的可能关键字 的注入情况
            foreach (string sqlKey in patten1)
            {
                if (_sWord.IndexOf(" " + sqlKey) >= 0 || _sWord.IndexOf(sqlKey + " ") >= 0)
                {
                    //只要存在一个可能出现Sql注入的参数,则直接退出
                    result = true;
                    break;
                }
            }
            //开始检查 模式1:Sql注入的可能特殊符号 的注入情况
            foreach (string sqlKey in patten2)
            {
                if (_sWord.IndexOf(sqlKey) >= 0)
                {
                    //只要存在一个可能出现Sql注入的参数,则直接退出
                    result = true;
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// 执行Sql注入验证
        /// </summary>
        public void Check()
        {
            if (CheckRequestQuery() || CheckRequestForm())
            {
                //response.Write(Msg);
                //response.End();
            }
        }
    }
}
