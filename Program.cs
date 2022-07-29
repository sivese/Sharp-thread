using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Parallelism
{
    public partial class Threadic
    {
        public Threadic()
        {
            ThreadParameter();
        }
    }

    /*
        Chapter 1
    */
    public partial class Threadic
    {
        private bool abortSign = false;

        private void ThreadParameter()
        {
            Action<int> CountNumbers = (int iter) => 
            {
                foreach (var i in Enumerable.Range(0, iter))
			    {
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                    Console.WriteLine("{0} prints {1}", Thread.CurrentThread.Name, i);
			    }
            };

            var par = new ParameterDelivery(10);

            var th1 = new Thread(par.CounterNum);
            th1.Name = "thread 1";
            th1.Start();
            th1.Join();

            Console.WriteLine("==============================");

            var th2 = new Thread( (object iter) => CountNumbers((int) iter) );
            th2.Name = "thread 2";
            th2.Start(8);
            th2.Join();

            Console.WriteLine("=============================");
            var th3 = new Thread(() => CountNumbers(12) );
            th3.Name = "thread 3";
            th3.Start();
            th3.Join();

            var PrintNumber = (int num) => Console.WriteLine(num);
            
            var i = 10;
            var th4 = new Thread(() => PrintNumber(i) );

            i = 20;
            var th5 = new Thread(() => PrintNumber(i));
            th4.Start();
            th5.Start();
        }

        class ParameterDelivery
        {
            private readonly int iter;

            public ParameterDelivery(int iter) => this.iter = iter;

            public void CounterNum()
            {
                foreach(var i in Enumerable.Range(1, iter))
                {
                    Thread.Sleep(500);
                    Console.WriteLine("{0} prints {1}", Thread.CurrentThread.Name, i);
                }
            }
        }

        private void ThreadGround()
        {
            var fore = new BFThread(10);
            var back = new BFThread(20);

            var th1 = new Thread(fore.CounterNumbers);
            th1.Name = "Foreground";
            var th2 = new Thread(back.CounterNumbers);
            th2.Name = "Background";
            th2.IsBackground = true;

            th1.Start();
            th2.Start();
        }

        class BFThread
        {
            private readonly int iter;

            public BFThread(int iter) => this.iter = iter;

            public void CounterNumbers()
            {
                foreach(var i in Enumerable.Range(0, iter))
                {
                    Thread.Sleep(500);
                    Console.WriteLine("{0} prints {1}", Thread.CurrentThread.Name, i);
                }
            }
        }

        public void ThreadPriority()
        {
            Console.WriteLine("Current thread priority : {0}", Thread.CurrentThread.Priority);
            Console.WriteLine("Run all cores available");

            RunThreads();
            Thread.Sleep(2500);

            Console.WriteLine("Run a single core");
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1);
            RunThreads();
        }

        private void RunThreads()
        {
            var mar = new ThreadMarmot();

            var th1 = new Thread(mar.CounterNum);
            th1.Name = "Thread one";
            
            var th2 = new Thread(mar.CounterNum);
            th2.Name = "Thread two";

            th1.Priority = System.Threading.ThreadPriority.Highest;
            th2.Priority = System.Threading.ThreadPriority.Lowest;

            th1.Start();
            th2.Start();

            Thread.Sleep(TimeSpan.FromSeconds(2));
            mar.Stop();
        }

        class ThreadMarmot
        {
            private bool isStop = false;

            public void Stop() => isStop = true;

            public void CounterNum()
            {
                long count = 0;

                while(!isStop) count++;

                Console.WriteLine("{0} with {1, 11} priority has a count = {2, 13} ", 
                                    Thread.CurrentThread.Name, 
                                    Thread.CurrentThread.Priority, 
                                    count.ToString());
            }
        }

        public void ThreadStatus()
        {
            var th1 = new Thread(PrintWithStatus);
            var th2 = new Thread(DoNothing);

            th2.Start();
            th1.Start();

            Thread.Sleep(TimeSpan.FromSeconds(3));

			Console.WriteLine(th1.ThreadState.ToString());
			Console.WriteLine(th2.ThreadState.ToString());
        }

        public void DoNothing() => Thread.Sleep(2000);

        public void PrintWithStatus()
        {
            Console.WriteLine(Thread.CurrentThread.ThreadState.ToString());

            foreach(var i in Enumerable.Range(0, 15))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(50));
                Console.WriteLine(i);
            }
        }

        // Start and join
        public void StartThread()
        {
            var thd = new Thread(Print);
            thd.Start();
            thd.Join();
            Console.WriteLine("Thread Completed");
        }

        public void AbortThread()
        {
            var thd = new Thread(Print);
            thd.Start();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            abortSign = true; // Abort doesn't support dotnet platform
        }

        private void Print()
        {
            new List<int>( Enumerable.Range(0, 15) )
                .ForEach (num => 
                {
                    if(abortSign) return;

                    Console.WriteLine("{0}", num);
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                });

            if(abortSign)
            {
                Console.WriteLine("Thread is aborted");
                abortSign = false;
            }
        }
    }
}