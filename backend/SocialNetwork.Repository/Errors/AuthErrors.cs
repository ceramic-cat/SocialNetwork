namespace SocialNetwork.Repository.Errors
{
    public static class AuthErrors
    {
        public const string RegisterFailed = "Could not register.";
        public const string UsernameOrEmailTaken = "Username or email is already in use.";

        public const string InvalidCredentials = "Invalid credentials.";

        public const string UserNotFound = "User not found.";
        public const string CouldNotUpdateProfile = "Could not update profile.";
        public const string CouldNotDeleteAccount = "Could not delete account.";
        public const string CouldNotDeleteUser = "User not found or could not be deleted.";

        public const string MissingUserIdClaim = "User id is missing in token.";

        public const string InvalidJwtKey = "JWT key is not configured.";
    }
}
