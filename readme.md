
# ShitShat Backend

ShitShat is a messaging platform that allows users to create groups, send messages, and manage connections with others. This repository contains the backend API for ShitShat, built with **ASP.NET Core Web API** and **Entity Framework**.

## Features

- **User Authentication:** Register and login with JWT-based authentication.
- **Groups:** Create groups, manage group members, and send messages within groups.
- **Connections:** Send, accept, and delete connection requests between users.
- **Messages:** Send and receive messages in groups with rich user information (username, avatar).

## Technologies Used

- **ASP.NET Core Web API**
- **Entity Framework Core**
- **JWT Authentication**
- **SQL Server** (Database)

## TODO

- Refactor all validation logic for better maintainability and consistency.
- Refactor all generic responses to standardize the format and improve error handling.

## Endpoints

- **Auth**
  - `POST /api/v1/auth/register` - Register a new user
  - `POST /api/v1/auth/login` - Login a user

- **Connection**
  - `POST /api/v1/connection/add` - Send a new connection request
  - `PUT /api/v1/connection/accept` - Accept a friend request
  - `DELETE /api/v1/connection/delete` - Delete a friend

- **Group**
  - `POST /api/v1/group` - Create a new group
  - `GET /api/v1/group/{groupGuid}` - Get group details
  - `POST /api/v1/group/{groupGuid}/members` - Add members to a group
  - `GET /api/v1/group/{groupGuid}/members` - List group members
  - `GET /api/v1/group/{groupGuid}/messages` - Get group messages
  - `POST /api/v1/group/{groupGuid}/messages` - Send a message to a group

- **User**
  - `GET /api/v1/user/{guid}` - Get a specific user by GUID
  - `PUT /api/v1/user/avatar` - Update user avatar
  - `GET /api/v1/user/connections` - Get user's connections
  - `GET /api/v1/user/groups` - Get groups the user is a part of