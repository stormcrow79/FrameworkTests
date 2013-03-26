using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CounterTest {
  class Program {
    static void Main(string[] args) {
      PerformanceCounterCategory category = new PerformanceCounterCategory("Karisma");
      PerformanceCounter counter = new PerformanceCounter("Karisma", "Messages/sec", "Live");
      counter.Increment();
    }
  }
}
