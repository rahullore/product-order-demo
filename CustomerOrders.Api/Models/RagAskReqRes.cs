namespace CustomerOrders.Api.Models;

public record RagAskRequest(string Question, int? TopK = 5);

public record RagAskResponse(string Answer, int TopK,IReadOnlyList<object> Sources);