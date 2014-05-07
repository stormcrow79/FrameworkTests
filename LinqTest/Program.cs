using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqTest
{
  class Request
  {
    public DateTimeOffset? RegisteredDate;
  }
  class Program
  {
    static void Main(string[] args)
    {
      var input = new string[] { "value1", "value2", "value3", "value4" };
      string output;
      
      output = String.Join(",", input);
      output = input.Aggregate((a, b) => a + "," + b);

      var test1 = new Request[] 
      { 
        //new Request() { RegisteredDate = new DateTimeOffset(2011, 6, 3, 11, 12, 13, TimeSpan.FromHours(8)) },
        //new Request()
      };
      var result1 = test1.Min(r => r.RegisteredDate);

      Console.WriteLine(output);
      Console.ReadLine();
    }
  }
}
