using System;
using System.Collections.Generic;
using System.Threading;

namespace SuperServer.superService
{
    class SuperServiceLocker
    {
    }

    public class SuperService
    {
        private SuperServiceLocker processLocker = new SuperServiceLocker();
        private SuperServiceLocker callLocker = new SuperServiceLocker();

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
