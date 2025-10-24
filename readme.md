# ShitChat Backend

ShitChat is a messaging platform that allows users to create groups, send messages, handle roles, invites and bans.

## Features

- **User Authentication:** Register and login with JWT-based authentication.
- **Groups:** Create groups, edit and delete groups.
- **Connections:** Send, accept, and delete connection requests between users. (Only backend support for now)
- **Invites:** Create and manage invites.
- **Group roles:** Create and manage group roles with different permissions.
-  **Send messages:** Send realtime messages within a group.

## Technologies Used

- **ASP.NET Core Web API**
- **Entity Framework Core**
- **JWT Authentication with refresh tokens**
- **SignalR**
- **PostgreSQL**
- **FluentValidation**
- **Docker**
- **Github Workflows for automated deploys**

## Getting Started

**1. Clone the repository:**

```bash
git clone https://github.com/nostr1c/ShitChatApi.git
cd ShitChatApi
```

**2. Add .env file to root of project**
```
DB_DATABASE=ShitChat
DB_USER=sa
DB_PASSWORD=changethis
```

**3. Start application**
```bash
docker-compose up --build
```

**App is now running at port 8080**

## API Documentation
Can be found at http://localhost:8080/scalar/v1

## ERD
![Alt text](https://i.imgur.com/kb5QGbK.jpeg)
