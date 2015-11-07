using System;
using SuperServer.userManager;
using SuperServer.server;
using SuperProto;
using System.Collections.Generic;

namespace SuperServer.superService
{
    public class SuperUserService<T> : SuperUserServiceBase where T : UserData, new()
    {
        protected T userData;

        internal override void SetUserData(UserData _userData)
        {
            userData = _userData as T;
        }

        internal override void Login(string _userName, string _password, ServerUnit _serverUnit)
        {
            bool result = userData.passward.Equals(_password);

            if (!result)
            {
                _serverUnit.GetLoginResult(null);
            }
            else
            {
                base.Login(_userName, _password, _serverUnit);
            }
        }
    }
}
