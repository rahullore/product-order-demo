namespace CustomerOrders.Api.Services;

public record RagDocument(string Id, string Text, Dictionary<string, string>? Metadata=null);

public class RagStore
{
    private readonly List<RagDocument> _doc = new();
    public IReadOnlyList<RagDocument> GetAll() => _doc;

    public void UpsertMany(IEnumerable<RagDocument> docs)
    {
        var map = _doc.ToDictionary(d => d.Id);
        
        foreach(var d in docs)
        {
            map[d.Id] = d;
        }
        _doc.Clear();
        _doc.AddRange(map.Values);

    }

    public void Clear() => _doc.Clear();
}