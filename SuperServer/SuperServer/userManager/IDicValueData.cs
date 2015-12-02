using SuperProto;

namespace SuperServer.userManager
{
    internal interface IDicValueData
    {
        bool IsChange();

        void SetName(string _name);

        string GetName();

        DicChangeBase GetChangeData();

        DicBase GetData();
    }
}
