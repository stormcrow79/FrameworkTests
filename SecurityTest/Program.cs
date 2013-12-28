using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SecurityTest
{
  class Program
  {
    static void Main(string[] args)
    {
      var sid = new SecurityIdentifier("S-1-5-21-4214557091-3990953747-1280595-1133");
      //var sid = new SecurityIdentifier("S-1-5-21-2971375037-12398727-3747011064-1131");
      string username = null;
      try
      {
        username = sid.Translate(typeof(NTAccount)).ToString();
      }
      catch
      {
        username = sid.ToString();
      }

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
