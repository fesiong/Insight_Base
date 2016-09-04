﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Insight.Base.Common.Entity;
using Insight.Utils.Common;
using Insight.Utils.Entity;

namespace Insight.Base.Common
{
    public static class TokenManage
    {
        /// <summary>
        /// Token缓存
        /// </summary>
        private static readonly List<Session> Tokens;

        /// <summary>
        /// 进程同步基元
        /// </summary>
        private static readonly Mutex Mutex = new Mutex();

        /// <summary>
        /// 构造方法，初始化Token缓存
        /// </summary>
        static TokenManage()
        {
            Tokens = new List<Session>();
        }

        /// <summary>
        /// 获取指定类型的所有在线用户
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Session> GetOnlineUsers(int type)
        {
            return Tokens.Where(s => s.UserType == type && s.OnlineStatus).ToList();
        }

        /// <summary>
        /// 根据传入的用户数据更新Token
        /// </summary>
        /// <param name="user">SYS_User</param>
        public static void Update(SYS_User user)
        {
            var session = Get(user.LoginName);
            if (session == null) return;

            session.UserName = user.Name;
            session.UserType = user.Type;
        }

        /// <summary>
        /// 设置指定账号的登录状态为离线
        /// </summary>
        /// <param name="account">登录账号</param>
        public static void Offline(string account)
        {
            var session = Get(account);
            if (session == null) return;

            session.OnlineStatus = false;
        }

        /// <summary>
        /// 设置指定账号的Validity状态
        /// </summary>
        /// <param name="account">登录账号</param>
        /// <param name="validity">bool 是否有效</param>
        /// <returns>Session</returns>
        public static void SetValidity(string account, bool validity)
        {
            var session = Get(account);
            if (session == null) return;

            session.Validity = validity;
        }

        /// <summary>
        /// 根据登录账号在缓存中查找Session并返回
        /// </summary>
        /// <param name="account">登录账号</param>
        /// <returns>Token</returns>
        public static Session Get(string account)
        {
            return Tokens.SingleOrDefault(s => Util.StringCompare(s.Account, account));
        }

        /// <summary>
        /// 根据SessionID获取缓存中的Token并返回
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Token</returns>
        public static Session Get(AccessToken token)
        {
            var fast = token.ID < Tokens.Count && Util.StringCompare(token.Account, Tokens[token.ID].Account);
            return fast ? Tokens[token.ID] : Find(token);
        }

        /// <summary>
        /// 计算Secret值
        /// </summary>
        /// <param name="signature">用户签名</param>
        /// <returns>string Secret</returns>
        public static string GetSecret(string signature)
        {
            return Util.Hash(Guid.NewGuid() + signature + DateTime.Now);
        }

        /// <summary>
        /// 在缓存中查找Token并返回
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Token</returns>
        private static Session Find(AccessToken token)
        {
            Mutex.WaitOne();
            var obj = Tokens.SingleOrDefault(s => Util.StringCompare(s.Account, token.Account)) ?? Add(token.Account);
            Mutex.ReleaseMutex();
            return obj;
        }

        /// <summary>
        /// 根据登录账号从数据库读取用户信息更新Session加入缓存并返回
        /// </summary>
        /// <param name="account">登录账号</param>
        private static Session Add(string account)
        {
            var user = DataAccess.GetUser(account);
            if (user == null) return null;

            var signature = Util.Hash(user.LoginName.ToUpper() + user.Password);
            var stamp = user.Type == 0 ? Guid.NewGuid().ToString("N") : null;
            var expired = DateTime.Now.AddHours(user.Type == 0 ? 24 : Parameters.Expired);
            var token = new Session
            {
                ID = Tokens.Count,
                UserType = user.Type,
                Account = user.LoginName,
                Signature = signature,
                Mobile = user.Mobile,
                UserId = user.ID,
                UserName = user.Name,
                Validity = user.Validity,
                Stamp = stamp,
                Secret = GetSecret(signature),
                FailureTime = expired
            };
            Tokens.Add(token);
            return token;
        }
    }
}