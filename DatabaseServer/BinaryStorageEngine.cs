using System;
using System.Collections.Generic;
using System.IO;
using static System.Environment;

namespace DatabaseServer
{
    public class BinaryStorageEngine
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
            using (var writer = new BinaryWriter(tableFile))
            {
                writer.Write(columns.Count);
                foreach (var column in columns)
                {
                    //write column name
                    writer.Write(column.Item1);
                    //write column type
                    writer.Write(column.Item2);
                }
            }
        }

        public void InsertRow(string tableName, List<object> row)
        {
            if (!TableExists(tableName)) return;

            var tablePath = GetTablePath(tableName);
            using (var tableFile = new FileStream(tablePath, FileMode.Append))
            using (var writer = new BinaryWriter(tableFile))
            {
                BinaryRowOperations.WriteRow(writer, row);
            }
        }

        public List<List<object>> SelectAll(string tableName)
        {
            if (!TableExists(tableName)) return null;

            var tablePath = GetTablePath(tableName);

            var rows = new List<List<object>>();
            var columnsTypes = GetColumnsTypes(tablePath);

            using (var tableFile = new FileStream(tablePath, FileMode.Open))
            using (var reader = new BinaryReader(tableFile))
            {
                SkipHeader(reader);
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var row = BinaryRowOperations.ReadRow(reader, columnsTypes);
                    rows.Add(row);
                }
            }

            return rows;
        }

        private static void SkipHeader(BinaryReader reader)
        {
            //Make sure file position is at the begining
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            var columnsNumber = reader.ReadInt32();
            for (var i = 0; i < columnsNumber; i++)
            {
                //Skip column name
                reader.ReadString();

                //Skip column type
                reader.ReadString();
            }
        }

        public List<Tuple<string, string>> GetHeader(string tableName)
        {
            if (!TableExists(tableName)) return null;

            var tablePath = GetTablePath(tableName);

            var header = new List<Tuple<string, string>>();

            using (var tableFile = new FileStream(tablePath, FileMode.Open))
            using (var reader = new BinaryReader(tableFile))
            {
                var columnsNumber = reader.ReadInt32();

                for (var i = 0; i < columnsNumber; i++)
                {
                    var columnName = reader.ReadString();
                    var columnType = reader.ReadString();
                    var columnInfo = new Tuple<string, string>(columnName, columnType);
                    header.Add(columnInfo);
                }
            }
            return header;
        }

        public List<string> GetColumnsTypes(string tableName)
        {
            if (!TableExists(tableName)) return null;

            var tablePath = GetTablePath(tableName);

            var columnsTypes = new List<string>();
            var header = GetHeader(tablePath);
            foreach (var column in header)
            {
                columnsTypes.Add(column.Item2);
            }
            return columnsTypes;
        }

        /*public static void Main(string[] args)
        {
            var storageEngine = new BinaryStorageEngine();

            var columns = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Name", "string"),
                new Tuple<string, string>("Surname", "string"),
                new Tuple<string, string>("Age", "integer"),
            };

            storageEngine.CreateIfNotExists("Student", columns);
            storageEngine.InsertRow("Student", new List<object> { "Vasya", "Vasya", 12 });
            var all = storageEngine.SelectAll("Student");
            PrintRows(all);

            Console.ReadLine();
        }*/

        public static void PrintRows(List<List<object>> rows)
        {
            foreach (var row in rows)
            {
                foreach (var field in row)
                {
                    Console.Write(field + " ");
                }
                Console.WriteLine();
            }
        }

        public static void PrintRow(List<object> row)
        {
            foreach (object field in row)
            {
                Console.Write(field + " ");
            }
            Console.WriteLine();
        }
    }
}
