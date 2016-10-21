using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseServer
{
    public class ServerPacket
    {
        public List<string> Rows { get; set; }

        public ServerPacket(List<string> rows)
        {
            Rows = rows;
        }

        public override string ToString()
        {
            if (Rows == null)
            {
                return "";
            }

            var rows = string.Join("\n", Rows.ToArray());
            return rows;     
        }
    }
}
