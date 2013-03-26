using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace LayoutTest {
  //[StructLayout(LayoutKind.Explicit, Size=8)]
  struct StartRecord {
    //[FieldOffset(0)] 
    public int RecordSize;
    //[FieldOffset(4)]
    public bool NoVersionCheckParameter;
    //[FieldOffset(5)]
    public bool NoExceptionTrackingParameter;
    //[FieldOffset(6)]
    public bool SingleUserParameter;
    //[FieldOffset(7)]
    public bool DebugParameter;
  }
  class Program {
    static void Main(string[] args) {
      //Console.WriteLine(sizeof(StartRecord));
      Console.WriteLine(Marshal.SizeOf(typeof(StartRecord)));
      Console.ReadLine();
    }
  }
}
