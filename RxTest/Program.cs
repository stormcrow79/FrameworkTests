using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxTest
{
  class Program
  {
    static void Main(string[] args)
    {
      var client = new Client();
      var stream = Observable.FromEvent()
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
