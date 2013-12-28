﻿using System;
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
    static int[] RandomKeys(IList<int> source, int count)
    {
      var r = new Random();
      var set = new HashSet<int>();
      while (set.Count > count)
        set.Add(source[r.Next(source.Count)]);
      var result = new int[count];
      set.CopyTo(result);
      return result;
    }
    static DataTable ProduceTable(IEnumerable<int> source)
    {
      var result = new DataTable();
      result.Columns.Add("data", typeof(long));
      foreach (var value in source) result.Rows.Add(value);
      return result;
    }
    static OdbcCommand ProduceIn(OdbcConnection connection, int[] keys)
    {
      var result = connection.CreateCommand();
      return result;
    }
    static OdbcCommand ProduceTvp(OdbcConnection connection, int[] keys)
    {
      var result = connection.CreateCommand();
      return result;
    }
    static void Main(string[] args) {
      Stopwatch timer;

      DataRow[] rows = new DataRow[1000000];

      var table = new DataTable();
      table.Columns.Add("ID", typeof(int));
      table.Columns.Add("Value", typeof(string));

      for (int i = 0; i < rows.Length; i++)
      {
        var row = table.NewRow();
        row.ItemArray = new object[] { i + 1, Convert.ToBase64String(BitConverter.GetBytes(DateTime.Now.Ticks)) };
        rows[i] = row;
      }

      using (var connection = new SqlConnection("Data Source=gavinm\\r2std;Initial Catalog=Perf;Integrated Security=SSPI"))
      using (var bulk = new SqlBulkCopy(connection))
      {
        connection.Open();
        bulk.BatchSize = 10000;
        bulk.DestinationTableName = "test";
        timer = Stopwatch.StartNew();
        bulk.WriteToServer(rows);
        timer.Stop();
        Console.WriteLine(timer.Elapsed);
      }

      Console.ReadLine();

      using (var connection = new SqlConnection("Data Source=gavinm\\r2std;Initial Catalog=Perf;Integrated Security=SSPI"))
      using (var bulk = new SqlBulkCopy(connection))
      {
        connection.Open();
        bulk.BatchSize = 10000;
        bulk.DestinationTableName = "test";
        timer = Stopwatch.StartNew();
        for (int i = 0; i < rows.Length; i++)
          bulk.WriteToServer(new [] { rows[i] });
        timer.Stop();
        Console.WriteLine(timer.Elapsed);
      }

      Console.ReadLine();
      return;

      using (var connection = new SqlConnection("Data Source=gavinm;Initial Catalog=Perf;Integrated Security=SSPI"))
      using (var command = connection.CreateCommand())
      {
        connection.Open();
        command.CommandText = "SELECT ID, Value FROM test2";
        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            var value = reader[1];
            Console.WriteLine(value);
          }
        }
        Console.ReadLine();
      }

      return;

      using (var connection = new SqlConnection("Data Source=gavinm;Initial Catalog=Perf;Integrated Security=SSPI"))
      {
        connection.Open();

        var bulk = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity, null)
        {
          BatchSize = 5000,
          BulkCopyTimeout = 60,
          DestinationTableName = "Test",
          EnableStreaming = true
        };

        try
        { 
          bulk.WriteToServer(new FakeReader());
        } 
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          Console.ReadLine();
        }
      }

      /*using (var connection = new OdbcConnection("Driver={SQL Server Native Client 10.0};Server=gavinm\\sql2008;Database=integration_311;Trusted_Connection=yes"))
      {
        connection.Open();
        int result;

        List<int> keyList = new List<int>();
        using (var command = connection.CreateCommand())
        {
          command.CommandText = "SELECT [Key], * FROM ServiceDefinitions";
          using (var reader = command.ExecuteReader())
            while (reader.Read()) keyList.Add((int)reader[0]);
        }

        // 1 <= n <= 100
        // sequential - explicit params; between; tvp
        // random - explicit params; tvp

        for (int numKeys = 0; numKeys < 100; numKeys++)
        {
          for (int iteration = 0; iteration < 100; iteration++)
          {
            var keys = RandomKeys(keyList, numKeys);
          }
        }
      }*/

      if (Debugger.IsAttached) Console.ReadLine();
    }
  }

  class FakeReader : IDataReader
  {
    int count;
    long size;
    long maxSize;

    DataTable schemaTable;

    public FakeReader()
    {
      maxSize = 6L << 30;
    }

    public void Close() { }

    public int Depth
    {
      get { throw new NotImplementedException(); }
    }

    public DataTable GetSchemaTable()
    {
      throw new NotImplementedException(); 
    }

    public bool IsClosed
    {
      get { throw new NotImplementedException(); }
    }

    public bool NextResult()
    {
      throw new NotImplementedException();
    }

    public bool Read()
    {
      count++;
      size += 60;
      return size < maxSize;
    }

    public int RecordsAffected
    {
      get { throw new NotImplementedException(); }
    }

    public void Dispose() { }

    public int FieldCount
    {
      get { return 2; }
    }

    public bool GetBoolean(int i)
    {
      throw new NotImplementedException();
    }

    public byte GetByte(int i)
    {
      throw new NotImplementedException();
    }

    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
    {
      throw new NotImplementedException();
    }

    public char GetChar(int i)
    {
      throw new NotImplementedException();
    }

    public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
    {
      throw new NotImplementedException();
    }

    public IDataReader GetData(int i)
    {
      throw new NotImplementedException();
    }

    public string GetDataTypeName(int i)
    {
      throw new NotImplementedException();
    }

    public DateTime GetDateTime(int i)
    {
      throw new NotImplementedException();
    }

    public decimal GetDecimal(int i)
    {
      throw new NotImplementedException();
    }

    public double GetDouble(int i)
    {
      throw new NotImplementedException();
    }

    public Type GetFieldType(int i)
    {
      throw new NotImplementedException();
    }

    public float GetFloat(int i)
    {
      throw new NotImplementedException();
    }

    public Guid GetGuid(int i)
    {
      throw new NotImplementedException();
    }

    public short GetInt16(int i)
    {
      throw new NotImplementedException();
    }

    public int GetInt32(int i)
    {
      if (i == 0) return count;
      throw new NotImplementedException();
    }

    public long GetInt64(int i)
    {
      throw new NotImplementedException();
    }

    public string GetName(int i)
    {
      throw new NotImplementedException();
    }

    public int GetOrdinal(string name)
    {
      throw new NotImplementedException();
    }

    public string GetString(int i)
    {
      throw new NotImplementedException();
    }

    public object GetValue(int i)
    {
      switch (i)
      {
        case 0: return count;
        case 1: return new string(' ', 60);
        default: throw new InvalidOperationException();
      }
    }

    public int GetValues(object[] values)
    {
      throw new NotImplementedException();
    }

    public bool IsDBNull(int i)
    {
      return false;
    }

    public object this[string name]
    {
      get { throw new NotImplementedException(); }
    }

    public object this[int i]
    {
      get { throw new NotImplementedException(); }
    }
  }

}
