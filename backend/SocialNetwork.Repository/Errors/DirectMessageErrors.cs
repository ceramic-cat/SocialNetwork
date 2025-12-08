namespace SocialNetwork.Repository.Errors
{
    public static class DirectMessageErrors
    {
        public const string SenderEmpty = "Sender ID cannot be empty.";
        public const string ReceiverEmpty = "Receiver ID cannot be empty.";
        public const string ContentEmpty = "Content cannot be empty.";
        public const string ContentTooLong = "Content cannot be longer than 280 characters.";
        public const string SenderDoesNotExist = "Sender does not exist.";
        public const string ReceiverDoesNotExist = "Receiver does not exist.";
    }
}
