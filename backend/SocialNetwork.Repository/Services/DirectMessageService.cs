using SocialNetwork.Entity;
using SocialNetwork.Repository.Interfaces;

namespace SocialNetwork.Repository.Services
{
    public class DirectMessageService : IDirectMessageService
    {
        private readonly IDirectMessageRepository _dmRepository;
        private readonly IUserRepository _userRepository;
        private const int MaxMessageLength = 280;

        public DirectMessageService(
            IDirectMessageRepository dmRepository,
            IUserRepository userRepository)
        {
            _dmRepository = dmRepository;
            _userRepository = userRepository;
        }

        public async Task<DirectMessageResult> SendDirectMessageAsync(Guid senderId, Guid receiverId, string message)
        {
            // Validate receiver exists
            var receiverExists = await _userRepository.ExistsAsync(receiverId);
            if (!receiverExists)
            {
                return DirectMessageResult.Fail("Receiver does not exist.");
            }

            // Validate message
            if (string.IsNullOrWhiteSpace(message))
            {
                return DirectMessageResult.Fail("Message cannot be empty.");
            }

            if (message.Length > MaxMessageLength)
            {
                return DirectMessageResult.Fail($"Message cannot be longer than {MaxMessageLength} characters.");
            }

            // Create and save direct message
            var directMessage = new DirectMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            await _dmRepository.AddAsync(directMessage);

            return DirectMessageResult.Ok();
        }
    }
}

