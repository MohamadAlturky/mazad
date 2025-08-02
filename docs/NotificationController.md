# Notifications API Documentation

This document provides details about the API endpoints for managing notifications.

## Base URL

`http://localhost:5000/api/Notification`

---

## Get My Notifications

Retrieves a paginated list of notifications for the currently authenticated user.

- **URL**: `/my-notifications`
- **Method**: `GET`
- **Authorization**: `Bearer [token]` (User)

### Query Parameters

| Parameter  | Type    | Description                                           | Default |
| ---------- | ------- | ----------------------------------------------------- | ------- |
| `Page`     | integer | The page number to retrieve.                          | 1       |
| `PageSize` | integer | The number of notifications to retrieve per page.     | 20      |

### Success Response (200 OK)

```json
{
  "data": {
    "notifications": [
      {
        "id": 1,
        "title": "New Message",
        "body": "You have a new message from John Doe.",
        "imageUrl": "http://example.com/image.png",
        "actionUrl": "/chats/123",
        "isRead": false,
        "createdAt": "2024-07-29T10:00:00Z",
        "notificationType": "Message"
      }
    ],
    "totalCount": 1,
    "page": 1,
    "pageSize": 20
  },
  "success": true,
  "message": "Notifications retrieved successfully"
}
```

### Error Response (401 Unauthorized)

```json
{
    "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
    "title": "Unauthorized",
    "status": 401,
    "traceId": "00-..."
}
```

---

## Mark Notification as Read

Marks a single notification as read.

- **URL**: `/{notificationId}/mark-as-read`
- **Method**: `POST`
- **Authorization**: `Bearer [token]` (User)

### URL Parameters

| Parameter        | Type   | Description                       |
| ---------------- | ------ | --------------------------------- |
| `notificationId` | long   | The ID of the notification to mark as read. |

### Success Response (200 OK)

```json
{
  "success": true,
  "message": "Notification marked as read successfully"
}
```

### Error Responses

#### 404 Not Found

```json
{
  "success": false,
  "message": "Notification not found or you don't have access to it"
}
```

#### 401 Unauthorized

```json
{
    "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
    "title": "Unauthorized",
    "status": 401,
    "traceId": "00-..."
}
```

---

## Mark All Notifications as Read

Marks all unread notifications for the current user as read.

- **URL**: `/mark-all-as-read`
- **Method**: `POST`
- **Authorization**: `Bearer [token]` (User)

### Success Response (200 OK)

```json
{
  "success": true,
  "message": "All notifications marked as read successfully"
}
```

### Error Response (401 Unauthorized)

```json
{
    "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
    "title": "Unauthorized",
    "status": 401,
    "traceId": "00-..."
}
```

---

## Get Unread Notifications Count

Retrieves the count of unread notifications for the currently authenticated user.

- **URL**: `/unread-count`
- **Method**: `GET`
- **Authorization**: `Bearer [token]` (User)

### Success Response (200 OK)

```json
{
  "data": {
    "unreadCount": 5
  },
  "success": true,
  "message": "Unread notifications count retrieved successfully"
}
```

### Error Response (401 Unauthorized)

```json
{
    "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
    "title": "Unauthorized",
    "status": 401,
    "traceId": "00-..."
}
``` 