using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using messanger.Data;
using messanger.DTOs;
using messanger.Models;
using messanger.Services;

namespace messanger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly EncryptionService _encryptionService;

        public MessagesController(AppDbContext dbContext, EncryptionService encryptionService)
        {
            _dbContext = dbContext;
            _encryptionService = encryptionService;
        }

        [HttpPost]
        public async Task<ActionResult<SendMessageResponseDto>> SendMessageAsync([FromBody] SendMessageRequestDto request)
        {
            if (request.SenderId <= 0 || request.ReceiverId <= 0)
            {
                return BadRequest("SenderId and ReceiverId must be positive.");
            }

            if (request.SenderId == request.ReceiverId)
            {
                return BadRequest("SenderId and ReceiverId cannot be the same.");
            }

            var message = new Message
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                EncryptedText = _encryptionService.Encrypt(request.Text),
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();

            return Ok(new SendMessageResponseDto { Id = message.Id });
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MessageResponseDto>>> GetConversationAsync([FromQuery] int user1, [FromQuery] int user2)
        {
            if (user1 <= 0 || user2 <= 0)
            {
                return BadRequest("user1 and user2 must be positive.");
            }

            if (user1 == user2)
            {
                return BadRequest("user1 and user2 cannot be the same.");
            }

            var messages = await _dbContext.Messages
                .AsNoTracking()
                .Where(m =>
                    (m.SenderId == user1 && m.ReceiverId == user2) ||
                    (m.SenderId == user2 && m.ReceiverId == user1))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            var response = messages
                .Select(m => new MessageResponseDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Text = _encryptionService.Decrypt(m.EncryptedText),
                    CreatedAt = m.CreatedAt
                })
                .ToList();

            return Ok(response);
        }
    }
}
