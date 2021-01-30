using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTest
{
    class Patient
    {
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Deleted { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"property name: {nameof(Patient.DateOfBirth)}");
            Console.WriteLine($"property name: {GetPropertyName(p => p.DateOfBirth)}");
            Console.WriteLine($"property value: {GetPropertyName(p => p.DateOfBirth)}");
        }



        static string GetPropertyName<TResult>(
            Expression<Func<Patient, TResult>> propertySelector,
            char delimiter = '.',
            char endTrim = ')')
        {
            if (propertySelector.NodeType != ExpressionType.Lambda || propertySelector.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException("invalid property accessor");

            return ((MemberExpression)propertySelector.Body).Member.Name;

            // gives you: "o => o.Whatever"
            var asString = propertySelector.ToString();

            // make sure there is a beginning property indicator; the "." in "o.Whatever" -- this may not be necessary?
            var firstDelim = asString.IndexOf(delimiter);

            return firstDelim < 0
                       ? asString
                       : asString.Substring(firstDelim + 1).TrimEnd(endTrim);
        }
    }
}
