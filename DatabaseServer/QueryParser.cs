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
            return null;
        }

        //INSERT column value column value INTO table_name
        public void ParseInsert(string query)
        {
            var regexTableName = new Regex(@"INTO \w{1,256}");
            var tableName = regexTableName.Matches(query)[0].ToString().Split()[1];
            
            var regexColumns = new Regex(@"(\w+ ('''.*'''))|(\w+ ([0-9]*[.])?[0-9]+)+");
            var matchCollection = regexColumns.Matches(query);
            var row = string.Join(" ", matchCollection.Cast<Match>().Select(m => m.Value));
            _storageEngine.InsertRow(tableName, row);
        }

        //CREATE table_name WITH column type column type
        public void ParseCreate(string query)
        {
            //WITH(\w+ \w+)+
            var splittedQuery = query.Split();
            var tableName = splittedQuery[1];
            var segment = new ArraySegment<string>(splittedQuery, 3, splittedQuery.Length - 3);
            var columns = string.Join(" ", segment);
            Console.WriteLine(columns);
            _storageEngine.CreateIfNotExists(tableName, columns);
        }

        public static void Main(string[] args)
        {
            var queryParser = new QueryParser();
            //queryParser.ParseInsert("INSERT lol '''fw eew f''' age 32.4 INTO vasya");
            queryParser.ParseCreate("CREATE test WITH number integer text string");
            Console.ReadLine();
        }
    }
}
