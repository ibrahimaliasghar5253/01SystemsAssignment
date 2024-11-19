using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Select_Parser___01Systems_Assignment
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var parser = new SQLParser();
            var testQueries = new[]
            {
            "SELECT Col1, Col2, Col3, Col4, Col5 FROM TABLE1 WHERE Col1 = '100'",
            "SELECT Col1, Col2, Col3, Col4, Col5 FROM TABLE1 WHERE Col1 >< '100'",
            "SELECT Col1, Col 2, Col3, Col4, Col5 FROM TABLE1 WEHERE Col2 = '100'",
            "SELECT Col1, Col2, Col3, Col4, Col'5 FROM TABLE1 WEHERE Col1 >< '100'"
        };

            foreach (var query in testQueries)
            {
                Console.WriteLine($"\nTesting query: {query}");
                var result = parser.ParseQuery(query);
                Console.WriteLine($"Is Valid: {result.IsValid}");
                Console.WriteLine($"Message: {result.Message}");
            }
            Console.ReadLine();
        }
    }
}
