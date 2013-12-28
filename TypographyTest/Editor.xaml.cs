using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TypographyTest
{
  /// <summary>
  /// Interaction logic for Editor.xaml
  /// </summary>
  public partial class Editor : Control
  {
    public Editor()
    {
      InitializeComponent();
    }

    public Content.Document Document { get; set; }

    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);
    }
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      if (sizeInfo.WidthChanged && Document != null) layoutEngine.Layout(Document, sizeInfo.NewSize.Width);
    }

    Layout.LayoutEngine layoutEngine = new Layout.LayoutEngine();
  }
}
