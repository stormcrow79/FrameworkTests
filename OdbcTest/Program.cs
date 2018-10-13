using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Text;

namespace OdbcTest {
  class Program {
    static void Main(string[] args) {

      // SQL setup
      // CREATE TABLE timetest ([key] int identity(1,1), value time(7))
      // INSERT INTO timetest (value) VALUES ('08:00'), ('12:00'), ('16:00'), ('20:00')

      // CREATE TABLE datetest ([key] int identity(1,1), value date)
      // INSERT INTO datetest (value) VALUES ('2012-01-02'), ('2012-02-03'), ('2012-03-04'), ('2012-04-05')

      string driver = "{SQL Server Native Client 11.0}";
      string server = "gavinm";
      string database = "XRG_Dev";

      using (var connection = new OdbcConnection(String.Format("Driver={0};Server={1};Database={2};Trusted_Connection=yes", driver, server, database))) {
        connection.Open();
        using (var command = connection.CreateCommand()) {
          try {


            #region date - http://technet.microsoft.com/en-us/library/bb630352
            command.CommandText = "SELECT value FROM datetest WHERE value < { d '2012-02-15' }"; // OK
            //command.CommandText = "SELECT value FROM datetest WHERE value < ?"; 
            //command.Parameters.AddWithValue("value", new DateTime(2012, 02, 15, 12, 34, 56)); // OK - fails if ms specified

            using (var reader = command.ExecuteReader())
              while (reader.Read()) Console.WriteLine(reader.GetDate(0));
            #endregion

            #region time - http://technet.microsoft.com/en-us/library/bb677243
            command.CommandText = "SELECT value FROM timetest WHERE value < { t '12:34:56' }"; // incompatible
            //command.CommandText = "SELECT value FROM timetest WHERE value < '12:34:56.123456789'"; // OK
            //command.CommandText = "SELECT value FROM timetest WHERE value < ?";
            //command.Parameters.AddWithValue("value", new TimeSpan(0, 12, 34, 56, 789)); // OK

            using (var reader = command.ExecuteReader())
              while (reader.Read()) Console.WriteLine(reader.GetTime(0));
            #endregion
          } catch (Exception ex) {
            Console.WriteLine(ex.Message);
          }

          Console.ReadKey();
        }
      }
    }
  }
}
