using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperServer.userManager
{
    internal interface IBaseData
    {
        bool IsChange();

        void SetName(string _name);
        
        string GetName();

        object GetChangeData();

        object GetData();
    }
}
