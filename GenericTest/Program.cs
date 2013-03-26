using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GenericTest {
  class Program {
    static void Main(string[] args) {
      var genericType = typeof(Dictionary<,>);
      var concreteType = genericType.MakeGenericType(typeof(int), typeof(string));
      var tryGetMethod = concreteType.GetMethod("TryGetValue");
      
      Func<int, float> x = (d) => { return d+1; };

    }
  }
}
