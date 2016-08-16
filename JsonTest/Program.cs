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
      dynamic dt = JsonConvert.DeserializeObject("\"2016-05-06T16:15:00\"");
      Console.WriteLine(dt.Date);

      dynamic payload = JsonConvert.DeserializeObject(File.ReadAllText(@"..\..\payload.json"));
      Console.WriteLine(payload.data.ServiceDateTime.Value.Date);

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
