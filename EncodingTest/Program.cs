using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncodingTest
{
  class Program
  {
    static void Main(string[] args)
    {
      var softHyphen = "\u00AD";
      var isWhite = String.IsNullOrWhiteSpace(softHyphen);
      var input = "\u00b2\u25a1";
      var utf8Bytes = Encoding.UTF8.GetBytes(input);
      var w1252Bytes = Encoding.GetEncoding(1252).GetBytes(input);
      Console.ReadLine();
    }
  }
}
