using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace SqlTest {
  class Program {
    static void Main(string[] args) {
      Stopwatch timer;
      using (var connection = new SqlConnection("Data Source=gavinm;Initial Catalog=rmh_311;Integrated Security=SSPI"))
      {
        connection.Open();
        int result;

        timer = Stopwatch.StartNew();
        using (var command = connection.CreateCommand()) {
          command.CommandText = "select count(*) from reportinstancevalues where datalength(blob) is not null and try_convert(xml, blob) is null";
          command.CommandTimeout = 0;
          result = (int)command.ExecuteScalar();
        }
        timer.Stop();
        Console.WriteLine("TRY_CONVERT: {0} failures in {1:n0}", result, timer.ElapsedMilliseconds);

        timer = Stopwatch.StartNew();
        using (var command = connection.CreateCommand())
        {
          result = 0;
          command.CommandText = "select blob from reportinstancevalues where datalength(blob) is not null";
          command.CommandTimeout = 0;
          using (var reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              var xml = new XmlDocument();
              try
              {
                xml.Load(new MemoryStream(reader.GetSqlBinary(0).Value));
              }
              catch (XmlException ex)
              {
                result++;
              }
            }
          }
        }
        timer.Stop();
        Console.WriteLine("XmlDocument: {0} failures in {1:n0}", result, timer.ElapsedMilliseconds);
      }
      
      /*using (var connection = new SqlConnection("Data Source=gavinm;Initial Catalog=qscan_dev;Integrated Security=SSPI")) {
        connection.Open();
        using (var command = connection.CreateCommand()) {
          command.CommandText = "SELECT [Key] FROM [Current].[Karisma.Practitioner.Record] WHERE Code = @Code";
          command.Parameters.Add("@Code", System.Data.SqlDbType.NVarChar, -1).Value = "ADAR";
          command.ExecuteReader();
        }
      }*/

/*
      Attributes.Add('SERVER=gavinm');
      Attributes.Add('DATABASE=Perf');
      Attributes.Add('Trusted_Connection=yes');

      Hstmt.SQL := 'UPDATE test SET value = :PU0 WHERE (id IN (select _KS.Data from :PC0 as _KS))';
      Hstmt.Prepare;
      Hstmt.BindTimestamp(1, aTimestamp);
      Hstmt.BindInteger32Set(2, oList);
      Hstmt.Execute;
      Hstmt.Terminate;
*/

      /*using (var conn = new OdbcConnection("Driver={SQL Server Native Client 11.0};SERVER=gavinm;DATABASE=Perf;Trusted_Connection=yes"))
      {
        conn.Open();
        using (var command = conn.CreateCommand())
        {
          command.CommandText = "UPDATE test SET value = @PU0 WHERE (id IN (select _KS.Data from @PC0 as _KS))";
          command.Parameters.Add("@PU0", OdbcType.DateTime).Value = DateTime.Parse("2012-12-03 08:45");
          //command.Parameters.Add("@PC0", OdbcType.ta
          command.ExecuteNonQuery();
        }
      }*/

      if (Debugger.IsAttached) Console.ReadLine();
    }
  }
}
