using System;
using System.Net.Sockets;
using System.Net;
using SuperServer.userManager;
using SuperServer.superService;

namespace SuperServer.server
{


    public class Server<T,U> where T : SuperUserService<U>,new() where U : UserData,new()
    {
        private Socket socket;

        public void Start(string _path, int _port)
        {
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

            ServerUnit<T,U> unit = new ServerUnit<T, U>(clientSocket);

            unit.Start();

            BeginAccept();
        }
    }
}
