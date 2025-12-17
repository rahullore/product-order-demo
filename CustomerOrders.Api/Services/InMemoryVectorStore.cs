using System.ComponentModel.DataAnnotations;
using CustomerOrders.Api.Models;
public interface IInMemoryVectorStore
{
    void UpsertVector(VectorRecord record);
    IReadOnlyList<VectorRecord> GetAllVectors();
}
public class InMemoryVectorStore : IInMemoryVectorStore
{
    private readonly List<VectorRecord> _vectors = new();

   public void UpsertVector(VectorRecord record)
    {
        _vectors.RemoveAll(v => v.Id == record.Id);
        _vectors.Add(record);
    }

    public IReadOnlyList<VectorRecord> GetAllVectors() => _vectors;
}

public static class VectorMath
{
    public static float CosineSimilarity(float[] a, float[] b)
    {
        float dot=0, magA=0, magB=0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }
        return dot / (MathF.Sqrt(magA) * MathF.Sqrt(magB) + 1e-6f);
    }
}