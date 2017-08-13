using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DelegateTest {
  class Program {
    static int value;
    static int LOOP_COUNT = Int32.MaxValue;
    void Test1()
    {
      value = 0;
      for (int i = 0; i < LOOP_COUNT; i++)
        value++;
    }
    void Test2()
    {
      value = 0;
      for (int i = 0; i < LOOP_COUNT; i++)
        Increment();
    }
    void Test3()
    {
      Action action = Increment;
      value = 0;
      for (int i = 0; i < LOOP_COUNT; i++)
        action();
    }
    void Test4()
    {
      Action action = () => Increment();
      value = 0;
      for (int i = 0; i < LOOP_COUNT; i++)
        action();
    }
    void Test6()
    {
      Action a2 = () => Increment();
      Action action = () => a2();
      value = 0;
      for (int i = 0; i < LOOP_COUNT; i++)
        action();
    }
    void Test7()
    {
      Action a12 = () => Increment();
      Action a11 = () => a12();
      Action a10 = () => a11();
      Action a9 = () => a10();
      Action a8 = () => a9();
      Action a7 = () => a8();
      Action a6 = () => a7();
      Action a5 = () => a6();
      Action a4 = () => a5();
      Action a3 = () => a4();
      Action a2 = () => a3();
      Action a1 = () => a2();
      value = 0;
      for (int i = 0; i < LOOP_COUNT; i++)
        a1();
    }
    static void Increment()
    {
      value++;
    }
    static void Time(string name, Action action)
    {
      var timer = Stopwatch.StartNew();
      action();
      timer.Stop();
      Console.WriteLine(name + ": " + timer.ElapsedMilliseconds.ToString());
    }

    static void Main(string[] args)
    {
      Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
      var p = new Program();
      Time("Inline", p.Test1);
      Time("Method", p.Test2);
      Time("Delegate", p.Test3);
      Time("Delegate2", p.Test4);
      Time("Delegate3", p.Test6);
      Time("Delegate12", p.Test7);

      int i = 0;
      Action x = () => { i = 1; };
      Action y = () => { Console.WriteLine(i); };

    }
  }
}
