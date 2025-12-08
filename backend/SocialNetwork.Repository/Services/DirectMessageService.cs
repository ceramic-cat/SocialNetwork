using SocialNetwork.Entity;
using SocialNetwork.Repository.Interfaces;
using SocialNetwork.Repository.Errors;

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

        public async Task<DirectMessageResult> SendDirectMessageAsync(Guid senderId, Guid receiverId, string content)
        {
            var senderExists = await _userRepository.ExistsAsync(senderId);
            if (!senderExists)
            {
                return DirectMessageResult.Fail(DirectMessageErrors.SenderDoesNotExist);
            }

            var receiverExists = await _userRepository.ExistsAsync(receiverId);
            if (!receiverExists)
            {
                return DirectMessageResult.Fail(DirectMessageErrors.ReceiverDoesNotExist);
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return DirectMessageResult.Fail(DirectMessageErrors.ContentEmpty);
            }

            if (content.Length > MaxMessageLength)
            {
                return DirectMessageResult.Fail(DirectMessageErrors.ContentTooLong);
            }

            var directMessage = new DirectMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            await _dmRepository.AddAsync(directMessage);

            return DirectMessageResult.Ok();
        }
    }
}