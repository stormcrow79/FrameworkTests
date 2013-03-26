using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTest
{
  class Program
  {
    static void Main(string[] args)
    {
      var BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Integrad.3\MIV\");
      foreach (string PossiblePath in Directory.GetDirectories(BasePath))
      {
      }
    }
  }
}
