using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetTest
{
  class Program
  {
    static void Main(string[] args)
    {
      var ipgp = IPGlobalProperties.GetIPGlobalProperties();
      var tcp = ipgp.GetActiveTcpConnections();

      Console.WriteLine(" --- Connections to 0.0.0.0:9556 --- ");
      foreach (var t in tcp.Where(t => t.LocalEndPoint.Port == 9556))
        Console.WriteLine(t.RemoteEndPoint);

      Console.WriteLine();

      var entry = new MIB_TCPROW()
      {
        dwState = (int)TcpState.DeleteTcb,
        dwLocalAddr = 0x7f000001,
        dwLocalPort = 9556,
        dwRemoteAddr = 0x7f000001,
        dwRemotePort = 15529
      };

      Console.WriteLine(" --- Calling SetTcpEntry --- ");
      var rc = IpHlpApi.SetTcpEntry(ref entry);
      Console.WriteLine($"Result: {rc}");

      if (Debugger.IsAttached)
        Console.ReadLine();
    }
  }

  struct MIB_TCPROW
  {
    public int dwState;
    public int dwLocalAddr;
    public int dwLocalPort;
    public int dwRemoteAddr;
    public int dwRemotePort;
  }

  static class IpHlpApi
  {
    [DllImport("iphlpapi.dll")]
    public static extern int SetTcpEntry(ref MIB_TCPROW pTcpRow);
  }
}
