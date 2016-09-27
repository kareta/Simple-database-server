using System;
using System.Collections.Generic;
using System.IO;

namespace DatabaseServer
{
    class DatabaseEngine
    {
        public void CreateIfNotExists(string tablePath, List<Tuple<string, string>> columns)
        {
            if (File.Exists(tablePath)) return;
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

        public void InsertRow(string tablePath, List<object> row)
        {
            using (var tableFile = new FileStream(tablePath, FileMode.Append))
            using (var writer = new BinaryWriter(tableFile))
            {
                RowOperations.WriteRow(writer, row);
            }
        } 

        public List<List<object>> SelectAll(string tablePath)
        {
            var rows = new List<List<object>>();
            var columnsTypes = GetColumnsTypes(tablePath);

            using (var tableFile = new FileStream(tablePath, FileMode.Open))
            using (var reader = new BinaryReader(tableFile))
            { 
                SkipHeader(reader);
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var row = RowOperations.ReadRow(reader, columnsTypes);
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

        public static List<Tuple<string, string>> GetHeader(string tablePath)
        {
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

        public static List<string> GetColumnsTypes(string tablePath)
        {
            var columnsTypes = new List<string>();
            var header = GetHeader(tablePath);
            foreach (var column in header)
            {
                columnsTypes.Add(column.Item2);
            }
            return columnsTypes;
        }

        public static void Main(string[] args)
        {
            var dbDirectoryPath = Environment.GetEnvironmentVariable("USERPROFILE");
            if (dbDirectoryPath != null)
            {
                var tablePath = Path.Combine(dbDirectoryPath, "ooplab2\\Person");
                var de = new DatabaseEngine();

                var columns = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("Name", "string"),
                    new Tuple<string, string>("Surname", "string"),
                    new Tuple<string, string>("Age", "integer"),
                };

                de.CreateIfNotExists(tablePath, columns);
                de.InsertRow(tablePath, new List<object> {"Vasya", "Vasya", 12});
                var all = de.SelectAll(tablePath);
                PrintRows(all);
            }
            Console.ReadLine();
        }

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
