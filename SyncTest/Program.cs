using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyncTest
{
  class Program
  {
    static void Main(string[] args)
    {
      int minWork, minIO, maxWork, maxIO;
      ThreadPool.GetMinThreads(out minWork, out minIO);
      ThreadPool.GetMaxThreads(out maxWork, out maxIO);
      Console.WriteLine($"ThreadPool size\nWork: {minWork}-{maxWork}\nIO: {minIO}-{maxIO}");
      int i;

      var sw = Stopwatch.StartNew();
      Parallel.Invoke(
        () =>
        {
          Thread.Sleep(2000);
          Console.WriteLine("1");
        },
        () =>
        {
          Thread.Sleep(5000);
          Console.WriteLine("2");
        }
      );
      sw.Stop();
      Console.WriteLine($"Completed after {sw.Elapsed.TotalSeconds} sec");    


      var events = new EventWaitHandle[32];
      for (i = 0; i < events.Length; i++)
        events[i] = new EventWaitHandle(false, EventResetMode.ManualReset);

      while (true)
      {
        ThreadPool.QueueUserWorkItem(value =>
        //var thread = new Thread(() =>
        {
          var index = Interlocked.Increment(ref i);
          events[i % events.Length].WaitOne();
          Console.WriteLine(index);
        });
        //thread.Start();

        //Thread.Sleep(500);
      }
    }
  }
}
