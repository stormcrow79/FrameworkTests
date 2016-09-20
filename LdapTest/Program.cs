using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    static string AttributeOf(SearchResultEntry entry, string attr)
    {
      return (string)entry.Attributes[attr]?.GetValues(typeof(string))[0];
    }
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
        //while (true)
        {
          var request = new SearchRequest(
            "DC=" + args[1].Replace(".", ",DC="),
            "(&(objectClass=organizationalPerson)(sAMAccountType=805306368))",
            System.DirectoryServices.Protocols.SearchScope.Subtree,
            new[] { "cn" }
          );

          try
          {
            var t = Stopwatch.StartNew();

            PageResultRequestControl pageRequestControl = new PageResultRequestControl(1000);

            // used to retrieve the cookie to send for the subsequent request
            PageResultResponseControl pageResponseControl;
            request.Controls.Add(pageRequestControl);

            while (true)
            {
              var response = (SearchResponse)connection.SendRequest(request);
              pageResponseControl = (PageResultResponseControl)response.Controls[0];
              if (pageResponseControl.Cookie.Length == 0)
                break;
              pageRequestControl.Cookie = pageResponseControl.Cookie;
              Console.WriteLine("{0}\t{1} entries: {2} - {3} in {4:F1}", DateTime.Now, response.Entries.Count,
                AttributeOf(response.Entries[0], "cn"),
                AttributeOf(response.Entries[response.Entries.Count - 1], "cn"),
                t.Elapsed.TotalSeconds
              );
            }
            t.Stop();
          }
          catch (Exception ex)
          {
            Console.WriteLine("{0}\tERRROR - {1}", DateTime.Now, ex.Message);
          }
          //Thread.Sleep(TimeSpan.FromSeconds(30));
        }
      }
    }
  }
}
