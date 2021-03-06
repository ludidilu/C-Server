﻿using System;
using System.Net.Sockets;
using System.Net;
using SuperServer.userManager;
using SuperServer.superService;
using SuperProto;
using SuperServer.timer;

namespace SuperServer.server
{
    public class Server
    {
        private static Server _Instance;

        public static Server Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = new Server();
                }

                return _Instance;
            }
        }

        private Socket socket;

        internal static Func<SuperUserServiceBase> getUserService;

        internal static Func<UserData> getUserData;

        public void Start<T,U>(string _path, int _port, int _maxConnections) where T : SuperUserService<U>,new() where U : UserData,new()
        {
            getUserService = delegate()
            {
                return new T();
            };

            getUserData = delegate ()
            {
                return new U();
            };

            Action<SuperUserServiceBase, BaseProto> callBack = delegate (SuperUserServiceBase _service, BaseProto _data)
            {
                _service.SendUserData(_data as UserDataProto);
            };

            SuperUserServiceBase.SetDataHandle<UserDataProto>(callBack);
            
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(new IPEndPoint(IPAddress.Parse(_path), _port));

            socket.Listen(_maxConnections);

            BeginAccept();
        }

        private void BeginAccept()
        {
            socket.BeginAccept(SocketAccept, null);
        }

        private void SocketAccept(IAsyncResult _result)
        {
            Socket clientSocket = socket.EndAccept(_result);

            Console.WriteLine("One user connected");

            ServerUnit unit = new ServerUnit(clientSocket);

            unit.Start();

            BeginAccept();
        }
    }
}
