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
      var s = entry.Attributes[attr];
      if (s == null)
        return null;
      return (string)s.GetValues(typeof(string))[0];
    }
    static void Main(string[] args)
    {
      int pageSize = 1000;
      //args = new[] { "kestral.local", "kestral.local" };
      //args = new[] { "mn-ad-1", "mnadtest.local", "administrator", "number1!", "mnadtest" };        

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

          Console.WriteLine(request.TimeLimit);

          try
          {
            var t = Stopwatch.StartNew();

            var pageRequestControl = new PageResultRequestControl(pageSize);

            // used to retrieve the cookie to send for the subsequent request
            request.Controls.Add(pageRequestControl);

            // please please AD, give me the results
            //request.Controls.Add(new SearchOptionsControl(System.DirectoryServices.Protocols.SearchOption.DomainScope));

            while (true)
            {
              var response = (SearchResponse)connection.SendRequest(request);

              Console.WriteLine("{0}\t{1} entries: {2} - {3} in {4:F1}", DateTime.Now, response.Entries.Count,
                AttributeOf(response.Entries[0], "cn"),
                AttributeOf(response.Entries[response.Entries.Count - 1], "cn"),
                t.Elapsed.TotalSeconds
              );

              var pageResponseControl = response.Controls.OfType<PageResultResponseControl>().First();
              if (pageResponseControl.Cookie.Length == 0)
                break;
              pageRequestControl.Cookie = pageResponseControl.Cookie;
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
