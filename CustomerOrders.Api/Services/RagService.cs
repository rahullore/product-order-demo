using System.Text;
using System.Text.RegularExpressions;


namespace CustomerOrders.Api.Services;

public class RagService
{
    private static readonly Regex TokenRx = new(@"[a-z0-9]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static HashSet<string> Token(string s)=>
        TokenRx.Matches(s.ToLowerInvariant())
               .Select(m => m.Value)
               .ToHashSet();

    private static int OverlapScore(HashSet<string> q,string text)
    {
        var t = Token(text);
        var score = 0;
        foreach(var w in q)
        {
            if(t.Contains(w))
            {
                score++;    
            }
        }
        return score;
    }

    public List<RagDocument> Retrieve(string query, IReadOnlyList<RagDocument> docs,int topK)
    {
        var qTokens = Token(query);
        var scored = docs
            .Select(d => new { Doc = d, Score = OverlapScore(qTokens, d.Text) })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .Select(x => x.Doc)
            .ToList();
        return scored;
    
    }

    public string BuildContext(List<RagDocument> top)
    {
        var sb = new StringBuilder();
        for(var i=0 ; i< top.Count; i++)
        {
            sb.AppendLine($"[Document {i+1}] {top[i].Text}");
            sb.AppendLine();
        }
        return sb.ToString();
    }
}