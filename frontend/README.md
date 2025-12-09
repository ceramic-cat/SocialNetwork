# MySpace– README

A compact overview of the Social Network application built with **ASP.NET Core**, **Entity Framework Core**, and **React**.

---

## Architecture

The backend is structured using a clean layered design:

- **Controllers** → Handle HTTP requests and return responses.
- **Services** → Contain business logic and validation.
- **Repositories** → Access the database using EF Core.
- **Error Classes** → Centralize error messages for consistency.
- **Entity**
- **Frontend (React)** → Communicates with the API via fetch. Styling with bootstrap and sass.

---

### **Authentication**

- JWT-based authentication
- Profile editing
- Account deletion
- Secure password hashing using BCrypt
- Token validation endpoint

---

## Features

- User registration & login (JWT-based)
- Create and delete posts
- User timelines with sender usernames
- Follow / Unfollow system
- Username search (case-insensitive)

---

## Testing

## Code coverage:

- **Repository tests**
- **Service tests**
- **Controller tests**

---

## Clean Code

- The architecture is structured with clear separation of concerns (controllers → services → repositories), making the system easier to navigate and extend.

- Reusable error constants eliminate magic strings and ensure consistent error handling across the API.

- Meaningful naming and small, focused methods improve readability and reduce cognitive load.

- Independent layers and low coupling make the application easier to test and maintain as it expands.

## Running the Project

### Backend

dotnet user-secrets set "Jwt:Key" "{secret-key-available-on-request}"
dotnet run

### Frontend

npm install
npm run dev

---

## Known Limitations

- No brute-force protection or rate limiting
- No refresh tokens
- No pagination in timeline or search results
- No tests in frontend
- No cashning or SignalR
- Some pages requires manual refresh

---

## Summary

This project demonstrates clean architecture, clear separation of concerns, reusable error handling, and a fully tested API connected to a React frontend designed with a galactic theme.
