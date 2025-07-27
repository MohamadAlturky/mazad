using Mazad.Core.Shared.Entities;

namespace Mazad.Models;

public class Chat : BaseEntity<int>
{
    public int User1Id { get; set; }
    public User User1 { get; set; }

    public int User2Id { get; set; }
    public User User2 { get; set; }
    public List<ChatMessage> Messages { get; set; }
}

public class ChatMessage : BaseEntity<int>
{
    public int ChatId { get; set; }
    public Chat Chat { get; set; }
    public string Message { get; set; }
    public DateTime Date { get; set; }
    public int SenderId { get; set; }
    public User Sender { get; set; }
    public int ReceiverId { get; set; }
    public User Receiver { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadDate { get; set; }
}
