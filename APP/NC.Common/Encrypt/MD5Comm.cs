#region --Copyright (C) 2017 wyd  邮箱：604401109@qq.com--
/* --*
* 作者：wyd
* 时间：2017-11-24 星期五 14:40:21
* 版本：v1.0.0
* GUID: 4704e96e-c599-49fc-8665-b60238e02dae 
* 备注：
* Powered by wyd
* --*/
#endregion

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NC.Common
{
    /// <summary>
    /// 此类获取md5加密值均为大写，如果要获取小写：MD5Comm.Get32MD5One(xx).ToLower();或完善此类。
    /// </summary>
    public class MD5Comm
    {
        #region --Expired code--
        ///// <summary>
        ///// MD5加密
        ///// </summary>
        //public string Md5(string txt)
        //{
        //    //return FormsAuthentication.HashPasswordForStoringInConfigFile(txt, "MD5");//此方法nf4.5后不再支持
        //}
        //public string Md5Pass(string pwd)
        //{
        //    return Md5(pwd + "Jiahao");
        //} 
        #endregion

        /// <summary>
        /// 此代码示例通过创建哈希字符串适用于任何 MD5 哈希函数 （在任何平台） 上创建 32 个字符的十六进制格式哈希字符串
        /// 官网案例改编
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Get32MD5One(string source)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                string hash = sBuilder.ToString();
                return hash.ToUpper();
            }
        }
        /// <summary>
        /// 获取16位md5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Get16MD5One(string source)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
                //转换成字符串，并取9到25位
                string sBuilder = BitConverter.ToString(data, 4, 8);
                //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉
                sBuilder = sBuilder.Replace("-", "");
                return sBuilder.ToString().ToUpper();
            }
        }
        //// <summary>
        /// </summary>
        /// <param name="strSource">需要加密的明文</param>
        /// <returns>返回16位加密结果，该结果取32位加密结果的第9位到25位</returns>
        public static string Get32MD5Two(string source)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            //获取密文字节数组
            byte[] bytResult = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(source));
            //转换成字符串，32位
            string strResult = BitConverter.ToString(bytResult);
            //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉
            strResult = strResult.Replace("-", "");
            return strResult.ToUpper();
        }
        //// <summary>
        /// </summary>
        /// <param name="strSource">需要加密的明文</param>
        /// <returns>返回16位加密结果，该结果取32位加密结果的第9位到25位</returns>
        public static string Get16MD5Two(string source)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            //获取密文字节数组
            byte[] bytResult = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(source));
            //转换成字符串，并取9到25位
            string strResult = BitConverter.ToString(bytResult, 4, 8);
            //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉
            strResult = strResult.Replace("-", "");
            return strResult.ToUpper();
        }
        /// <summary>
        /// 自定义MD5函数,32位,从老版本迁移过来，后续淘汰不用。与Get32MD5One重复
        /// </summary>
        public static string Get32MD5Old(string str)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');

            return ret.ToUpper();
        }
    }
}
