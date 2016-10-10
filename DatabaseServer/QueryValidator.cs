using System.Text.RegularExpressions;

namespace DatabaseServer
{
    public class QueryValidator
    {
        public static string SelectAllPattern { get; } = 
            @"SELECT ALL FROM \w{1,256}";

        public static string InsertPattern { get; } =
            @"INSERT (\w+ ('''.*'''))|(\w+ ([0-9]*[.])?[0-9]+)+ INTO \w{1,256}";

        public static string CreatePattern { get; } =
            @"CREATE \w{1,256}";

        public static string SelectWherePattern { get; } =
            @"SELECT WHERE (\w+ ('''.*'''))|(\w+ ([0-9]*[.])?[0-9]+) FROM \w{1,256}";

        //SELECT ALL FROM table_name
        public static bool SelectAllIsValid(string query) => 
            Regex.IsMatch(query, SelectAllPattern);

        // SELECT WHERE column value FROM table_name
        public static bool SelectWhereIsValid(string query) => 
            Regex.IsMatch(query, SelectWherePattern);

        //INSERT column value column value INTO table_name
        public static bool InsertIsValid(string query) => 
            Regex.IsMatch(query, InsertPattern);

        //CREATE table_name column type column type
        public static bool CreateIsValid(string query) => 
            Regex.IsMatch(query, CreatePattern);
    }
}
