using System;
using System.Collections.Generic;
using SuperServer.superService;
using SuperServer.redis;
using SuperServer.server;
using SuperServer.locker;

namespace SuperServer.userManager
{
    internal class UserManager : SuperService
    {
        private static readonly SuperLocker locker = new SuperLocker();

        private static UserManager _Instance;

        internal static UserManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (locker)
                    {
                        if(_Instance == null)
                        {
                            _Instance = new UserManager();
                        }
                    }
                }

                return _Instance;
            }
        }

        private Dictionary<string, SuperUserServiceBase> dic = new Dictionary<string , SuperUserServiceBase>();

        internal void Login(string _userName,string _password,ServerUnit _serverUnit) 
        {
            if (dic.ContainsKey(_userName))
            {
                SuperUserServiceBase userService = dic[_userName];
                
                Action<SuperUserServiceBase> callBack = delegate (SuperUserServiceBase _service)
                {
                    _service.Login(_userName, _password, _serverUnit);
                };

                userService.Process(callBack);
            }
            else
            {
                Action<Redis> callBack = delegate (Redis _service)
                {
                    _service.Login(_userName, _password, _serverUnit);
                };

                Redis.Instance.Process(callBack);
            }
        }

        internal void GetLoginResult(UserData _userData,ServerUnit _serverUnit)
        {
            if(_userData != null)
            {
                SuperUserServiceBase userService = Server.getUserService();

                userService.SetUserData(_userData);

                userService.SetServerUnit(_serverUnit);

                dic.Add(_userData.userName, userService);

                _serverUnit.GetLoginResult(userService);
            }
            else
            {
                _serverUnit.GetLoginResult(null);
            }
        }
    }
}
