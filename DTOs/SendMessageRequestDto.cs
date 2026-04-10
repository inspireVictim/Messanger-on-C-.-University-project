using System.ComponentModel.DataAnnotations;

namespace messanger.DTOs
{
    public class SendMessageRequestDto
    {
        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        [MinLength(1)]
        public string Text { get; set; } = string.Empty;
    }
}
