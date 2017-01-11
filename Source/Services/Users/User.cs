﻿using System;
using System.Collections.Generic;
using System.Linq;
using Insight.Base.Common;
using Insight.Base.Common.Entity;
using Insight.Base.OAuth;
using Insight.Utils.Entity;

namespace Insight.Base.Services
{
    public class User
    {
        public Result Result = new Result();
        public Authority Authority;

        private readonly SYS_User _User;

        /// <summary>
        /// 用户是否已存在(按登录账号)
        /// </summary>
        public bool Existed
        {
            get
            {
                using (var context = new BaseEntities())
                {
                    var user = context.SYS_User.SingleOrDefault(u => u.LoginName == _User.LoginName);
                    var existed = user != null && user.ID != _User.ID;
                    if (existed) Result.AccountExists();

                    return existed;
                }
            }
        }

        /// <summary>
        /// 用户唯一ID
        /// </summary>
        public Guid ID
        {
            get { return _User.ID; }
            set { _User.ID = value; }
        }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name
        {
            get { return _User.Name; }
            set { _User.Name = value; }
        }

        /// <summary>
        /// 用户登录账号
        /// </summary>
        public string LoginName
        {
            get { return _User.LoginName; }
            set { _User.LoginName = value; }
        }

        /// <summary>
        /// 用户手机号
        /// </summary>
        public string Mobile
        {
            get { return _User.Mobile; }
            set { _User.Mobile = value; }
        }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password
        {
            get { return _User.Password; }
            set { _User.Password = value; }
        }

        /// <summary>
        /// 支付密码
        /// </summary>
        public string PayPassword
        {
            get { return _User.PayPassword; }
            set { _User.PayPassword = value; }
        }

        /// <summary>
        /// 用户描述
        /// </summary>
        public string Description
        {
            get { return _User.Description; }
            set { _User.Description = value; }
        }

        /// <summary>
        /// 用户类型
        /// </summary>
        public int Type
        {
            get { return _User.Type; }
            set { _User.Type = value; }
        }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Validity
        {
            get { return _User.Validity; }
            set { _User.Validity = value; }
        }

        /// <summary>
        /// 是否内置用户
        /// </summary>
        public bool BuiltIn
        {
            get { return _User.BuiltIn; }
            set { _User.BuiltIn = value; }
        }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public Guid CreatorUserId
        {
            get { return _User.CreatorUserId; }
            set { _User.CreatorUserId = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return _User.CreateTime; }
            set { _User.CreateTime = value; }
        }

        /// <summary>
        /// 授予用户的功能权限
        /// </summary>
        public List<RoleAction> Actions => Authority?.GetActions();

        /// <summary>
        /// 授予用户的数据权限
        /// </summary>
        public List<RoleData> Datas => Authority?.GetDatas();

        /// <summary>
        /// 构造函数，构造新的用户实体
        /// </summary>
        public User()
        {
            _User = new SYS_User();
            Result.Success();
        }

        /// <summary>
        /// 构造函数，根据ID读取用户实体
        /// </summary>
        /// <param name="id">用户ID</param>
        public User(Guid id)
        {
            using (var context = new BaseEntities())
            {
                _User = context.SYS_User.SingleOrDefault(u => u.ID == id);
                if (_User == null)
                {
                    _User = new SYS_User();
                    Result.NotFound();
                }
                else
                {
                    Result.Success();
                }
            }
        }

        /// <summary>
        /// 构造函数，根据登录账号读取用户实体
        /// </summary>
        /// <param name="account">登录账号</param>
        public User(string account)
        {
            using (var context = new BaseEntities())
            {
                _User = context.SYS_User.SingleOrDefault(u => u.LoginName == account);
                if (_User == null)
                {
                    _User = new SYS_User();
                    Result.NotFound();
                }
                else Result.Success();
            }
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <returns>bool 是否成功</returns>
        public bool Add()
        {
            var result = DbHelper.Insert(_User);
            if (result)
                Result.Created();
            else
                Result.DataBaseError();

            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns>bool 是否成功</returns>
        public bool Delete()
        {
            var result = DbHelper.Delete(_User);
            if (!result) Result.DataBaseError();

            return result;
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <returns>bool 是否成功</returns>
        public bool Update()
        {
            var result = DbHelper.Update(_User);
            if (!result) Result.DataBaseError();

            return result;
        }
    }
}