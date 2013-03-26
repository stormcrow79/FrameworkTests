using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using SqlDom = Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlDomTest {
  class Program {
    static void Main(string[] args) {
      using (var connection = new SqlConnection("Data Source=gavinm;Initial Catalog=CIG_Dev;Integrated Security=SSPI"))
      {
        connection.Open();
        List<string> scripts = new List<string>();
        using (var command = connection.CreateCommand())
        {
          command.CommandText = "SELECT definition FROM sys.sql_modules";
          using (var reader = command.ExecuteReader())
            while (reader.Read())
              scripts.Add(reader.GetString(0));
        }

        int success = 0;
        int error = 0;

        var timer = Stopwatch.StartNew();
        foreach (var script in scripts)
        {
          var parser = new SqlDom.TSql110Parser(false);
          IList<SqlDom.ParseError> errors;
          SqlDom.TSqlFragment fragment;
          using (var reader = new StringReader(script))
          {
            try
            {
              fragment = parser.Parse(reader, out errors);
              success++;
            }
            catch
            {
              error++;
            }
          }
        }
        timer.Stop();

        Console.WriteLine("Success: {0}\r\nErrors: {1}\r\nElapsed: {2}", success, error, timer.Elapsed);
      }
    }
  }
}
