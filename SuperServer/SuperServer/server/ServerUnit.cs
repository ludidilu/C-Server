using System;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SuperServer.superService;
using SuperServer.protocol;
using SuperServer.userManager;

namespace SuperServer.server
{
    public class ServerUnit<T,U> : IServerUnit where T : SuperUserService<U>,new() where U : UserData , new()
    {
        private Socket socket;

        private int headLength;

        private int headOffset;

        private byte[] headBytes = new byte[4];

        private int bodyLength;

        private int bodyOffset;

        private byte[] bodyBytes = new byte[1024];

        private byte[] sendBytes;

        private MemoryStream receiveStream = new MemoryStream();

        private BinaryFormatter reveiveFormatter = new BinaryFormatter();

        private MemoryStream sendStream = new MemoryStream();

        private BinaryFormatter sendFormatter = new BinaryFormatter();

        private T userService;

        public ServerUnit(Socket _socket)
        {
            socket = _socket;
        }

        internal void Start()
        {
            BeginReceive();
        }

        private void BeginReceive()
        {
            headLength = 4;

            headOffset = 0;

            socket.BeginReceive(headBytes, headOffset, headLength, SocketFlags.None, GetHead, null);
        }

        private void GetHead(IAsyncResult _result)
        {
            try
            {
                int i = socket.EndReceive(_result);

                if (i == 0)
                {
                    Console.WriteLine("disconnect!");

                    return;
                }
                else if (i < headLength)
                {
                    headOffset = headOffset + i;

                    headLength = headLength - i;

                    socket.BeginReceive(headBytes, headOffset, headLength, SocketFlags.None, GetHead, null);
                }
                else
                {
                    bodyLength = BitConverter.ToInt32(headBytes, 0);

                    bodyOffset = 0;

                    socket.BeginReceive(bodyBytes, bodyOffset, bodyLength, SocketFlags.None, GetBody, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("disconnect!");

                return;
            }
        }

        private void GetBody(IAsyncResult _result)
        {
            int i = socket.EndReceive(_result);

            if (i == 0)
            {
                Console.WriteLine("disconnect!");

                return;
            }
            else if(i < bodyLength)
            {
                bodyOffset = bodyOffset + i;

                bodyLength = bodyLength - i;

                socket.BeginReceive(bodyBytes, bodyOffset, bodyLength, SocketFlags.None, GetBody, null);
            }
            else
            {
                receiveStream.Position = 0;

                receiveStream.Write(bodyBytes, 0, i);

                receiveStream.Position = 0;

                BaseProto data = reveiveFormatter.Deserialize(receiveStream) as BaseProto;

                GetData(data);
            }
        }

        private void GetData(BaseProto _data)
        {
            if (userService != null)
            {
                Action<T> callBack = delegate (T _service)
                {
                    _service.GetData(_data);
                };

                userService.Process(callBack);
            }
            else
            {
                LoginProto loginProto = _data as LoginProto;

                Action<UserManager<T, U>> callBack = delegate (UserManager<T, U> _service)
                {
                    _service.Login(loginProto.userName, loginProto.password, this);
                };

                UserManager<T,U>.Instance.Process(callBack);
            }
        }

        public void SendData(bool _beginReceive,BaseProto _data)
        {
            sendFormatter.Serialize(sendStream, _data);

            sendBytes = sendStream.GetBuffer();
            
            socket.BeginSend(BitConverter.GetBytes(sendBytes.Length), 0, 4, SocketFlags.None, SendHead, _beginReceive);
        }

        private void SendHead(IAsyncResult _result)
        {
            socket.EndSend(_result);

            socket.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, SendBody, _result.AsyncState);
        }

        private void SendBody(IAsyncResult _result)
        {
            socket.EndSend(_result);

            bool beginReceive = (bool)_result.AsyncState;

            if (beginReceive)
            {
                BeginReceive();
            }
        }

        public void GetLoginResult(T _userService)
        {
            LoginResultProto result = new LoginResultProto();

            if (_userService != null)
            {
                userService = _userService;

                result.result = true;
            }
            else
            {
                result.result = false;
            }

            Console.WriteLine("LoginResult:{0}", result.result);

            SendData(true, result);
        }

        public void Kick()
        {
            userService = null;
        }
    }
}
