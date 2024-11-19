using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Select_Parser___01Systems_Assignment
{
    public class SQLParser
    {
        private readonly HashSet<string> validOperators = new HashSet<string> { "=", "<", ">", ">=", "<=", "<>" };
        private readonly HashSet<string> keywords = new HashSet<string> { "SELECT", "FROM", "WHERE" };

        public class ParseResult
        {
            public bool IsValid { get; set; }
            public string Message { get; set; }

            public ParseResult(bool isValid, string message)
            {
                IsValid = isValid;
                Message = message;
            }
        }

        public ParseResult ParseQuery(string query)
        {
            try
            {
                // Basic cleanup
                query = query.Trim();

                // Check basic structure
                if (!query.ToUpper().StartsWith("SELECT"))
                    return new ParseResult(false, "Query must start with SELECT");

                // Split into main parts
                var fromParts = query.Split(new[] { " FROM " }, StringSplitOptions.None);
                if (fromParts.Length != 2)
                    return new ParseResult(false, "Invalid query structure. Must contain FROM");

                var selectPart = fromParts[0];
                var fromWherePart = fromParts[1];


                
                // Validate SELECT part
                var columnsResult = ValidateColumns(selectPart.Substring(6).Trim());
                if (!columnsResult.IsValid)
                    return columnsResult;

                // Split FROM and WHERE parts
                var tableWhereParts = fromWherePart.Split(new[] { " WHERE " }, StringSplitOptions.None);
                if (tableWhereParts.Length > 2)
                    return new ParseResult(false, "Multiple WHERE clauses found");

                // Validate table name
                var tableName = tableWhereParts[0].Trim();
                if (!IsValidIdentifier(tableName))
                    return new ParseResult(false, $"Invalid table name: {tableName}");

                // If WHERE clause exists, validate it
                if (tableWhereParts.Length == 2)
                {
                    var whereResult = ValidateWhere(tableWhereParts[1], columnsResult.Columns);
                    if (!whereResult.IsValid)
                        return whereResult;
                }

                return new ParseResult(true, "Query is valid");
            }
            catch (Exception ex)
            {
                return new ParseResult(false, $"Error parsing query: {ex.Message}");
            }
        }

        private class ColumnValidationResult : ParseResult
        {
            public List<string> Columns { get; set; }

            public ColumnValidationResult(bool isValid, string message, List<string> columns = null)
                : base(isValid, message)
            {
                Columns = columns;
            }
        }

        private ColumnValidationResult ValidateColumns(string columnsStr)
        {
            var columns = columnsStr.Split(',').Select(c => c.Trim()).ToList();
            var validatedColumns = new List<string>();

            foreach (var col in columns)
            {
                if (!IsValidIdentifier(col))
                    return new ColumnValidationResult(false, $"Invalid column name: {col}");
                validatedColumns.Add(col);
            }

            return new ColumnValidationResult(true, "Valid columns", validatedColumns);
        }

        private bool IsValidIdentifier(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                return false;

            return !identifier.Contains('\'') && !identifier.Contains(' ');
        }

        private ParseResult ValidateWhere(string whereClause, List<string> columns)
        {
            var parts = whereClause.Trim().Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                return new ParseResult(false, "Invalid WHERE clause structure");

            var column = parts[0];
            var operation = parts[1];
            var value = parts[2];

            // Check if column exists
            if (!columns.Contains(column))
                return new ParseResult(false, $"Column in WHERE clause not found: {column}");

            // Check operator
            if (!validOperators.Contains(operation))
                return new ParseResult(false, $"Invalid operator in WHERE clause: {operation}");

            // Check value type based on column position
            // Assuming alternating columns (odd indices are integer, even are varchar)
            var colIndex = columns.IndexOf(column);
            var isIntegerColumn = colIndex % 2 == 0;

            // Remove quotes if present
            value = value.Trim('\'');

            if (isIntegerColumn)
            {
                if (!int.TryParse(value, out _))
                    return new ParseResult(false, $"Column {column} expects integer value, got string");
            }

            return new ParseResult(true, "Valid WHERE clause");
        }
    }
}
