public record VectorRecord(string Id, float[] Vector,string Text, Dictionary<string, string>? Metadata=null);

public record VectorSearchRequest(string Query,
    int TopK = 5,
    string? type = "product",
    string? Intent = null //Cheapest most_expensive, most_ordered
);