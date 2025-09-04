namespace API.DTOs;

public class MessageCreateDTO
{
    public required int RecipientId { get; set; }
    public required string Content { get; set; }
}