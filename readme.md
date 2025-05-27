# ShitShat Backend

ShitShat is a messaging platform that allows users to create groups, send messages, and manage connections with others. 

## Features

- **User Authentication:** Register and login with JWT-based authentication.
- **Groups:** Create groups, manage group members, and send messages within groups.
- **Connections:** Send, accept, and delete connection requests between users.
- **Messages:** Send and receive messages in groups with rich user information (username, avatar).
- **Invites:** Create and manage invites.
- **Group roles:** Create and manage group roles.

## Technologies Used

- **ASP.NET Core Web API**
- **Entity Framework Core**
- **JWT Authentication with refresh tokens**
- **SignalR**
- **PostgreSQL**
- **FluentValidation**
- **Docker**
- **Github Workflows for automated deploys**

## TODO

- Refactor error handling, exceptions etc.

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

## ERD
![Alt text](https://i.imgur.com/kb5QGbK.jpeg)
