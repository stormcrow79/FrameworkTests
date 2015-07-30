using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ServiceModel;

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
    public void Test()
    {
      Console.Write("Bar.Test");
    }
    /*void IFoo.Test()
    {
      Console.Write("IFoo.Test");
    }*/
  }
  class Program
  {
    static void Main(string[] args)
    {
      var a = "a";
      var b = "b";
      var c = a + b;

      var an = new AssemblyName("Test");
      var ab = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);
      var mdb = ab.DefineDynamicModule("Test", "Test.dll");

      TypeBuilder cbitb, cbtb, pitb, ptb;
      CustomAttributeBuilder cab;
      Type callbackIntf, callback, providerIntf, provider;
      MethodBuilder mtb;
      ILGenerator ilg;
      ServiceContractAttribute sca;
      object x;

      cbitb = mdb.DefineType("Test.IFoo1Callback", TypeAttributes.Interface | TypeAttributes.Abstract);
      cbitb.DefineMethod("Bar", MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Abstract | MethodAttributes.Virtual);
      callbackIntf = cbitb.CreateType();

      cbtb = mdb.DefineType("Test.Foo1Callback", TypeAttributes.Class, typeof(object), new Type[] { callbackIntf });
      mtb = cbtb.DefineMethod("Bar", MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.Public);
      ilg = mtb.GetILGenerator();
      ilg.Emit(OpCodes.Ret);
      callback = cbtb.CreateType();

      //x = Activator.CreateInstance(callback);

      pitb = mdb.DefineType("Test.IFoo1", TypeAttributes.Interface | TypeAttributes.Abstract);
      cab = new CustomAttributeBuilder(
        typeof(ServiceContractAttribute).GetConstructor(new Type[] { }),
        new object[] { },
        new PropertyInfo[] { typeof(ServiceContractAttribute).GetProperty("Name"), typeof(ServiceContractAttribute).GetProperty("CallbackContract") },
        new object[] { "Foo1Contract", cbitb /*callbackIntf*/ } // FIX HERE
      );
      pitb.SetCustomAttribute(cab);
      providerIntf = pitb.CreateType();

      ptb = mdb.DefineType("Test.Foo1Provider", TypeAttributes.Class, typeof(object), new Type[] { providerIntf });
      provider = ptb.CreateType();

      //ab.Save("Test.dll");

      sca = providerIntf.GetCustomAttribute<ServiceContractAttribute>();

      return;

      //////////////////////////////////////////////

      /*DateTime? a = new DateTime(1899, 12, 31);
      var b = new DateTime(2013, 4, 9);
      var d = a.Value.Subtract(b);*/

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
