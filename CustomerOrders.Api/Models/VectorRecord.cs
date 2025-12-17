namespace CustomerOrders.Api.Models;
public record VectorRecord(string Id, float[] Vector,string Text, Dictionary<string, string>? Metadata=null);

public record VectorSearchRequest(string Query,
    int TopK = 5
);