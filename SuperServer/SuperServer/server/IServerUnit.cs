using SuperProto;
using SuperServer.superService;
using SuperServer.userManager;

namespace SuperServer.server
{
    internal interface IServerUnit
    {
        void Kick();

        void SendData(bool _beginReceive, BaseProto _data);
    }
}
