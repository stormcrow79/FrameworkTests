using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace AsyncTest
{
  static class Helper
  {
    public static Task<bool> TryConnect(this TcpClient client, IPEndPoint ep)
    {
      var task = new Task<bool>(() =>
      {
        try
        {
          client.Connect(ep);
          return client.Connected;
        }
        catch
        {
          return false;
        }
      });
      return task;
    }
  }
  class Program
  {
    static void Main(string[] args)
    {
      Parallel.Invoke( () => { }, () => { });

      IObservable


      var karismatest = IPAddress.Parse("10.250.2.51");
      var endpoints = new IPEndPoint[] { 
        new IPEndPoint(karismatest, 40102),
        new IPEndPoint(karismatest, 65532),
        new IPEndPoint(karismatest, 40105),
        new IPEndPoint(karismatest, 40107),
        new IPEndPoint(karismatest, 40109),
        new IPEndPoint(IPAddress.Parse("10.250.48.18"), 40109),
      };

      foreach (var ep in endpoints)
      {
        var client = new TcpClient();

        /*client.TryConnect(ep).RunSynchronously();
        Console.WriteLine("{0}: {1}", ep, client.Connected);
        client.Close();*/

        /*client.ConnectAsync(ep.Address, ep.Port).ContinueWith(Task => {
          Console.WriteLine("{0}: {1}", ep, client.Connected);
          client.Close();
        });*/

        var task = client.TryConnect(ep);
        task.ContinueWith(prior => {
          Console.WriteLine("{0}: {1}", ep, prior.Result);
          client.Close();
        });
        task.Start();
      }
      Console.ReadLine();
    }
  }
}
