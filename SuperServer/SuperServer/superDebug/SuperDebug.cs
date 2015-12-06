using System;
using SuperServer.locker;

namespace SuperServer.superDebug
{
    public class SuperDebug
    {
        private static SuperLocker locker = new SuperLocker();

        private static int num = 0;

        private static int maxNum = 0;

        public static void Add()
        {
            lock (locker)
            {
                num++;

                if(num > maxNum)
                {
                    maxNum = num;
                }
            }
        }

        public static void Del()
        {
            lock (locker)
            {
                num--;
            }
        }

        public static void Print()
        {
            lock (locker)
            {
                Console.WriteLine("threadMaxNum:{0}", maxNum);
            }
        }
    }
}
