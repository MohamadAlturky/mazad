# Chat API Documentation

## Get User Chats

Returns all chat conversations where the current user is a participant.

**URL**: `/api/Chat/my-chats`

**Method**: `GET`

**Auth required**: Yes (User)

### Success Response

**Code**: `200 OK`

**Content example**:

```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "user1": {
        "id": 123,
        "name": "John Doe",
        "phoneNumber": "+1234567890",
        "profilePhotoUrl": "https://example.com/profile.jpg"
      },
      "user2": {
        "id": 456,
        "name": "Jane Smith",
        "phoneNumber": "+9876543210",
        "profilePhotoUrl": "https://example.com/profile2.jpg"
      }
    }
  ],
  "message": "تم جلب المحادثات بنجاح"
}
```

### Error Response

**Condition**: If an error occurs during processing.

**Code**: `400 BAD REQUEST`

**Content example**:

```json
{
  "success": false,
  "message": "فشل في جلب المحادثات"
}
```

### Notes

- The current user can be either `user1` or `user2` in the returned chat objects
- The endpoint returns all chats regardless of whether they contain messages
- Authentication is required with the "User" policy
