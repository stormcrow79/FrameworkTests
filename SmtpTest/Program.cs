using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SmtpTest
{
  class Program
  {
    static void Main(string[] args)
    {
      //var server = "10.250.1.18";
      //var server = "per-ex2";
      var server = "mail3.kestral.com.au";
      //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
      var client = new SmtpClient(server, 2525);
      client.EnableSsl = true;
      client.UseDefaultCredentials = true;
      client.Send(new MailMessage("gavinm@kestral.com.au", "gavinm@kestral.com.au", "Test", "Hello"));
    }
  }
}
