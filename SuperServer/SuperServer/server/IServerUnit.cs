using SuperServer.protocol;
using SuperServer.superService;
using SuperServer.userManager;

namespace SuperServer.server
{
    public interface IServerUnit
    {
        void Kick();

        void SendData(bool _beginReceive, BaseProto _data);

        //void GetResultFromUserManager<T,U>(T _userService) where T : SuperUserService<U>,new() where U : UserData,new();
    }
}
