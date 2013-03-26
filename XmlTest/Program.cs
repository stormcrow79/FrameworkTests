using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace XmlTest {
  class Program {
    static void Main(string[] args) {
      var content1 = "<WordProcessor><Table Background=\"red\" Background=\"green\" Style=\"foo\"><Row><Cell/></Row></Table></WordProcessor>";
      //XmlDocument doc1 = new XmlDocument();
      //doc1.LoadXml(content1);

      var settings = new XmlReaderSettings();
      var reader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(content1)), settings);
      XmlNode last;
      while (reader.Read()) { }
      
      XmlDocument doc2 = new XmlDocument();
      doc2.LoadXml(@"	<KeyImages>
		<StudyID>1.2.840.113704.1.111.5072.1124177427.1</StudyID>
		<SerieID>1.2.840.113704.1.111.5072.1124178290.24</SerieID>
		<UID>1.2.840.113704.1.111.3220.1124178396.2650</UID>
		<StudyID>1.2.840.113704.1.111.5072.1124177427.1</StudyID>
		<SerieID>1.2.840.113704.1.111.5072.1124178243.20</SerieID>
		<UID>1.2.840.113704.1.111.3220.1124178333.2271</UID>
	</KeyImages>");

      foreach (XmlElement studyUID in doc2.SelectNodes("/KeyImages/StudyID")) {
        var seriesUID = studyUID.SelectSingleNode("following-sibling::*");
        var objectUID = seriesUID.SelectSingleNode("following-sibling::*");

        Console.WriteLine("studyUID={0}&seriesUID={1}&objectUID={2}", 
          studyUID.InnerText, seriesUID.InnerText, objectUID.InnerText);
      }
      
      Console.WriteLine();
    }
  }
}
