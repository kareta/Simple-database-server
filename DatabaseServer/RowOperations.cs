using System;
using System.Collections.Generic;
using System.IO;

namespace DatabaseServer
{
    class RowOperations
    {
        public static void WriteRow(BinaryWriter writer, List<object> row)
        {
            foreach (var field in row)
            {
                switch (field.GetType().ToString())
                {
                    case "System.Int32":
                        writer.Write((int)field);
                        break;
                    case "System.Double":
                        writer.Write((double)field);
                        break;
                    case "System.String":
                        writer.Write((string)field);
                        break;
                    default:
                        throw new Exception("No such type");
                }
            }
        }

        public static List<object> ReadRow(BinaryReader reader, List<string> columnTypes)
        {
            var row = new List<object>();
            foreach (var columnType in columnTypes)
            {
                switch (columnType)
                {
                    case "integer":
                        row.Add(reader.ReadInt32());
                        break;
                    case "string":
                        row.Add(reader.ReadString());
                        break;
                    case "double":
                        row.Add(reader.ReadDouble());
                        break;
                    default:
                        throw new Exception("No such type");
                }
            }
            return row;
        }
    }
}
