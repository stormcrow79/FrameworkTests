using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SecurityTest
{
  class Program
  {
    static void Main(string[] args)
    {
      var cache = new CredentialCache();
      var defaultCred = CredentialCache.DefaultCredentials;
      var defaultNetCred = CredentialCache.DefaultNetworkCredentials;
      
      SecureString password = new SecureString();
      foreach (var _char in "password".ToCharArray())
        password.AppendChar(_char);
      Console.WriteLine(password.ToString());
      Console.ReadLine();
    }
  }
}
