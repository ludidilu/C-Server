using System;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using SuperServer.superService;
using SuperProto;
using SuperServer.userManager;

namespace SuperServer.server
{
    internal class ServerUnit
    {
        private static readonly int HEAD_LENGTH = 4;

        private static readonly int BODY_LENGTH = 10240;

        private Socket socket;

        private int headLength;

        private int headOffset;

        private byte[] headBuffer = new byte[HEAD_LENGTH];

        private int bodyLength;

        private int bodyOffset;

        private byte[] bodyBuffer = new byte[BODY_LENGTH];
        
        private MemoryStream receiveStream = new MemoryStream();

        private BinaryFormatter reveiveFormatter = new BinaryFormatter();

        private bool isSendingData;

        private List<BaseProto> sendPool = new List<BaseProto>();

        private MemoryStream sendStream = new MemoryStream();

        private BinaryFormatter sendFormatter = new BinaryFormatter();

        private SuperUserServiceBase userService;

        public ServerUnit(Socket _socket)
        {
            socket = _socket;
        }

        internal void Start()
        {
            headLength = HEAD_LENGTH;

            headOffset = 0;

            ReceiveHead();
        }

        private void ReceiveHead()
        {
            socket.BeginReceive(headBuffer, headOffset, headLength, SocketFlags.None, ReceiveHeadEnd, null);
        }

        private void ReceiveHeadEnd(IAsyncResult _result)
        {
            try
            {
                int i = socket.EndReceive(_result);

                if (i == 0)
                {
                    Console.WriteLine("disconnect!");
                }
                else if (i < headLength)
                {
                    headOffset = headOffset + i;

                    headLength = headLength - i;

                    ReceiveHead();
                }
                else
                {
                    bodyLength = BitConverter.ToInt32(headBuffer, 0);

                    bodyOffset = 0;

                    headLength = HEAD_LENGTH;

                    headOffset = 0;

                    ReceiveBody();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("disconnect!" + e.ToString());
            }
        }

        private void ReceiveBody()
        {
            socket.BeginReceive(bodyBuffer, bodyOffset, bodyLength, SocketFlags.None, ReceiveBodyEnd, null);
        }

        private void ReceiveBodyEnd(IAsyncResult _result)
        {
            int i = socket.EndReceive(_result);

            if (i == 0)
            {
                Console.WriteLine("disconnect!");
            }
            else if(i < bodyLength)
            {
                bodyOffset = bodyOffset + i;

                bodyLength = bodyLength - i;

                ReceiveBody();
            }
            else
            {
                receiveStream.Position = 0;

                receiveStream.Write(bodyBuffer, 0, bodyOffset + i);

                receiveStream.Position = 0;

                BaseProto data = reveiveFormatter.Deserialize(receiveStream) as BaseProto;

                GetData(data);
            }
        }

        private void GetData(BaseProto _data)
        {
            Console.WriteLine("GetData:" + _data.GetType().ToString());

            if (userService != null)
            {
                Action<SuperUserServiceBase> callBack = delegate (SuperUserServiceBase _service)
                {
                    _service.GetData(_data);
                };

                userService.Process(callBack);
            }
            else
            {
                LoginProto loginProto = _data as LoginProto;

                Action<UserManager> callBack = delegate (UserManager _service)
                {
                    _service.Login(loginProto.userName, loginProto.password, this);
                };

                UserManager.Instance.Process(callBack);
            }
        }

        public void SendData(BaseProto _data)
        {
            lock (sendPool)
            {
                if (!isSendingData)
                {
                    isSendingData = true;
                }
                else
                {
                    sendPool.Add(_data);

                    return;
                }
            }

            SendDataReal(_data);
        }

        private void SendDataReal(BaseProto _data)
        {
            sendFormatter.Serialize(sendStream, _data);

            sendStream.Position = 0;

            bool beginReceive = _data.type == PROTO_TYPE.S2C;

            Console.WriteLine("SendDataReal:" + _data.GetType().ToString());

            socket.BeginSend(BitConverter.GetBytes(sendStream.GetBuffer().Length), 0, HEAD_LENGTH, SocketFlags.None, SendHeadEnd, beginReceive);
        }

        private void SendHeadEnd(IAsyncResult _result)
        {
            socket.EndSend(_result);
            
            socket.BeginSend(sendStream.GetBuffer(), 0, sendStream.GetBuffer().Length, SocketFlags.None, SendBodyEnd, _result.AsyncState);
        }

        private void SendBodyEnd(IAsyncResult _result)
        {
            socket.EndSend(_result);

            bool beginReceive = (bool)_result.AsyncState;

            if (beginReceive)
            {
                ReceiveHead();
            }

            BaseProto sendData = null;

            lock (sendPool)
            {
                if (sendPool.Count > 0)
                {
                    sendData = sendPool[0];

                    sendPool.RemoveAt(0);
                }
                else
                {
                    isSendingData = false;
                }
            }

            if(sendData != null)
            { 
                SendDataReal(sendData);
            }
        }

        internal void GetLoginResult(SuperUserServiceBase _userService)
        {
            LoginResultProto proto = new LoginResultProto();

            if (_userService != null)
            {
                userService = _userService;

                proto.result = true;
            }
            else
            {
                proto.result = false;
            }

            Console.WriteLine("LoginResult:{0}", proto.result);

            SendData(proto);
        }

        public void Kick()
        {
            userService = null;
        }
    }
}
