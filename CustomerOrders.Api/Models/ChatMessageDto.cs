namespace CustomerOrders.Api.Models;

public sealed class ChatMessageDto
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public ChatMessageDto(string role, string content)
    {
        Role = role;
        Content = content;
    }
}   
    
public sealed record ChatRequestDto(IReadOnlyList<ChatMessageDto> Messages);

public sealed record ChatResponseDto(string Reply);