namespace CustomerOrders.Api.Services.Ai.SqlCopilot;

public record SqlGenerateRequest(string Instruction);

public record SqlExecuteRequest(string TableName,string Sql,
    Dictionary<string,object>? Parameters = null
);

public record SqlGenerateResponse(string Sql,
    string Kind,
    string Notes,
    string TableName,
    Dictionary<string,object>? Parameters = null
);

public record SqlExecuteResponse(bool Success,
    string Kind,
    int AffectedRows = 0,
    List<Dictionary<string, object>>? Rows = null
);