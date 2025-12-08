const BASE_URL = "http://localhost:5148";

export const API = {
  BASE_URL,
  POSTS: `${BASE_URL}/api/posts`,
  AUTH: {
    LOGIN: `${BASE_URL}/api/auth/login`,
    REGISTER: `${BASE_URL}/api/auth/register`,
    VALIDATE: `${BASE_URL}/api/auth/validate`,
    DELETE_ACCOUNT: `${BASE_URL}/api/auth/delete-account`,
  },
  USERS: {
    TIMELINE: (userId: string) => `${BASE_URL}/api/users/${userId}/timeline`,
  },
  FOLLOW: {
    FOLLOW: (userId: string) => `${BASE_URL}/api/Follow/${userId}`,
    GETFOLLOWS: `${BASE_URL}/api/Follow`,
  },
};
