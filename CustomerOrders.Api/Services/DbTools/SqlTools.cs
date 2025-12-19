using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace CustomerOrders.Api.Services.DbTools;

public sealed class SqlTools
{
    private readonly string _connectionString;

    public SqlTools(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Sql") 
                            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public async Task<List<string>> ListTablesAsync(CancellationToken ct)
    {
        const string sql = """
            SELECT TABLE_SCHEMA, TABLE_NAME
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_TYPE = 'BASE TABLE' 
            ORDER BY TABLE_SCHEMA, TABLE_NAME;
            """;
        
        var results = new List<string>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            var schema = reader.GetString(0);
            var tableName = reader.GetString(1);
            results.Add($"{schema}.{tableName}");
        }

        return results;
    
    }

    public async Task<List<Dictionary<string, string>>> DescribeTableAsync(string table,CancellationToken ct)
    {
        var parts = table.Split('.', 2);
        var schema = parts.Length == 2 ? parts[0] : "dbo";
        var name = parts.Length == 2 ? parts[1] : parts[0];

        const string sql = """
            SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @TableName
            ORDER BY ORDINAL_POSITION;
            """;

            var cols = new List<Dictionary<string, string>>();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(ct);
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Schema", schema);
            command.Parameters.AddWithValue("@TableName", name);
            await using var reader = await command.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                var col = new Dictionary<string, string>
                {
                    ["COLUMN_NAME"] = reader.GetString(0),
                    ["DATA_TYPE"] = reader.GetString(1),
                    ["IS_NULLABLE"] = reader.GetString(2)
                };
                cols.Add(col);
            }
            return cols;
    }

    public async Task<List<Dictionary<string,object>>> ExecuteSelectAsync(string tableName,string sql,
        Dictionary<string,object> parameters,
        CancellationToken ct)
    {
        var results = new List<Dictionary<string, object?>>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand(sql, connection);

        if (parameters != null)
        {
            await BuildSqlParameters(tableName, command, parameters, ct);
        }

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var row = new Dictionary<string, object?>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.GetValue(i);
            }
            results.Add(row);
        }

        return results;
    }

    public async Task<int> ExecuteNonQueryAsync(string tableName,string sql,
        Dictionary<string,object> parameters,
        CancellationToken ct)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand(sql, connection);

        Console.WriteLine("Executing SQL Non-Query:");
        Console.WriteLine(sql);

        foreach (var param in parameters)
        {
            Console.WriteLine($"Parameter: {param.Key} = {param.Value}");
        }


        if (parameters != null)
        {
            await BuildSqlParameters(tableName, command, parameters, ct);
        }

        return await command.ExecuteNonQueryAsync(ct);
    }

   public async Task BuildSqlParameters(
    string tableName,
    SqlCommand command,
    Dictionary<string, object>? inputParameters,
    CancellationToken ct = default)
    {
        if (inputParameters is null || inputParameters.Count == 0) return;

        var cols = await DescribeTableAsync(tableName, ct);

        var colMap = cols.ToDictionary(
            c => c["COLUMN_NAME"],
            c => (DataType: c["DATA_TYPE"], IsNullable: c["IS_NULLABLE"].Equals("YES", StringComparison.OrdinalIgnoreCase)),
            StringComparer.OrdinalIgnoreCase);

        foreach (var kv in inputParameters)
        {
            var rawKey = kv.Key ?? "";
            var columnKey = rawKey.TrimStart('@');              
            var paramName = rawKey.StartsWith("@") ? rawKey : "@" + rawKey;

            var value = NormalizeSqlValue(kv.Value) ?? DBNull.Value;

            if (colMap.TryGetValue(columnKey, out var colInfo))
            {
                var sqlType = MapSqlType(colInfo.DataType);
                command.Parameters.Add(paramName, sqlType).Value = value;
            }
            else
            {
                // IMPORTANT for SELECT filters not tied to a column name (e.g. @minPrice)
                // Also prevents “param not supplied” errors.
                command.Parameters.AddWithValue(paramName, value);
            }
        }
    }


    private static SqlDbType MapSqlType(string dataType) => dataType.ToLowerInvariant() switch
    {
        "int" => SqlDbType.Int,
        "bigint" => SqlDbType.BigInt,
        "smallint" => SqlDbType.SmallInt,
        "tinyint" => SqlDbType.TinyInt,
        "bit" => SqlDbType.Bit,
        "decimal" or "numeric" => SqlDbType.Decimal,
        "money" => SqlDbType.Money,
        "smallmoney" => SqlDbType.SmallMoney,
        "float" => SqlDbType.Float,
        "real" => SqlDbType.Real,
        "date" => SqlDbType.Date,
        "datetime" => SqlDbType.DateTime,
        "datetime2" => SqlDbType.DateTime2,
        "smalldatetime" => SqlDbType.SmallDateTime,
        "time" => SqlDbType.Time,
        "uniqueidentifier" => SqlDbType.UniqueIdentifier,
        "nvarchar" => SqlDbType.NVarChar,
        "varchar" => SqlDbType.VarChar,
        "nchar" => SqlDbType.NChar,
        "char" => SqlDbType.Char,
        "text" => SqlDbType.Text,
        "ntext" => SqlDbType.NText,
        "varbinary" => SqlDbType.VarBinary,
        "binary" => SqlDbType.Binary,
        _ => SqlDbType.Variant
    };

    private static object? NormalizeSqlValue(object? value)
    {
        if (value is null) return DBNull.Value;

        if (value is JsonElement je)
        {
            return je.ValueKind switch
            {
                JsonValueKind.String => je.TryGetDateTime(out var dt) ? dt : je.GetString() ?? (object)DBNull.Value,
                JsonValueKind.Number => je.TryGetInt32(out var i) ? i
                                    : je.TryGetInt64(out var l) ? l
                                    : je.TryGetDecimal(out var d) ? d
                                    : je.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null or JsonValueKind.Undefined => DBNull.Value,
                JsonValueKind.Object or JsonValueKind.Array => je.GetRawText(), // or throw (safer)
                _ => je.GetRawText()
            };
        }

        return value;
    }

    
}