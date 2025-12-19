using System.Text.Json;
using CustomerOrders.Api.Services.DbTools;
using Microsoft.SemanticKernel;
using System.Text.RegularExpressions;


namespace CustomerOrders.Api.Services.Ai.SqlCopilot;

public interface ISqlCopilotService
{
    Task<SqlGenerateResponse> GenerateSqlAsync(string instruction, CancellationToken ct);
    Task<SqlExecuteResponse> ExecuteSqlAsync(SqlExecuteRequest request, CancellationToken ct);
}

public class SqlCopilotService : ISqlCopilotService
{
    private readonly Kernel _kernel;
    private readonly SqlTools _sqlTools;

    public SqlCopilotService(
        Kernel kernel,
        SqlTools sqlTools)
    {
        _kernel = kernel;
        _sqlTools = sqlTools;
    }

    public async Task<SqlGenerateResponse> GenerateSqlAsync(string instruction, CancellationToken ct)
    {
        var table = _sqlTools.ListTablesAsync(ct);

        var products = await _sqlTools.DescribeTableAsync("dbo.Products", ct);
        var oders = await _sqlTools.DescribeTableAsync("dbo.Orders", ct);

        var schemaJson = JsonSerializer.Serialize(new
        {
            table,
            Products= products,
            Orders= oders
        });


        var prompt = """
                You generate ONE SQL Server statement for the given instruction.

                Rules:
                - Output STRICT JSON ONLY with keys: sql, parameters, notes, tableName
                - sql must be ONE statement. No comments. No multiple statements.
                - For parameters, use an object with keys without '@' (example: {"name":"x","price":12.3})
                - Prefer parameterized SQL using @param names.
                - Allowed statements: SELECT, INSERT INTO Products, UPDATE Products, INSERT INTO Orders, Update Orders
                - Use the following logic for statements:
                - If instruction asks to add product: INSERT into Products (Name, Description, Price, Stock)
                - If instruction asks to update product stock, price, name : UPDATE Products SET Stock = @stock, Price = @price, Name = @name WHERE Id = @id
                - if instruction asks to add order: INSERT into Orders (ProductId, Quantity, UnitPrice, CreatedAtUtc) and use Products table to get Price based on ProductId
                - if instruction asks to update order quantity: UPDATE Orders SET Quantity = @quantity WHERE Id = @id
                - If instruction asks to query: SELECT

                Schema (JSON):
                {{$schema}}

                Instruction:
                {{$instruction}}

                Return JSON now.
                """;

            var args = new KernelArguments
            {
                ["schema"] = schemaJson,
                ["instruction"] = instruction
            };

            var result = await _kernel.InvokePromptAsync(prompt, args);

            var raw = StripCodeFences(result.GetValue<string>() ?? ""); //trim code fences if any

            Console.WriteLine("SQL Copilot Raw Response:");
            Console.WriteLine(raw);

            using var doc = JsonDocument.Parse(raw);
            Console.WriteLine("Parsed JSON Response:");
            var root = doc.RootElement;

            var sql = root.GetProperty("sql").GetString() ?? "";
            var notes = root.TryGetProperty("notes", out var notesElem) ? notesElem.GetString() ?? "" : "";
            var tableName = root.TryGetProperty("tableName", out var tableNameElem) ? tableNameElem.GetString() ?? "" : "";


            var parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (root.TryGetProperty("parameters", out var p) && p.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in p.EnumerateObject())
                {
                    parameters[prop.Name] = prop.Value.ValueKind switch
                    {
                        JsonValueKind.Number => prop.Value.TryGetInt64(out var l) ? l :
                                            prop.Value.TryGetDecimal(out var d) ? d :
                                            prop.Value.GetDouble(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        JsonValueKind.String => prop.Value.GetString() ?? "",
                        JsonValueKind.Null => null!,
                        _ => prop.Value.ToString() ?? ""
                    };
                }
            }

            SqlGuardrails.ValidateSingleStatement(sql);
            var kind = SqlGuardrails.DetectKind(sql);
            SqlGuardrails.ValidateAllowedKind(kind);
            SqlGuardrails.ValidateWriteScope(sql);

            return new SqlGenerateResponse(sql, kind, notes, tableName, parameters);


    }

    public async Task<SqlExecuteResponse> ExecuteSqlAsync(SqlExecuteRequest request, CancellationToken ct)
    {
        SqlGuardrails.ValidateSingleStatement(request.Sql);
        var kind = SqlGuardrails.DetectKind(request.Sql);
        SqlGuardrails.ValidateAllowedKind(kind);
        SqlGuardrails.ValidateWriteScope(request.Sql);

        try
        {
           var parameters = request.Parameters ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

              if (kind == "SELECT")
              {
                var rows = await _sqlTools.ExecuteSelectAsync(request.TableName, request.Sql, parameters, ct);
                return new SqlExecuteResponse(true, kind, 0, rows);
              }
              else
              {
                var affected = await _sqlTools.ExecuteNonQueryAsync(request.TableName, request.Sql, parameters, ct);
                return new SqlExecuteResponse(true, kind, affected, null);
              }
        }
        catch (Exception ex)
        {
            Console.WriteLine("SQL Execution Error: " + ex.Message);
            throw new InvalidOperationException("Error executing SQL statement: " + ex.Message, ex);
        }
    }

    static string StripCodeFences(string s)
    {
        if(string.IsNullOrEmpty(s))
            return s;

        s = s.Trim();
        if (s.StartsWith("```"))
        {
            // remove opening ```... line
            s = Regex.Replace(s, @"^\s*```[a-zA-Z0-9_-]*\s*\r?\n", "", RegexOptions.Multiline);

            // remove trailing ```
            s = Regex.Replace(s, @"\r?\n\s*```\s*$", "", RegexOptions.Multiline);
        }

        return s.Trim();
    }
}