using System;
using System.Collections.Generic;
using SuperServer.superService;
using SuperServer.redis;
using SuperServer.server;

namespace SuperServer.userManager
{
    public class UserManager<T,U> : SuperService where T : SuperUserService<U>, new() where U : UserData, new()
    {
        private static UserManager<T,U> _Instance;

        public static UserManager<T,U> Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new UserManager<T,U>();
                }

                return _Instance;
            }
        }

        private Dictionary<string, T> dic = new Dictionary<string , T>();

        internal void Login(string _userName,string _password,ServerUnit<T,U> _serverUnit) 
        {
            if (dic.ContainsKey(_userName))
            {
                T userService = dic[_userName];
                
                Action<T> callBack = delegate (T _service)
                {
                    _service.Login(_userName, _password, _serverUnit);
                };

                userService.Process(callBack);
            }
            else
            {
                Action<Redis> callBack = delegate (Redis _service)
                {
                    _service.Login<T,U>(_userName, _password, _serverUnit);
                };

                Redis.Instance.Process(callBack);
            }
        }

        internal void GetLoginResult(U _userData,ServerUnit<T,U> _serverUnit)
        {
            if(_userData != null)
            {
                T userService = new T();

                userService.Init(_userData, _serverUnit);

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
