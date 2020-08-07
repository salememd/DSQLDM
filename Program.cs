using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static DSQLDM.Models;

namespace DSQLDM
{
    class Models
    {
        public class Schema
        {
            public string Name { get; set; }
        }

        public class Table
        {
            public string Name { get; set; }
            public Schema Schema { get; set; }
        }

        public class Column
        {
            public string Name { get; set; }
            public int OrdinalPosition { get; set; }
            public string DataType { get; set; }
            public int CharMaxLength { get; set; }
            public Table ClTable { get; set; }
        }
    }
    class Program
    {

        static async Task Main(string[] args)
        {
            Column p = await new DB<Column>("Server=DEX\\SQLEXPRESS;Database=master;Trusted_Connection=True")
                .QuerySingle(@"SELECT TABLE_SCHEMA AS [ClTable.Schema.Name], 
                     TABLE_NAME AS [ClTable.Name],
                     COLUMN_NAME AS [Name], 
                     ORDINAL_POSITION AS [OrdinalPosition],
                     DATA_TYPE AS [DataType],
                     CHARACTER_MAXIMUM_LENGTH AS [CharMaxLength]  FROM  INFORMATION_SCHEMA.COLUMNS", new {  });
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(p));
          }
    }
}
