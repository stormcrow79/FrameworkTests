using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTest
{
  class BaseControl
  {
    public void Click()
    {
      if (ClickEvent != null) ClickEvent();
    }
    public event Action ClickEvent;
  }
  class SpecificControl
  {
    public SpecificControl()
    {
      Base = new BaseControl();
      Base.ClickEvent += OnClick;
    }

    public event Action ClickEvent;
    private void OnClick()
    {
      var Action = ClickEvent;
      if (Action != null) 
        Action();
    }

    /*public event Action ClickEvent
    {
      add { Base.ClickEvent += value; }
      remove { Base.ClickEvent -= value; }
    }*/

    public void Click()
    {
      Base.Click();
    }

    BaseControl Base;
  }
  class Program
  {
    static void Main(string[] args)
    {
      var Control = new SpecificControl();
      Control.ClickEvent += () => Console.WriteLine("click");

      Control.Click();

      Control.ClickEvent += () => Console.WriteLine("click2");

      Control.Click();

      Console.ReadLine();
    }
  }
}
