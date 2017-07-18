using System;
using System.Collections.Generic;
using System.Threading;

namespace SuperServer.superService
{
    public class SuperService
    {
        private readonly object processLocker = new object();
        private readonly object callLocker = new object();

        private bool inProcessQueue = false;//判断是否已经召唤线程去执行process队列了  使用这个属性一定要持有processLocker的锁

        private int callNum = 0;//记录call的等待数量  使用这个属性一定要持有processLocker的锁

        private Queue<Delegate> processActionList = new Queue<Delegate>();

        public void Process<T>(Action<T> _action) where T : SuperService
        {
            bool needCallThread = false;

            lock (processLocker)
            {
                processActionList.Enqueue(_action);

                if (!inProcessQueue && callNum == 0)
                {
                    inProcessQueue = true;

                    needCallThread = true;
                }
            }

            if (needCallThread)
            {
                ThreadPool.QueueUserWorkItem(StartProcess<T>);
            }
        }

        private void BeginCall()
        {
            lock (processLocker)
            {
                callNum++;
            }
        }

        private void EndCall<T>() where T : SuperService
        {
            bool needCallThread = false;

            lock (processLocker)
            {
                callNum--;

                if (!inProcessQueue && callNum == 0 && processActionList.Count > 0)
                {
                    inProcessQueue = true;

                    needCallThread = true;
                }
            }

            if (needCallThread)
            {
                ThreadPool.QueueUserWorkItem(StartProcess<T>);
            }
        }

        public void Call<T>(Action<T> _action) where T : SuperService
        {
            BeginCall();

            lock (callLocker)
            {
                _action(this as T);
            }

            EndCall<T>();
        }

        public R Call<T, R>(Func<T, R> _action) where T : SuperService
        {
            R result;

            BeginCall();

            lock (callLocker)
            {
                result = _action(this as T);
            }

            EndCall<T>();

            return result;
        }

        private void StartProcess<T>(object _param) where T : SuperService
        {
            while (true)
            {
                Action<T> action;

                lock (processLocker)
                {
                    if (processActionList.Count == 0 || callNum > 0)
                    {
                        inProcessQueue = false;

                        return;
                    }

                    action = processActionList.Dequeue() as Action<T>;
                }

                lock (callLocker)
                {
                    action(this as T);
                }
            }
        }
    }
}
