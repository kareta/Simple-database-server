using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.Environment;

namespace DatabaseServer
{
    public class TextStorageEngine
    {
        public string DatabaseDirectory { get; } =
            Path.Combine(GetEnvironmentVariable("USERPROFILE"), "lab2");

        public bool TableExists(string tableName) => File.Exists(GetTablePath(tableName));

        public string GetTablePath(string tableName) => Path.Combine(DatabaseDirectory, tableName);

        public void CreateIfNotExists(string tableName, string columns)
        {
            if (TableExists(tableName)) return;

            var tablePath = GetTablePath(tableName);

            var splittedColumns = columns.Split();

            using (var tableFile = new FileStream(tablePath, FileMode.Create))
            using (var writer = new StreamWriter(tableFile))
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(splittedColumns.Length / 2 + " ");
                for (var i = 0; i < splittedColumns.Length; i+=2)
                {
                    //write column name
                    stringBuilder.Append(splittedColumns[i] + " ");
                    //write column type
                    stringBuilder.Append(splittedColumns[i + 1] + " ");
                }
                writer.WriteLine(stringBuilder.ToString().Trim());
            }
        }

        public void InsertRow(string tableName, string row)
        {
            if (!TableExists(tableName)) return;

            var tablePath = GetTablePath(tableName);
            using (var tableFile = new FileStream(tablePath, FileMode.Append))
            using (var writer = new StreamWriter(tableFile))
            {
                writer.WriteLine(row);
            }
        }

        public List<string> SelectAll(string tableName)
        {
            if (!TableExists(tableName)) return null;

            var tablePath = GetTablePath(tableName);
            using (var tableFile = new FileStream(tablePath, FileMode.Open))
            using (var reader = new StreamReader(tableFile))
            {
                SkipHeader(reader); 
                return new List<string>(reader.ReadToEnd().Split());                          
            }
        }

        private static void SkipHeader(StreamReader reader)
        {
            //Make sure file position is at the begining
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            //Skip columns number
            reader.ReadLine();
            //Skip columns names and types
            reader.ReadLine();

        }
     
    }
}
