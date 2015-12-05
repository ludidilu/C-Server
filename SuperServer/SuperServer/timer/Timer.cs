using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SuperServer.superService;
using SuperServer.locker;

namespace SuperServer.timer
{
    public class Timer
    {
        private static Timer _Instance;

        public static Timer Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = new Timer();
                }

                return _Instance;
            }
        }

        private int timeSpan;

        private int time;

        private SuperLocker locker = new SuperLocker();

        private List<ITimerUnit> list = new List<ITimerUnit>();

        public void Start(int _timeSpan)
        {
            timeSpan = _timeSpan;

            ThreadPool.QueueUserWorkItem(Start);
        }

        private void Start(object _obj)
        {
            time = 0;

            while (true)
            {
                Thread.Sleep(timeSpan);

                time = time + timeSpan;

                lock (locker)
                {
                    for (int i = list.Count - 1; i > -1; i--)
                    {
                        bool result = list[i].Check(time);

                        if (result)
                        {
                            list.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public void DelayCall<T>(T _service, Action<T> _callBack, int _time) where T : SuperService
        {
            lock (locker)
            {
                TimerUnit<T> unit = new TimerUnit<T>(_service,_callBack,time + _time);

                list.Add(unit);
            }
        }
    }
}
