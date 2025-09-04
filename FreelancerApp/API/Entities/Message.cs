namespace API.Entities;

public class Message
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime MessageSent { get; set; } = DateTime.UtcNow;
    public DateTime? DateRead { get; set; }

    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }

    // Links
    public int SenderId { get; set; }
    public AppUser Sender { get; set; } = null!;
    public int RecipientId { get; set; }
    public AppUser Recipient { get; set; } = null!;
    public int ConversationId { get; set; }
    public ProjectConversation Conversation { get; set; } = null!;
}