# DSQLDM
Mapping Dapper query result set to a domain model

Example: - (Available in Program.cs)

            Column p = await new DB<Column>("Server=DEX\\SQLEXPRESS;Database=master;Trusted_Connection=True")
                .QuerySingle(@"SELECT TABLE_SCHEMA AS [ClTable.Schema.Name], 
                     TABLE_NAME AS [ClTable.Name],
                     COLUMN_NAME AS [Name], 
                     ORDINAL_POSITION AS [OrdinalPosition],
                     DATA_TYPE AS [DataType],
                     CHARACTER_MAXIMUM_LENGTH AS [CharMaxLength]  FROM  INFORMATION_SCHEMA.COLUMNS", new {  });
                     
                      Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(p));
                 
                      
Result: -

<code>
     {"Name":"xserver_name","OrdinalPosition":1,"DataType":"varchar","CharMaxLength":30,"ClTable":{"Name":"spt_fallback_db","Schema":{"Name":"dbo"}}}
</code>
