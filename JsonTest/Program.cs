using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTest
{
  class Program
  {
    static void Main(string[] args)
    {
      JsonConverter x;
      dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("config.json"));
      Console.WriteLine(config);
      Console.WriteLine(config.Karisma);
      Console.WriteLine(config.Karisma.Host);
      Console.WriteLine((object)config.Diagnostic);

      var conf = new Configuration() { Karisma = config.Karisma, Diagnostic = config.Diagnostic };

      Console.ReadLine();
    }
  }
  public class Configuration
  {
    public KarismaConfiguration Karisma { get; set; }
    public DiagnosticConfiguration Diagnostic { get; set; }
  }
  public class KarismaConfiguration
  {
    public string Host { get; set; }
    public int Port { get; set; }
  }
  public class DiagnosticConfiguration
  {
    public string HtmlDump { get; set; }
  }
}
