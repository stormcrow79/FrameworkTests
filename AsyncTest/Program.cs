using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncTest
{
  class Program
  {
    static CancellationTokenSource cts = new CancellationTokenSource();

    static async Task ServerAsync(CancellationToken ct)
    {
      throw new ApplicationException("died");

      var client = new UdpClient(5555);

      var request = new byte[0];

      while (!ct.IsCancellationRequested)
      {
        await client.SendAsync(request, request.Length);
        var response = await client.ReceiveAsync();
      }
    }
    static void Main(string[] args)
    {
      /*var t = Task.Run(() => { Thread.Sleep(2000); return 123; });
      Console.WriteLine("Running");
      Console.WriteLine(t.Result);
      Console.ReadLine();*/

      var task = ServerAsync(cts.Token);

      Console.WriteLine("Server running. Press enter ...");

      Console.ReadLine();

      cts.Cancel();

      task.Wait();
      if (task.IsFaulted)
      {
        Console.WriteLine(task.Exception.Message);
      }

      Console.WriteLine("Finished");
      Console.ReadLine();
    }
  }
}
