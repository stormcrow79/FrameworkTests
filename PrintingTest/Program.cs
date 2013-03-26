using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace PrintingTest
{
  class Program
  {
    static double MmToPx(double value)
    {
      return value / 25.4 * 96;
    }
    static Rect MmToPx(Rect value)
    {
      return new Rect(MmToPx(value.X), MmToPx(value.Y), MmToPx(value.Width), MmToPx(value.Height));
    }
    static double PtToPx(double value)
    {
      return value / 72 * 96;
    }
    static FormattedText ProduceFT(string value)
    {
      return new FormattedText(value, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, face, size, Brushes.Black);
    }
    static void DrawText(DrawingContext context, string value)
    {
      var ft = ProduceFT(value);
      context.DrawText(ft, new Point(0, top));
      top += ft.Height;
    }

    static Typeface face = new Typeface("Consolas");
    static double size = 12.0;
    static double top;

    static void ObtainGlyphRuns(Drawing drawing, List<GlyphRun> glyphRuns)
    {
      var group = drawing as DrawingGroup;
      if (group != null)
      {
        // recursively go down the DrawingGroup
        foreach (Drawing child in group.Children)
        {
          ObtainGlyphRuns(child, glyphRuns);
        }
      }
      else
      {
        var glyphRunDrawing = drawing as GlyphRunDrawing;
        if (glyphRunDrawing != null)
        {
          // add the glyph run to the collection
          var glyphRun = glyphRunDrawing.GlyphRun;
          if (glyphRun != null)
          {
            glyphRuns.Add(glyphRun);
          }
        }
      }
    }
    static void Main(string[] args)
    {
      DrawingContext context;
      GlyphTypeface glyphTypeface;
      face.TryGetGlyphTypeface(out glyphTypeface);

      for (int i = 32; i < 96; i++)
      {
        var s = String.Format("{0}", (char)i);
        var ft = ProduceFT(s);
        GlyphRun gr = null;

        var drawing = new DrawingGroup();
        context = drawing.Open();
        context.DrawText(ft, new Point(0, 0));
        var walker = new GlyphRunWalker();
        context.Close();
        ObtainGlyphRuns(drawing, walker.GlyphRuns);

        walker.start();

        Console.WriteLine("'{0}', {1}, {2}, {3}", s, ft.Width, ft.WidthIncludingTrailingWhitespace, glyphTypeface.AdvanceWidths[walker.current]);
      }
      Console.ReadLine();

      var image = new BitmapImage(new Uri(@"file:///c:\incoming\pos\lrsc.png"));

      var server = new PrintServer();
      var queue = new PrintQueue(server, "EPSON TM-T88V Receipt");
      queue.UserPrintTicket = new PrintTicket() { PageMediaSize = new PageMediaSize(MmToPx(80), MmToPx(1)) };

      var writer = PrintQueue.CreateXpsDocumentWriter(queue);
      writer.WritingPrintTicketRequired += (sender, e) =>
      {
        Console.WriteLine("PrintTicketRequired");
      };
      var visual = new DrawingVisual();
      context = visual.RenderOpen();
      //var line1 = new FormattedText("         1         2         3         4         5         6         7", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, face, size, Brushes.Black);
      //var line2 = new FormattedText("1234567890123456789012345678901234567890123456789012345678901234567890", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, face, size, Brushes.Black);

      //context.DrawImage(image, new Rect(ToPx(4), 0, ToPx(72), 68)); top += 68;
      DrawText(context, "Lone Ranges Shooting Complex");
      DrawText(context, "107-109 Robinson Ave, BELMONT WA 6104");
      DrawText(context, "Phone: (618) 9277 9200");
      DrawText(context, "Dealer No.: 9993912");
      DrawText(context, "ABN: 84 286 785 988");
      DrawText(context, "");
      DrawText(context, "7/03/2013 12:56 PM");
      DrawText(context, "");
      DrawText(context, "Name: DAVID CENTAMAN");
      DrawText(context, "Member No.: 24453");
      DrawText(context, "Served by: PAULD");
      DrawText(context, "");
      DrawText(context, "Own firearm - 'Peashooter'        20.00");
      DrawText(context, "(1AKK469) [Safe 2-4]");
      DrawText(context, "9mm Ammunition (60 rounds)        40.00");
      DrawText(context, "Glock 19 9mm Magazine 10 rounds   55.95");
      DrawText(context, "Nestea Lemon                       3.50");
      DrawText(context, "Nestea Lemon                       3.50");
      DrawText(context, "Snickers 53g                       2.20");
      DrawText(context, "Snickers 53g                       2.20");
      DrawText(context, ".44 Ruger Rifle (30 rounds)      100.00");
      DrawText(context, "");
      DrawText(context, "                          TOTAL  227.35");
      DrawText(context, "                            GST   20.67");
      DrawText(context, "");
      DrawText(context, "                Amount tendered  250.00");
      DrawText(context, "                         Change   22.65");      
      
      context.Close();
      writer.Write(visual);
    }

    static void writer_WritingPrintTicketRequired(object sender, System.Windows.Documents.Serialization.WritingPrintTicketRequiredEventArgs e)
    {
      throw new NotImplementedException();
    }
  }
  class GlyphRunWalker
  {
    public List<GlyphRun> GlyphRuns { get; private set; }
    private int runIndex;
    private int glyphIndex;
    private int maxGlyphs;

    public GlyphRunWalker()
    {
      GlyphRuns = new List<GlyphRun>();
      current = 0;
      currentRunLength = 0;
    }

    public void start()
    {
      runIndex = 0;
      glyphIndex = 0;
      state();
    }

    public void next()
    {
      if (runIndex >= GlyphRuns.Count)
        throw new Exception("run out of glyphruns");
      glyphIndex++;
      if (glyphIndex == maxGlyphs)
      {
        runIndex++;
        glyphIndex = 0;
      }
      state();
    }

    private void state()
    {
      if (runIndex >= GlyphRuns.Count)
      {
        currentRunLength = 1;
        current = 0;
      }
      else
      {
        GlyphRun g = GlyphRuns[runIndex];
        if (g.GlyphIndices.Count == 0)
          throw new Exception("empty glyphRun!");

        if (g.ClusterMap == null)
        {
          maxGlyphs = g.GlyphIndices.Count;
          currentRunLength = 1;
          current = g.GlyphIndices[glyphIndex];
        }
        else
        {
          maxGlyphs = g.ClusterMap.Count;
          current = g.GlyphIndices[g.ClusterMap[glyphIndex]];
          currentRunLength = 0;
          foreach (var v in g.ClusterMap)
          {
            if (v == g.ClusterMap[glyphIndex])
              currentRunLength++;
          }
        }
      }
    }

    public ushort current { get; private set; }
    public ushort currentRunLength { get; private set; }
  }
}
