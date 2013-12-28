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
    public class PersistenceEngine
    {
      public Document LoadFromText(StreamReader reader)
      {
        var result = new Document();
        result.DefaultStyle = new Style()
        {
          FontFamily = new FontFamily("Calibri"),
          FontSize = 11,
          FontStretch = FontStretches.Normal,
          FontStyle = FontStyles.Normal,
          FontWeight = FontWeights.Normal
        };
        string line = null;
        var builder = new StringBuilder();
        while ((line = reader.ReadLine()) != null)
        {
          if (line == "")
          {
            var paragraph = new Paragraph();
            paragraph.Inline.Add(new Text(builder.ToString()));
            result.Block.Add(paragraph);
            builder.Length = 0;
          }
          else
          {
            builder.Append(line);
          }
        }
        return result;
      }
    }
  }
  namespace Layout
  {
    public class Typography
    {
      private class GlyphRunWalker : IEnumerator<ushort>
      {
        public GlyphRunWalker(IList<GlyphRun> glyphRuns)
        {
          this.glyphRuns = glyphRuns;
        }

        public ushort CurrentGlyphIndex { get; private set; }
        public ushort CurrentGlyphLengthInChars { get; private set; }
        public double CurrentGlyphWidth { get; private set; }
        /// <summary>
        /// Current glyph width divided by the number of contributing characters
        /// </summary>
        public double EffectiveWidth { get; private set; }

        IList<GlyphRun> glyphRuns;
        int runIndex;
        int glyphIndex;
        int maxGlyphs;

        public void start()
        {
          runIndex = 0;
          glyphIndex = 0;
          state();
        }
        public void next()
        {
          if (runIndex >= glyphRuns.Count)
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
          if (runIndex >= glyphRuns.Count)
          {
            CurrentGlyphLengthInChars = 1;
            CurrentGlyphIndex = 0;
          }
          else
          {
            GlyphRun g = glyphRuns[runIndex];
            if (g.GlyphIndices.Count == 0)
              throw new Exception("empty glyphRun!");

            if (g.ClusterMap == null)
            {
              maxGlyphs = g.GlyphIndices.Count;
              CurrentGlyphLengthInChars = 1;
              CurrentGlyphIndex = g.GlyphIndices[glyphIndex];
            }
            else
            {
              maxGlyphs = g.ClusterMap.Count;
              CurrentGlyphIndex = g.GlyphIndices[g.ClusterMap[glyphIndex]];
              CurrentGlyphLengthInChars = 0;
              foreach (var v in g.ClusterMap)
              {
                if (v == g.ClusterMap[glyphIndex])
                  CurrentGlyphLengthInChars++;
              }
            }
            double glyphWidth;
            if (!g.GlyphTypeface.AdvanceWidths.TryGetValue(CurrentGlyphIndex, out glyphWidth))
              glyphWidth = g.AdvanceWidths[0x1a]; // unicode replacement character
            CurrentGlyphWidth = glyphWidth;
            EffectiveWidth = CurrentGlyphLengthInChars == 1 ? CurrentGlyphWidth : CurrentGlyphWidth / CurrentGlyphLengthInChars;
          }
        }

        ushort IEnumerator<ushort>.Current
        {
          get { return CurrentGlyphIndex; }
        }
        void IDisposable.Dispose() { }
        object System.Collections.IEnumerator.Current
        {
          get { return CurrentGlyphIndex; }
        }
        bool System.Collections.IEnumerator.MoveNext()
        {
          next();
          return runIndex < glyphRuns.Count;
        }
        void System.Collections.IEnumerator.Reset()
        {
          start();
        }
      }

      internal void GetGlyphRuns(Drawing drawing, List<GlyphRun> result)
      {
        var group = drawing as DrawingGroup;
        if (group != null)
        {
          // recursively go down the DrawingGroup
          foreach (Drawing child in group.Children)
            GetGlyphRuns(child, result);
        }
        else
        {
          var glyphRunDrawing = drawing as GlyphRunDrawing;
          if (glyphRunDrawing != null)
          {
            // add the glyph run to the collection
            var glyphRun = glyphRunDrawing.GlyphRun;
            if (glyphRun != null) result.Add(glyphRun);
          }
        }
      }
      public GlyphRun[] GetGlyphRuns(FormattedText ft)
      {
        var drawing = new DrawingGroup();
        var drawingContext = drawing.Open();
        drawingContext.DrawText(ft, new Point(0, 0));
        drawingContext.Close();

        List<GlyphRun> result = new List<GlyphRun>();
        GetGlyphRuns(drawing, result);
        return result.ToArray();
      }
      public int[] CalculateBreaks(string value, Typeface typeface, double size)
      {
        return new int[0];
      }
    }

    public class LayoutEngine
    {
      public LayoutEngine()
      {
        PageHeight = new List<double>();
      }
      
      public Section Layout(Content.Document document, double maxWidth)
      {
        this.document = document;
        this.maxWidth = maxWidth;

        Section result = new Section();
        foreach (var block in document.Block)
        {
          if (block is Content.Paragraph)
            Layout(result, (Content.Paragraph)block);
          else if (block is Content.Table)
            Layout(result, (Content.Table)block);
        }
        return result;
      }

      public double ContentHeight { get; set; }
      public List<double> PageHeight { get; set; }

      void Layout(Section result, Content.Paragraph paragraph)
      {
        foreach (var inline in paragraph.Inline)
        {
          if (inline is Content.Text)
            Layout(result, (Content.Text)inline);
          else if (inline is Content.Image)
            Layout(result, (Content.Image)inline);
        }
      }

      void Layout(Section result, Content.Text text)
      {
        var typeface = new Typeface(document.DefaultStyle.FontFamily, document.DefaultStyle.FontStyle, document.DefaultStyle.FontWeight, document.DefaultStyle.FontStretch);
        var ft = new FormattedText(text.Value, document.CultureInfo ?? CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, document.DefaultStyle.FontSize, new SolidColorBrush(document.DefaultStyle.ForegroundColor));
        int index = 0;
        
      }

      void Layout(Section result, Content.Image image)
      {
        throw new NotImplementedException();
      }

      void Layout(Section result, Content.Table table)
      {
        throw new NotImplementedException();
      }

      Content.Document document;
      double maxWidth;
    }
  }
}
