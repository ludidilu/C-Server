using System;
using System.Net.Sockets;
using System.Net;
using SuperServer.userManager;
using SuperServer.superService;

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

        public void Start<T,U>(string _path, int _port) where T : SuperUserService<U>,new() where U : UserData,new()
        {
            getUserService = delegate(){

                return new T();
            };

            getUserData = delegate ()
            {
                return new U();
            };
            
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(new IPEndPoint(IPAddress.Parse(_path), _port));

            socket.Listen(1000);

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
