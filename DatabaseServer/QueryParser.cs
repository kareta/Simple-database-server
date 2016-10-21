using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DatabaseServer
{
    public class QueryParser
    {
        private TextStorageEngine _storageEngine = new TextStorageEngine();

        public List<string> ParseQuery(string query)
        {
            if (QueryValidator.CreateIsValid(query))
            {
                ParseCreate(query);
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
                ParseInsert(query);
            }
            return null;
        }

        // SELECT ALL FROM table_name
        public List<string> ParseSelectAll(string query)
        {
            var tableName = query.Split()[3];
            return _storageEngine.SelectAll(tableName);
        }

        // SELECT WHERE column value FROM table_name
        public List<string> ParseSelectWhere(string query)
        {
            var queryParts = query.Split();
            var column = queryParts[2];
            var value = queryParts[3];
            var tableName = queryParts[5];

            return _storageEngine.SelectWhere(tableName, column, value);
        }

        //INSERT column value column value INTO table_name
        public void ParseInsert(string query)
        {
            var splittedQuery = query.Split(' ');
            var tableName = splittedQuery[splittedQuery.Length - 1];

            var columnsPart = new ArraySegment<string>(splittedQuery, 1, splittedQuery.Length - 3);

            var row = string.Join(" ", columnsPart);
            _storageEngine.InsertRowUnique(tableName, row);
        }

        //CREATE table_name WITH column type column type
        public void ParseCreate(string query)
        {
            var splittedQuery = query.Split();
            var tableName = splittedQuery[1];
            var columnsPart = new ArraySegment<string>(splittedQuery, 3, splittedQuery.Length - 3);
            var queryPart = string.Join(" ", columnsPart);
            var columnsAndUnique = queryPart.Split(new string[] { "UNIQUE" }, StringSplitOptions.None);
            var columns = columnsAndUnique[0];

            if (columnsAndUnique.Length == 2)
            {
                var unique = columnsAndUnique[1];
                _storageEngine.CreateIfNotExists(tableName, columns.Trim(), unique.Trim());
            }
            else
            {
                _storageEngine.CreateIfNotExists(tableName, columns, "");
            }
            
        }

        public static void Main(string[] args)
        {
            var queryParser = new QueryParser();
            queryParser.ParseCreate("CREATE test WITH number integer text string UNIQUE number");
            queryParser.ParseInsert("INSERT 32.4 '''fergge''' INTO test");
            Console.ReadLine();
        }
    }
}
