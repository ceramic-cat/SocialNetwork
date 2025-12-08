namespace SocialNetwork.Repository.Errors
{
    public static class PostErrors
    {
        public const string InvalidUserId = "Invalid or missing user id.";
        public const string ContentEmpty = "Content cannot be empty.";
        public const string ContentTooLong = "Content cannot be longer than 280 characters.";
        public const string SenderDoesNotExist = "Sender does not exist.";
        public const string SenderEmpty = "Sender id cannot be empty.";
        public const string ReceiverDoesNotExist = "Receiver does not exist.";
        public const string RecieverEmpty = "Reciever id cannot be empty.";
        public const string PostNotFound = "Post not found.";
        public const string PostIdEmpty = "Post id cannot be empty.";
        public const string NotAllowedToDelete = "You are not allowed to delete this post.";
    }
}
