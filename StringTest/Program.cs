using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringTest
{
  class Program
  {
    static void Main(string[] args)
    {
      var data = File.ReadAllBytes(@"c:\incoming\mobile\davidw-225343.html"); //Convert.FromBase64String(Content);

      int i = 0;
      int fixups = 0;
      while (i < data.Length)
      {
        if (data[i] == 10 && (i == 0 || data[i - 1] != 13))        
          fixups++;
        i++;
      }

      if (fixups > 0)
      {
        var data2 = new byte[data.Length + fixups];
        int source = 0;
        int dest = 0;
        int length = 0;

        while (source < data.Length)
        {
          while (source + length < data.Length && data[source + length] != 10) length++;
          Array.Copy(data, source, data2, dest, length);
          if (dest + length < data2.Length)
          {
            data2[dest + length] = 13;
            data2[dest + length + 1] = 10;
          }
          source += length + 1;
          dest = dest + length + 2;
          length = 0;        
        }
        Console.ReadLine();
      }
    }
  }
}
