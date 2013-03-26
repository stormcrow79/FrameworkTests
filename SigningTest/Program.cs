using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SigningTest
{
  class Program
  {
    static void Main(string[] args)
    {
      // issue
      //makecert -r -pe -n "CN=Centaman Software Licensing" -b 01/01/2000 -e 01/01/2099 -eku 1.3.6.1.5.5.7.3.3 -ss My -a sha256

      // message
      var data = new byte[32];

      // sign
      var cert = new X509Certificate2();
      cert.Import(@"c:\development\visual studio\frameworktests\signingtest\almostpurple-root.pfx", "EnxnApv1", X509KeyStorageFlags.PersistKeySet);
      var sigbytes = ((RSACryptoServiceProvider)cert.PrivateKey).SignData(data, "SHA1");

      // verify
      var verifier = new RSACryptoServiceProvider();
      verifier.ImportParameters(LoadParameters(Assembly.GetExecutingAssembly().GetName().GetPublicKey()));
      Console.WriteLine(verifier.VerifyData(data, "SHA1", sigbytes));

      if (Debugger.IsAttached) Console.ReadLine();
    }
    public static RSAParameters LoadParameters(byte[] data)
    {
      var result = new RSAParameters();
      var keylen = BitConverter.ToInt32(data, 24);
      result.Exponent = new byte[4];
      Array.Copy(data, 28, result.Exponent, 0, result.Exponent.Length);
      Array.Reverse(result.Exponent);
      result.Modulus = new byte[keylen / 8];
      Array.Copy(data, 32, result.Modulus, 0, result.Modulus.Length);
      Array.Reverse(result.Modulus);
      return result;
    }
  }
}
