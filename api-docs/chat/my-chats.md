# Get My Chats

Retrieves all chat conversations for the authenticated user.

## Endpoint

```
GET /api/chat/my-chats
```

## Authentication

- Requires user authentication
- User policy required

## Request Parameters

No parameters required.

## Response

### Success Response

- **Status Code**: 200 OK
- **Content**:
  - `success`: `true`
  - `data`: Array of chat objects
  - `message`: Localized success message

### Chat Object

| Field | Type | Description |
|-------|------|-------------|
| `id` | integer | Unique identifier of the chat |
| `user` | object | Information about the other user in the conversation |

### User Object

| Field | Type | Description |
|-------|------|-------------|
| `id` | integer | Unique identifier of the user |
| `name` | string | Name of the user |
| `phoneNumber` | string | Phone number of the user |
| `profilePhotoUrl` | string | URL to the user's profile photo |

### Error Response

- **Status Code**: 400 Bad Request
- **Content**:
  - `success`: `false`
  - `message`: Localized error message

## Example Response

```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "user": {
        "id": 2,
        "name": "John Doe",
        "phoneNumber": "+1234567890",
        "profilePhotoUrl": "https://example.com/profile.jpg"
      }
    },
    {
      "id": 3,
      "user": {
        "id": 4,
        "name": "Jane Smith",
        "phoneNumber": "+0987654321",
        "profilePhotoUrl": "https://example.com/jane.jpg"
      }
    }
  ],
  "message": "تم جلب المحادثات بنجاح"
}
```