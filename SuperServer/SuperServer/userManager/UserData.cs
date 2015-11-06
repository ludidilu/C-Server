using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperServer.userManager
{
    [System.Serializable]
    public class UserData
    {
        public string userName;
        public string passward;

        public void Init(string _userName,string _password)
        {
            userName = _userName;
            passward = _password;
        }
    }
}
