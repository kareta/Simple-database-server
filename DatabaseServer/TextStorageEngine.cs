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

        public void CreateIfNotExists(string tableName, List<Tuple<string, string>> columns)
        {
            if (TableExists(tableName)) return;

            var tablePath = GetTablePath(tableName);

            using (var tableFile = new FileStream(tablePath, FileMode.Create))
            using (var writer = new StreamWriter(tableFile))
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(columns.Count + " ");
                foreach (var column in columns)
                {
                    //write column name
                    stringBuilder.Append(column.Item1 + " ");
                    //write column type
                    stringBuilder.Append(column.Item2 + " ");
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
            return null;
        }    
    }
}
