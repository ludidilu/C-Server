using SuperProto;

namespace SuperServer.userManager
{
    internal interface IValueData
    {
        bool IsChange();

        void SetName(string _name);
        
        string GetName();

        ValueChangeBase GetChangeData();

        ValueBase GetData();
    }
}
