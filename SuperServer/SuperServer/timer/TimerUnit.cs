using System;
using SuperServer.superService;

namespace SuperServer.timer
{
    class TimerUnit<T> : ITimerUnit where T : SuperService
    {
        private T service;

        private Action callBack;

        private int time;

        public TimerUnit(T _service, Action _callBack, int _time)
        {
            service = _service;

            callBack = _callBack;

            time = _time;
        }

        public bool Check(int _time)
        {
            if (_time > time)
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
