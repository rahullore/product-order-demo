using System.Text.RegularExpressions;

namespace CustomerOrders.Api.Services.Ai.SqlCopilot;


public static class SqlGuardrails
{   private static readonly Regex Dangerous =
        new(@"(\bDROP\b|\bALTER\b|\bTRUNCATE\b|\bMERGE\b|\bGRANT\b|\bREVOKE\b|\bEXEC\b|\bEXECUTE\b|\bxp_)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

   public static void ValidateSingleStatement(string sql)
   {
       if(string.IsNullOrWhiteSpace(sql))
           throw new ArgumentException("SQL statement is empty.");

           var trimmed = sql.Trim();
           var semicolons = trimmed.Count(c => c == ';');

        if(semicolons > 1)
            throw new ArgumentException("Multiple SQL statements are not allowed.");

        if(Dangerous.IsMatch(trimmed))
            throw new ArgumentException("The SQL statement contains potentially dangerous operations.");
        
        if(trimmed.StartsWith("BEGIN", StringComparison.OrdinalIgnoreCase) &&
           trimmed.EndsWith("END", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Batch statements are not allowed.");
        }

        if(trimmed.Contains("--") || trimmed.Contains("/*"))
            throw new ArgumentException("SQL comments are not allowed.");
   }

   public static string DetectKind(string sql)
   {
       var trimmed = sql.TrimStart();

       if (trimmed.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
           return "SELECT";
       if (trimmed.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
           return "INSERT";
       if (trimmed.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase))
           return "UPDATE";
       if (trimmed.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
           return "DELETE";

       return "UNKNOWN";
       
   }

   public static void ValidateAllowedKind(string kind)
   {
       var allowedKinds = new HashSet<string> { "SELECT", "INSERT", "UPDATE", "DELETE" };

       if (!allowedKinds.Contains(kind.ToUpperInvariant()))
           throw new ArgumentException($"SQL statement of kind '{kind}' is not allowed.");
   }

   public static void ValidateWriteScope(string sql)
    {
        string s = sql.TrimStart();

        if(s.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase) ||
           s.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) ||
           s.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
        {
            if(!sql.Contains("Products", StringComparison.OrdinalIgnoreCase) &&
               !sql.Contains("Orders", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Write operations are only allowed on Products and Orders tables.");
            }
        }
    }
}