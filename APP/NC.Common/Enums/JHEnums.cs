#region --Copyright (C) 2017 wyd  邮箱：604401109@qq.com--
/* --*
* 作者：wyd
* 时间：2017-12-11 星期一 11:47:00
* 版本：v1.0.0
* GUID: a2a0e614-5ddc-4a6a-99fd-5d363c3c30ec 
* 备注：
* Powered by wyd
* --*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace NC.Common
{
    public class JHEnums
    {

        /// <summary>
        /// 统一管理操作枚举
        /// </summary>
        public enum ActionEnum
        {
            /// <summary>
            /// 所有
            /// </summary>
            All,
            /// <summary>
            /// 显示
            /// </summary>
            Show,
            /// <summary>
            /// 查看
            /// </summary>
            View,
            /// <summary>
            /// 添加
            /// </summary>
            Add,
            /// <summary>
            /// 修改
            /// </summary>
            Edit,
            /// <summary>
            /// 删除
            /// </summary>
            Delete,
            /// <summary>
            /// 审核
            /// </summary>
            Audit,
            /// <summary>
            /// 回复
            /// </summary>
            Reply,
            /// <summary>
            /// 确认
            /// </summary>
            Confirm,
            /// <summary>
            /// 取消
            /// </summary>
            Cancel,
            /// <summary>
            /// 作废
            /// </summary>
            Invalid,
            /// <summary>
            /// 生成
            /// </summary>
            Build,
            /// <summary>
            /// 安装
            /// </summary>
            Instal,
            /// <summary>
            /// 卸载
            /// </summary>
            UnLoad,
            /// <summary>
            /// 登录
            /// </summary>
            Login,
            /// <summary>
            /// 备份
            /// </summary>
            Back,
            /// <summary>
            /// 还原
            /// </summary>
            Restore,
            /// <summary>
            /// 替换
            /// </summary>
            Replace,
            /// <summary>
            /// 复制
            /// </summary>
            Copy
        }

        /// <summary>
        /// 系统导航菜单类别枚举
        /// </summary>
        public enum NavigationEnum
        {
            /// <summary>
            /// 系统后台菜单
            /// </summary>
            System,
            /// <summary>
            /// 会员中心导航
            /// </summary>
            Users,
            /// <summary>
            /// 网站主导航
            /// </summary>
            WebSite
        }

        /// <summary>
        /// 用户生成码枚举
        /// </summary>
        public enum CodeEnum
        {
            /// <summary>
            /// 注册验证
            /// </summary>
            RegVerify,
            /// <summary>
            /// 邀请注册
            /// </summary>
            Register,
            /// <summary>
            /// 取回密码
            /// </summary>
            Password
        }

        /// <summary>
        /// 金额类型枚举
        /// </summary>
        public enum AmountTypeEnum
        {
            /// <summary>
            /// 系统赠送
            /// </summary>
            SysGive,
            /// <summary>
            /// 在线充值
            /// </summary>
            Recharge,
            /// <summary>
            /// 用户消费
            /// </summary>
            Consumption,
            /// <summary>
            /// 购买商品
            /// </summary>
            BuyGoods,
            /// <summary>
            /// 积分兑换
            /// </summary>
            Convert
        }
        /// <summary>
        /// 文章状态
        /// </summary>
        public enum ArticleStatus
        {
            /// <summary>
            /// 通过
            /// </summary>
            Pass = 0,
            /// <summary>
            /// 草稿
            /// </summary>
            Draft = -3,
            /// <summary>
            /// 待审核
            /// </summary>
            PendingAudit = -2,
            /// <summary>
            /// 草稿
            /// </summary>
            NotPass = -1
        }
        /// <summary>
        /// 返回json结果状态
        /// </summary>
        public enum ResultStatus
        {
            /// <summary>
            /// 正常
            /// </summary>
            OK = 100,
            /// <summary>
            /// 失败
            /// </summary>
            Failed = 101,
            /// <summary>
            /// 未登录
            /// </summary>
            NotLogin = 102,
            /// <summary>
            /// 未经授权的; 未经许可的; 未经批准的;
            /// </summary>
            Unauthorized = 103,
        }
    }
}
