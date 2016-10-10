using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DatabaseServer
{
    public class QueryParser
    {
        TextStorageEngine storageEngine = new TextStorageEngine();

        public List<string> ParseQuery(string query)
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
                ParseInsert(query);
            }
            return null;
        }

        // SELECT ALL FROM table_name
        public List<string> ParseSelectAll(string query)
        {
            string tableName = query.Split()[3];
            return storageEngine.SelectAll(tableName);
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
            //var row = string.Join(" ", regexColumns.Matches(query));
            var matchCollection = regexColumns.Matches(query);
            var row = string.Join(" ", matchCollection.Cast<Match>().Select(m => m.Value));
            storageEngine.InsertRow(tableName, row);
            Console.WriteLine(row);
        }

        //CREATE table_name column type column type
        public List<string> ParseCreate(string query)
        {
            return null;
        }

        public static void Main(string[] args)
        {
            var queryParser = new QueryParser();
            queryParser.ParseInsert("INSERT lol '''fw eew f''' age 32.4 INTO vasya");
            Console.ReadLine();
        }
    }
}
