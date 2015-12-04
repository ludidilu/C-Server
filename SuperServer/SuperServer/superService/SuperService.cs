using System;
using System.Collections.Generic;
using System.Threading;
using SuperServer.locker;

namespace SuperServer.superService
{
    public class SuperService
    {
        private readonly SuperLocker processLocker = new SuperLocker();
        private readonly SuperLocker callLocker = new SuperLocker();

        private bool inProcessQueue = false;

        private List<Delegate> actionList = new List<Delegate>();

        public void Process<T>(Action<T> _action) where T : SuperService
        {
            lock (processLocker)
            {
                actionList.Add(_action);

                if (!inProcessQueue)
                {
                    inProcessQueue = true;

                    ThreadPool.QueueUserWorkItem(StartProcess<T>);
                }
            }
        }

        public void Call<T>(Action<T> _action) where T : SuperService
        {
            lock (callLocker)
            {
                _action(this as T);
            }
        }

        public R Call<T,R>(Func<T,R> _action) where T : SuperService
        {
            lock (callLocker)
            {
                return _action(this as T);
            }
        }

        private void StartProcess<T>(object _param) where T : SuperService
        {
            while (true)
            {
                Action<T> action;

                lock (processLocker)
                {
                    if (actionList.Count == 0)
                    {
                        inProcessQueue = false;

                        return;
                    }

                    action = actionList[0] as Action<T>;

                    actionList.RemoveAt(0);
                }

                lock (callLocker)
                {
                    action(this as T);
                }
            }
        }
    }
}
