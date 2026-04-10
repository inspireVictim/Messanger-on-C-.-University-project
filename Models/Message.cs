namespace messanger.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string EncryptedText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
