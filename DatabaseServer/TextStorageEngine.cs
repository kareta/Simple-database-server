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

        public string GetTablePath(string tableName)
        {
            return Path.Combine(DatabaseDirectory, tableName.Trim());
        }

        public void CreateIfNotExists(string tableName, string columns, string uniqueColumns)
        {
            if (TableExists(tableName)) return;

            var tablePath = GetTablePath(tableName);

            var splittedColumns = columns.Split();

            using (var tableFile = new FileStream(tablePath, FileMode.Create))
            using (var writer = new StreamWriter(tableFile))
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(splittedColumns.Length / 2 + "\n");
                for (var i = 0; i < splittedColumns.Length - 1; i+=2)
                {
                    //write column name
                    stringBuilder.Append(splittedColumns[i] + " ");
                    //write column type
                    stringBuilder.Append(splittedColumns[i + 1] + " ");
                }
                writer.WriteLine(stringBuilder.ToString().Trim());
                writer.WriteLine(uniqueColumns);
            }
        }

        public void DeleteTable(string tableName )
        {
            if (!TableExists(tableName)) return;

            var tablePath = GetTablePath(tableName);
            File.Delete(tablePath);
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

        public void InsertRowUnique(string tableName, string row)
        {
            if (!TableExists(tableName)) return;

            var tablePath = GetTablePath(tableName);
            

            var indeces = GetUniqueIndeces(tableName);

            File.Move(tablePath, tablePath + "_temp");
            using (var tempTableFile = new FileStream(tablePath + "_temp", FileMode.Open))
            using (var reader = new StreamReader(tempTableFile))
            using (var tableFile = new FileStream(tablePath, FileMode.Create))
            using (var writer = new StreamWriter(tableFile))
            {
                var values = row.Split(' ');

                //Write columns number
                writer.WriteLine(reader.ReadLine());
                //Write columns names and types
                writer.WriteLine(reader.ReadLine());
                //Write unique columns
                writer.WriteLine(reader.ReadLine());

                
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var currentValues = line.Split(' ');
                    var foundUnique = false;
                    Console.WriteLine(indeces.Count);
                    foreach (var index in indeces)
                    {
                        if (currentValues[index] == values[index])
                        {
                            foundUnique = true;
                            break;
                        }
                    }

                    if (foundUnique)
                    {
                        continue;
                    }
                    writer.WriteLine(line);
                }
                writer.WriteLine(row);
            }

            File.Delete(tablePath + "_temp");
        }

        public List<string> SelectAll(string tableName)
        {
            if (!TableExists(tableName)) return null;

            var tablePath = GetTablePath(tableName);
            using (var tableFile = new FileStream(tablePath, FileMode.Open))
            using (var reader = new StreamReader(tableFile))
            {
                SkipHeader(reader);
                return new List<string>(reader.ReadToEnd().Split('\n'));                          
            }
        }

        public List<string> SelectWhere(string tableName, string column, string value)
        {
            if (!TableExists(tableName)) return null;

            var rows = new List<string>();

            var tablePath = GetTablePath(tableName);
            using (var tableFile = new FileStream(tablePath, FileMode.Open))
            using (var reader = new StreamReader(tableFile))
            {
                SkipHeader(reader);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains($"{column} {value}"))
                    {
                        rows.Add(line);
                    }
                }
            }

            return rows;
        }

        private List<string> GetColumnsNames(string tableName)
        {
            var columnsNames = new List<string>();

            if (!TableExists(tableName)) return null;

            var tablePath = GetTablePath(tableName);
            using (var tableFile = new FileStream(tablePath, FileMode.Open))
            using (var reader = new StreamReader(tableFile))
            {
                //Skip columns number
                reader.ReadLine();
                //Get columns names and types
                var columns = reader.ReadLine();

                if (columns == null) return columnsNames;

                var columnsData = columns.Split(' ');
                for (var i = 0; i < columnsData.Length; i += 2)
                {
                    columnsNames.Add(columnsData[i]);
                }
            }
            return columnsNames;
        }

        private List<string> GetUniqueColumns(string tableName)
        {
            var columnsNames = new List<string>();
            
            if (!TableExists(tableName)) return null;
            var tablePath = GetTablePath(tableName);
            using (var tableFile = new FileStream(tablePath, FileMode.Open))
            using (var reader = new StreamReader(tableFile))
            {
                //Skip columns number
                reader.ReadLine();
                //Skip columns names and types
                reader.ReadLine();
                //Get unique columns
                var uniqueColumns = reader.ReadLine();
                if (uniqueColumns == null) return columnsNames;

                var columnsData = uniqueColumns.Split(' ');
                foreach (string uniqueColumn in columnsData)
                {
                    columnsNames.Add(uniqueColumn);
                }
            }
            return columnsNames;
        }

        private List<int> GetUniqueIndeces(string tableName)
        {
            var columnsNames = GetColumnsNames(tableName);
            var uniqueColumns = GetUniqueColumns(tableName);

            var indeces = new List<int>();
            
            if (uniqueColumns == null)
            {
                return indeces;
            }

            foreach (var uniqueColumn in uniqueColumns)
            {
                var index = columnsNames.IndexOf(uniqueColumn);
                indeces.Add(index);
            }
            return indeces;
        }

        private static void SkipHeader(StreamReader reader)
        {
            //Make sure file position is at the begining
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            //Skip columns number
            reader.ReadLine();
            //Skip columns names and types
            reader.ReadLine();
            //Skip unique columns
            reader.ReadLine();
        }    
    }
}
