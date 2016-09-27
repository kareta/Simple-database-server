
using System;
using System.Collections.Generic;

namespace DatabaseServer
{
    class QueryParser
    {
        public static DatabaseEngine de = new DatabaseEngine();

        public static List<List<object>> ParseQuery(string query)
        {
            var splittedQuery = query.Split(' ');
            var operationName = splittedQuery[0];
            switch (operationName)
            {
                case "SELECT":
                    return ParseSelect(query);
                case "INSERT":
                    return ParseInsert(query);
                case "CREATE":
                    return ParseCreate(query);
                default:
                    throw new Exception("Unsupported query");
            }
        }

        public static List<List<object>> ParseSelect(string query)
        {
            var splittedQuery = query.Split(' ');
            var operationName = splittedQuery[1];
            switch (operationName)
            {
                case "ALL":
                    return ParseSelectAll(query);
                case "WHERE":
                    return ParseSelectWhere(query);
                default:
                    throw new Exception("Unsupported query");
            }

        }

        // SELECT ALL FROM table_name
        public static List<List<object>> ParseSelectAll(string query)
        {
            return null;
        }

        // SELECT WHERE column value FROM table_name
        public static List<List<object>> ParseSelectWhere(string query)
        {
            return null;
        }

        //INSERT column value column value INTO table_name
        public static List<List<object>> ParseInsert(string query)
        {
            return null;
        }

        //CREATE table_name column type column type
        public static List<List<object>> ParseCreate(string query)
        {
            return null;
        }
    }
}
