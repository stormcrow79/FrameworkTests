using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTest
{
  #region Model
  class HumanName
  {
    public string Surname;
    public string FirstName;
  }
  class HumanInstance
  {
    public HumanName PreferredName;
    public HumanName[] OtherNames;
    public DateTime DateOfBirth;
    public bool Deceased;
  }
  class Patient
  {
    public HumanInstance HumanInstance;
  }
  class Request
  {
    public string Identifier;
    public Patient Patient;
    public Service[] Service;
  }
  class Service
  {
    public ServiceDefinition OrderedDefinition;
    public ServiceDefinition PerformedDefinition;
  }
  class ServiceDefinition
  {
    public string Code;
    public string Name;
  }
  #endregion
  class Grid<T>
  {
    public void AddColumn<TResult>(Expression<Func<T, TResult>> selector)
    {
      selectors.Add(selector);
    }
    public List<LambdaExpression> selectors = new List<LambdaExpression>();

    internal void Compile()
    {
      throw new NotImplementedException();
    }
  }
  class HumanNameRenderer
  {
    public Expression<Func<HumanInstance, object[]>> _DisplayName = (hi) => new object[] { hi.PreferredName.Surname.ToUpper(), hi.PreferredName.FirstName };
    public string DisplayName(HumanInstance hi)
    {
      return hi.PreferredName.Surname.ToUpper() + ", " + hi.PreferredName.FirstName;
    }
    public Expression<Func<HumanInstance, object[]>> _Age = (hi) => new object[] { hi.DateOfBirth, hi.Deceased };
    public string Age(HumanInstance hi)
    {
      if (hi.Deceased) return "Deceased";
      else return ((DateTime.Now - hi.DateOfBirth).TotalDays / 365.25).ToString("f0");
    }
  }
  class Program
  {
    static void Main(string[] args)
    {
      var renderer = new HumanNameRenderer();

      var grid = new Grid<Request>();
      grid.AddColumn(r => r.Identifier);
      grid.AddColumn(r => renderer.DisplayName(r.Patient.HumanInstance));
      grid.AddColumn(r => r.Patient.HumanInstance.DateOfBirth);
      grid.AddColumn(r => renderer.Age(r.Patient.HumanInstance));
      grid.Compile();

      var requests = new Request[] {
        new Request() {
          Identifier = "2012A0000001",
          Patient = new Patient() {
            HumanInstance = new HumanInstance() {
             PreferredName = new HumanName() { FirstName = "Justin", Surname = "Wake" },
             OtherNames = new HumanName[] { },
              DateOfBirth = new DateTime(1983, 3, 17)
            }
          }
        },
        new Request() {
          Identifier = "2012A0000002",
          Patient = new Patient() {
            HumanInstance = new HumanInstance() {
              PreferredName = new HumanName() { FirstName = "Gavin", Surname = "Morris" },
              OtherNames = new HumanName[] { },
              DateOfBirth = new DateTime(1979, 12, 5)
            }
          }
        }
      };

      var linq = requests.AsQueryable().Select(r => new { 
        Identifier = r.Identifier,
        PatientName1 = r.Patient.HumanInstance.PreferredName.Surname.ToUpper() + ", " + r.Patient.HumanInstance.PreferredName.FirstName,
        PatientName2 = renderer.DisplayName(r.Patient.HumanInstance)
      });

      Dump(grid, requests);
      Console.WriteLine("----------------");
      foreach (var r in linq)
        Console.WriteLine("{0}\t{1}\t{2}", r.Identifier, r.PatientName1, r.PatientName2);

      Console.ReadLine();
    }

    private static void Dump<T>(Grid<T> grid, IEnumerable<T> values)
    {
      Delegate[] selectors = new Delegate[grid.selectors.Count];
      for (int i = 0; i < grid.selectors.Count; i++)
        selectors[i] = grid.selectors[i].Compile();

      foreach (var v in values)
      {
        foreach (var s in selectors)
        {
          Console.Write(s.DynamicInvoke(v));
          Console.Write("\t");
        }
        Console.WriteLine();
      }
    }
  }
}
