// DirectMessage error messages - should match backend DirectMessageErrors
export const DirectMessageErrors = {
  InvalidUserId: "Invalid or missing user id.",
  SenderEmpty: "Sender ID cannot be empty.",
  ReceiverEmpty: "Receiver ID cannot be empty.",
  ContentEmpty: "Content cannot be empty.",
  ContentTooLong: "Content cannot be longer than 280 characters.",
  SenderDoesNotExist: "Sender does not exist.",
  ReceiverDoesNotExist: "Receiver does not exist.",
} as const;
