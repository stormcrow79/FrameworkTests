using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketTest
{
  class Program
  {
    static void ClientHandler(object o)
    {
      var socket = (Socket)o;
      Console.WriteLine(socket.RemoteEndPoint.ToString());
      Thread.Sleep(120);
      socket.Close();
    }
    static void Main(string[] args)
    {
      if (args.Length == 0)
        Console.WriteLine("Usage: SocketTest.exe [-server | -client]");
      else if (args[0] == "-server")
      {
        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(0, 1234));
        socket.Listen(50);
        while (true)
        {
          var s = socket.Accept();
          var t = new Thread(ClientHandler);
          t.Start(s);
        }
      }
      else if (args[0] == "-client")
      {
        for (int i = 0; i < 128; i++)
        {
          var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
          socket.Connect("localhost", 1234);
          var buf = new byte[4];
          var len = socket.Receive(buf);
          Console.WriteLine("{0} ", len);
          socket.Close();
        }
      }
    }
  }
}
