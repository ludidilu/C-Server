using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperServer.timer
{
    interface ITimerUnit
    {
        bool Check(int _time);
    }
}
