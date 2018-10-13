using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventTest
{
  class Program
  {
    static void Main(string[] args)
    {
      for (int i = 0; i < 20; i++)
      {
        //var name = $"Local\\{Guid.NewGuid()}"; // \Sessions\<id>\BaseNamedObjects\<name>
        //var name = $"Global\\{Guid.NewGuid()}";  // \BaseNamedObjects\<name>
        var name = new string((char)(i+65), 260);// \Sessions\<id>\BaseNamedObjects\<name>
        _list.Add(new EventWaitHandle(false, EventResetMode.AutoReset, name));
      }
      Console.ReadLine();
    }
    static List<EventWaitHandle> _list = new List<EventWaitHandle>();
  }
}
