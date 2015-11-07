using System;
using SuperServer.userManager;
using SuperServer.server;
using SuperProto;
using System.Collections.Generic;

namespace SuperServer.superService
{
    public class SuperUserServiceBase : SuperService
    {
        public static Dictionary<BaseProto, Action<BaseProto>> dataHandleDic = new Dictionary<BaseProto, Action<BaseProto>>();

        private ServerUnit serverUnit;
        private Action replaceServerUnitCallBack;

        private bool isWaittingForResponse;

        internal virtual void SetUserData(UserData _userData)
        {
        }

        internal void SetServerUnit(ServerUnit _serverUnit)
        {
            serverUnit = _serverUnit;
        }

        internal virtual void Login(string _userName, string _password, ServerUnit _serverUnit)
        {
            if (serverUnit != null)
            {
                Kick();

                serverUnit.Kick();

                serverUnit = null;
            }

            if (isWaittingForResponse)
            {
                replaceServerUnitCallBack = delegate ()
                {
                    serverUnit = _serverUnit;

                    _serverUnit.GetLoginResult(this);
                };
            }
            else
            {
                serverUnit = _serverUnit;

                _serverUnit.GetLoginResult(this);
            }
        }

        protected virtual void Kick()
        {

        }

        internal void GetData(BaseProto _data)
        {
            isWaittingForResponse = true;

            dataHandleDic[_data](_data);
        }

        protected void SendData(bool _beginReceive, BaseProto _data)
        {
            if (_beginReceive)
            {
                isWaittingForResponse = false;
            }

            if (serverUnit != null)
            {
                serverUnit.SendData(_beginReceive, _data);
            }
            else
            {
                if (replaceServerUnitCallBack != null)
                {
                    replaceServerUnitCallBack();

                    replaceServerUnitCallBack = null;
                }
            }
        }
    }
}
