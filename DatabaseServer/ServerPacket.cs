using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseServer
{
    class ServerPacket
    {
        public List<Tuple<string, string>> Header { get; set; }
        public List<List<object>> Rows { get; set; }

        public ServerPacket(List<Tuple<string, string>> header, List<List<object>> rows)
        {
            Header = header;
            Rows = rows;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var column in Header)
            {
                //column name
                builder.Append(column.Item1 + " ");
                //column type
                builder.Append(column.Item2 + " ");
            }
            builder.Append("\n");

            foreach (var row in Rows)
            {
                foreach (var field in row)
                {
                    builder.Append(field + " ");
                }
                builder.Append("\n");
            }
            return builder.ToString();     
        }
    }
}
