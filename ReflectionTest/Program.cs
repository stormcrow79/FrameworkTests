using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionTest
{
  [Serializable]
  class Configuration
  {
    public string Username;
    public string Password;
    public string Url;
  }
  interface IFoo
  {
    void Test();
  }
  class Bar : IFoo
  {
    void Test()
    {
      Console.Write("Bar.Test");
    }
    void IFoo.Test()
    {
      Console.Write("IFoo.Test");
    }
  }
  class Program
  {
    static void Main(string[] args)
    {
      var configuration = new Configuration()
      {
        Username = "gavin",
        Password = "password",
        Url = "http://isite/web"
      };
      var dcs = new DataContractSerializer(typeof(Configuration));
      var stream = new MemoryStream();
      dcs.WriteObject(stream, configuration);
      var s = Encoding.UTF8.GetString(stream.ToArray());
      
      var t = JsonConvert.SerializeObject(configuration);

      var jds = new DataContractJsonSerializer(typeof(Configuration));
      stream = new MemoryStream();
      jds.WriteObject(stream, configuration);
      var u = Encoding.UTF8.GetString(stream.ToArray());
    }
  }
}
