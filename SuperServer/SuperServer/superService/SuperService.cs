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

        private List<Delegate> actionList = new List<Delegate>();

        public void Process<T>(Action<T> _action) where T : SuperService
        {
            bool isInQueue = false;

            lock (processLocker)
            {
                if (actionList.Count > 0)
                {
                    isInQueue = true;
                }

                actionList.Add(_action);
            }

            if (!isInQueue)
            {
                ThreadPool.QueueUserWorkItem(StartProcess<T>);
            }
        }

        public void Call<T>(Action<T> _action) where T : SuperService
        {
            lock (callLocker)
            {
                _action(this as T);
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
