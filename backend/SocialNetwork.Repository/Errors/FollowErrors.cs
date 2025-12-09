namespace SocialNetwork.Repository.Errors
{
    public static class FollowErrors
    {
        public const string CannotFollowSelf = "You can't follow yourself.";
        public const string AlreadyFollowing = "Already following this user.";
        public const string EmptyUser = "Empty user.";
        public const string UnableToUnfollow = "Unable to unfollow that user.";
        public const string UnknownError = "An unknown error occurred while handling follow operation.";
    }
}
