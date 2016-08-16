using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace XmlTest {
  class Program {
    static void Main(string[] args) {
      /*var content1 = "<WordProcessor><Table Background=\"red\" Background=\"green\" Style=\"foo\"><Row><Cell/></Row></Table></WordProcessor>";
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
      }*/

      /*var settings = new XmlReaderSettings();
      var reader = XmlReader.Create(File.OpenRead(@"..\..\sample1.xml"), settings);
      while (reader.Read())
        Console.WriteLine("{0}, {1}, {2}, {3}", reader.LocalName, reader.Name, reader.NamespaceURI, reader.Prefix);*/

      /*var settings = new XmlWriterSettings();
      var writer = XmlWriter.Create(File.Create(@"..\..\sample2.xml"), settings);
      writer.WriteStartDocument();
      writer.WriteStartElement("book", "urn:loc.gov:books");
      writer.WriteAttributeString("xmlns", "isbn", null, "urn:ISBN:0-395-36341-6");
      writer.WriteElementString("title", "Cheaper by the Dozen");
      writer.WriteElementString("number", "urn:ISBN:0-395-36341-6", "1568491379");
      writer.WriteEndElement();
      writer.Close();*/

      /*var settings = new XmlWriterSettings();
      var writer = XmlWriter.Create(File.Create(@"..\..\sample3.xml"), settings);
      writer.WriteStartDocument();
      writer.WriteStartElement("Personnel", null);
      writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
      writer.WriteAttributeString("noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "PersonnelSchema.xsd");
      writer.WriteStartElement("CREATE");
      writer.WriteStartElement("NewPerson");
      writer.WriteElementString("FirstName", "Callan");
      writer.WriteEndElement();
      writer.Close();*/

      var settings = new XmlWriterSettings() { 
        Indent = true, 
        //NamespaceHandling = NamespaceHandling.OmitDuplicates, 
        OmitXmlDeclaration = true, 
        Encoding = Encoding.GetEncoding(1252)
      };

      using (var writer = System.Xml.XmlWriter.Create(@"c:\incoming\out.xml", settings))
      {
        writer.WriteStartDocument();
        writer.WriteStartElement("WordProcessor");
        writer.WriteStartElement("Field");
        writer.WriteString("");
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndDocument();
      }

      //Console.ReadLine();
    }
  }
}
