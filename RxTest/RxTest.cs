using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RxTest
{
  class Program
  {
    static void Main(string[] args)
    {
      var client = new Client();

      var source = Observable.Range(1, 1000);

      var r = new Random();

      source.ObserveOn(Scheduler.Default).Subscribe(value => {
        Thread.Sleep(r.Next(10) * 100);
        Console.WriteLine($"T{Thread.CurrentThread.ManagedThreadId}V{value}");
      });
      source.ObserveOn(Scheduler.Default).Subscribe(value => {
        Thread.Sleep(r.Next(10) * 100);
        Console.WriteLine($"\tT{Thread.CurrentThread.ManagedThreadId}V{value}");
      });
      source.ObserveOn(Scheduler.Default).Subscribe(value => {
        Thread.Sleep(r.Next(10) * 100);
        Console.WriteLine($"\t\tT{Thread.CurrentThread.ManagedThreadId}V{value}");
      });
      source.ObserveOn(Scheduler.Default).Subscribe(value => {
        Thread.Sleep(r.Next(10) * 100);
        Console.WriteLine($"\t\t\tT{Thread.CurrentThread.ManagedThreadId}V{value}");
      });

      Console.ReadLine();
    }
  }

  delegate void ResponseDelegate(Request request, Response response);

  class Request
  {
    public string Type { get; set; }
  }

  class Response
  {
    public bool Complete { get; set; }
  }

  class Client
  {
    List<Request> requests = new List<Request>();

    public void AddRequest(Request request)
    {
      requests.Add(request);
    }
    public void Send()
    {
      foreach (var request in requests)
      {
        var count = request.Type == "FIND" ? 2 : 6;
        for (int i = 0; i < count; i++)
          OnResponse(request, new Response() { Complete = false });
        OnResponse(request, new Response() { Complete = true });
      }
    }

    public event ResponseDelegate OnResponse;
  }
}
