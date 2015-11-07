using System;
using SuperServer.userManager;
using SuperServer.server;
using SuperProto;
using System.Collections.Generic;

namespace SuperServer.superService
{
    public class SuperUserService<T> : SuperService where T : UserData, new()
    {
        protected static Dictionary<BaseProto, Action<BaseProto>> dataHandleDic = new Dictionary<BaseProto, Action<BaseProto>>();

        protected T userData;
        private IServerUnit serverUnit;
        private Action replaceServerUnitCallBack;

        private bool isWaittingForResponse;

        internal void Init(T _userData, IServerUnit _serverUnit)
        {
            userData = _userData;
            serverUnit = _serverUnit;
        }

        internal void Login<F>(string _userName, string _password, ServerUnit<F, T> _serverUnit) where F : SuperUserService<T>, new()
        {
            bool result = userData.passward.Equals(_password);

            if (!result)
            {
                _serverUnit.GetLoginResult(null);
            }
            else
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

                        _serverUnit.GetLoginResult(this as F);
                    };
                }
                else
                {
                    serverUnit = _serverUnit;

                    _serverUnit.GetLoginResult(this as F);
                }
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

            if(serverUnit != null)
            {
                serverUnit.SendData(_beginReceive, _data);
            }
            else
            {
                if(replaceServerUnitCallBack != null)
                {
                    replaceServerUnitCallBack();

                    replaceServerUnitCallBack = null;
                }
            }
        }
    }
}
