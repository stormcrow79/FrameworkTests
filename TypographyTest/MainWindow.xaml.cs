using System;
using System.Collections.Generic;
using System.IO;
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
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      using (var reader = new StreamReader(@"c:\incoming\wordprocessor\art-of-war-preface.txt"))
        Editor.Document = PersistenceController.LoadFromText(reader);
    }

    Content.PersistenceEngine PersistenceController = new Content.PersistenceEngine();
    Layout.LayoutEngine LayoutController = new Layout.LayoutEngine();
    Content.Document Document;
    Layout.Section Section;
  }
}
