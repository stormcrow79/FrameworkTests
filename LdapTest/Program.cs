using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LdapTest
{
  class Program
  {
    static void Main(string[] args)
    {
      // LdapTest <address> <domain> [<username> <password> [<domain>]]
      //              0        1          2          3           4
      var directory = new LdapDirectoryIdentifier(args[0]);
      var credential = args.Length > 4 ? new NetworkCredential(args[2], args[3], args[4])
        : args.Length > 2 ? new NetworkCredential(args[2], args[3])
        : new NetworkCredential();

      using (var connection = new LdapConnection(directory, credential))
      {
        while (true)
        {
          var request = new SearchRequest("DC=" + args[1].Replace(".", ",DC="), "(&(objectClass=organizationalPerson)(sAMAccountType=805306368))",
            System.DirectoryServices.Protocols.SearchScope.Subtree, new[] { "mail" });
          try
          {
            var result = connection.SendRequest(request) as SearchResponse;
            Console.WriteLine("{0}\t{1} entries", DateTime.Now, result.Entries.Count);
          }
          catch (Exception ex)
          {
            Console.WriteLine("{0}\tERRROR - {1}", DateTime.Now, ex.Message);
          }
          Thread.Sleep(TimeSpan.FromSeconds(30));
        }
      }
    }
  }
}
