using System;
using SuperServer.userManager;
using SuperServer.server;
using SuperProto;
using System.Collections.Generic;

namespace SuperServer.superService
{
    public class SuperUserServiceBase : SuperService
    {
        private static Dictionary<Type, Action<SuperUserServiceBase, BaseProto>> dataHandleDic = new Dictionary<Type, Action<SuperUserServiceBase, BaseProto>>();

        public static void SetDataHandle<T>(Action<SuperUserServiceBase, BaseProto> _callBack) where T : BaseProto
        {
            dataHandleDic.Add(typeof(T), _callBack);
        }

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
            
            dataHandleDic[_data.GetType()](this, _data);
        }

        protected void SendData(BaseProto _data)
        {
            if (_data.type == PROTO_TYPE.S2C)
            {
                isWaittingForResponse = false;
            }

            if (serverUnit != null)
            {
                serverUnit.SendData(_data);
            }
            else
            {
                if (!isWaittingForResponse && replaceServerUnitCallBack != null)
                {
                    replaceServerUnitCallBack();

                    replaceServerUnitCallBack = null;
                }
            }
        }
    }
}
