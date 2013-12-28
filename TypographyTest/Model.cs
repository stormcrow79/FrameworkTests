using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TypographyTest
{
  namespace Content
  {
    public class Style
    {
      public FontFamily FontFamily { get; set; }
      public double FontSize { get; set; }
      public FontStretch FontStretch { get; set; }
      public FontStyle FontStyle { get; set; }
      public FontWeight FontWeight { get; set; }
      public Color ForegroundColor { get; set; }
    }
    public class Element
    {
    }
    public class Inline : Element
    {
    }
    public class Block : Element
    {
    }
    public class Paragraph : Block
    {
      public Paragraph()
      {
        Inline = new List<Inline>();
      }
      public List<Inline> Inline { get; set; }
    }
    public class Cell : Inline
    {
      public Cell()
      {
        Content = new List<Element>();
      }
      public List<Element> Content { get; set; }
    }
    public class Row : Block
    {
      public Row()
      {
        Cell = new List<Cell>();
      }
      public List<Cell> Cell { get; set; }
    }
    public class Table : Block
    {
      public Table()
      {
        Row = new List<Row>();
      }
      public List<Row> Row { get; set; }
    }
    public class Text : Inline
    {
      public Text(string value)
      {
        this.Value = value;
      }
      public string Value { get; set; }
    }
    public class Image : Inline
    {
      public double Width { get; set; }
      public double Height { get; set; }
    }
    public class Document
    {
      public Document()
      {
        Block = new List<Block>();
      }
      public List<Block> Block { get; set; }
      public Style DefaultStyle { get; set; }
      public CultureInfo CultureInfo { get; set; }
    }
  }
  /*namespace Editor
  {
    // TODO: do we need an editor model? individual words seems useful
  }*/
  namespace Layout
  {
    public class Base
    {
      public Rect Bounds { get; set; }
    }
    public class Page : Base
    {
      public Page()
      {
        Section = new List<Section>();
      }
      public List<Section> Section { get; set; }
    }
    public class Section : Base
    {
      public Section()
      {
        Row = new List<Row>();
      }
      public List<Row> Row { get; set; }
    }
    public class Row : Base
    {
      public Row()
      {
        Cell = new List<Cell>();
      }
      public List<Cell> Cell { get; set; }
    }
    public class Cell : Base
    {
      public string Text { get; set; }
    }
  }
}
