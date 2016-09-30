using System;
using System.Collections.Generic;

namespace DatabaseServer
{
    public class QueryParser
    {
        public static List<List<object>> ParseQuery(string query)
        {
            if (QueryValidator.CreateIsValid(query))
            {
                return ParseCreate(query);
            }

            if (QueryValidator.SelectAllIsValid(query))
            {
                return ParseSelectAll(query);
            }

            if (QueryValidator.SelectWhereIsValid(query))
            {
                return ParseSelectWhere(query);
            }

            if (QueryValidator.InsertIsValid(query))
            {
                return ParseInsert(query);
            }
            return null;
        }

        // SELECT ALL FROM table_name
        public static List<List<object>> ParseSelectAll(string query)
        {
            var resultSet = new List<List<object>>();

            return resultSet;
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
