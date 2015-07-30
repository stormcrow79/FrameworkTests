using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTest
{
  class RebateLibrary
  {
    public string Name;
  }
  class Program
  {
    static void Main(string[] args)
    {
      var rlt = Task.Run<RebateLibrary>(() =>
      {
        throw new ApplicationException();
        return new RebateLibrary() { Name = "Kestral" };
      });

      //Console.WriteLine(rlt.Result.Name);

      Console.WriteLine("Done");
      Console.ReadLine();
    }
  }
}
