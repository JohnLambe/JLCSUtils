using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util;
using JohnLambe.Util.Threading;

namespace JohnLambe.Tests.JLUtilsTest.Threading
{
    [TestClass]
    public class ThreadBaseTest
    {
        public const int TimeMultiplier = 1;

        [TestMethod]
        public void ThreadTest()
        {
            var thread = new CustomThread();
            
            thread.Start();
            
            Console.WriteLine("Main thread");
            Thread.Sleep(410 * TimeMultiplier);
            Console.WriteLine("Main thread. before Suspend");
//            Assert.AreEqual(thread.Counter, 4);

            thread.Suspend();

            Console.WriteLine("Main thread. after Suspend");
            Thread.Sleep(310 * TimeMultiplier);
//            Assert.AreEqual(thread.Counter, 4);
            Console.WriteLine("Main thread. before Resume");

            thread.Resume();

            Console.WriteLine("Main thread. after Resume");
            Thread.Sleep(300 * TimeMultiplier);
            Console.WriteLine("Main thread. before Stop");

            thread.Stop();

            Console.WriteLine("Main thread. after Stop");
            Thread.Sleep(200 * TimeMultiplier);
            Console.WriteLine("Main thread.");

            thread.Dispose();
            Console.WriteLine("Thread disposed");
        }

    }

    public class CustomThread : PublicThreadBase
    {
        public CustomThread()
        {
            ThreadInterval = ThreadBaseTest.TimeMultiplier * 100;
        }

        protected override bool MainLoop()
        {
            base.MainLoop();

            Console.WriteLine("In thread. " + Counter++);
//            Counter.Value = Counter.Value + 1;

            return true;
        }

        public int Counter { get; protected set; }
    }

    /*
     * 

        public T operator ++(ThreadSafe<T> a)
        {
            lock (this)
            {
                return _value++;
            }
        }
*/
}
