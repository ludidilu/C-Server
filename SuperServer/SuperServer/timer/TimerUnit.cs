using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperServer.superService;

namespace SuperServer.timer
{
    class TimerUnit<T> : ITimerUnit where T : SuperService
    {
        private T service;

        private Action<T> callBack;

        private int time;

        public TimerUnit(T _service,Action<T> _callBack,int _time)
        {
            service = _service;

            callBack = _callBack;

            time = _time;
        }

        public bool Check(int _time)
        {
            if(_time > time)
            {
                service.Process(callBack);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
