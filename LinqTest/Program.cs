using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqTest
{
  class Program
  {
    static void Main(string[] args)
    {
      var input = new string[] { "value1", "value2", "value3", "value4" };
      string output;
      
      output = String.Join(",", input);
      output = input.Aggregate((a, b) => a + "," + b);
      
      Console.WriteLine(output);
      Console.ReadLine();
    }
  }
}
