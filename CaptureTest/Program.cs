using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaptureTest
{
  class Program
  {
    static List<object[]> list = new List<object[]>();
    static void Main(string[] args)
    {
      //var list = new List<object[]>();
      for (int i = 0; i < 1; i++)
      {
        var x = new object();
        var ts = new ThreadStart(() =>
        {
          var y = new object[1024];
          for (int j = 0; j < y.Length; j++)
            y[j] = new object();
          list.Add(y);
          Console.WriteLine(x);
        });
        var t = new Thread(ts);
      }
    }
  }
}
