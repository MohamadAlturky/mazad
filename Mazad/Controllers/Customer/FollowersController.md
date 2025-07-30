# Followers API Documentation

## Overview
The Followers API allows users to manage and retrieve their follower relationships within the Mazad system. This API provides functionality to view users that the current authenticated user is following.

## Base URL
```
/api/followers
```

## Authentication
All endpoints require authentication using the "User" policy. Users must be authenticated and have valid user permissions to access these endpoints.

## Endpoints

### Get Following Users

Retrieves a list of users that the authenticated user is currently following.

#### HTTP Request
```http
GET /api/followers/following
```

#### Authentication Required
- **Policy**: User
- **Authorization**: Bearer token required

#### Request Headers
```http
Authorization: Bearer <access_token>
Content-Type: application/json
```

#### Request Parameters
None

#### Response Format

##### Success Response (200 OK)
```json
{
  "data": [
    {
      "id": "string",
      "name": "string", 
      "phoneNumber": "string",
      "profilePhotoUrl": "string"
    }
  ],
  "success": true,
  "message": {
    "arabic": "تم جلب المتابعين بنجاح",
    "english": "Following users retrieved successfully"
  }
}
```

##### Error Response (400/500)
```json
{
  "success": false,
  "message": {
    "arabic": "فشل في جلب المتابعين", 
    "english": "Failed to retrieve following users"
  },
  "error": "Error details"
}
```

#### Response Fields

| Field | Type | Description |
|-------|------|-------------|
| `data` | Array | List of users being followed |
| `data[].id` | String | Unique identifier of the followed user |
| `data[].name` | String | Display name of the followed user |
| `data[].phoneNumber` | String | Phone number of the followed user |
| `data[].profilePhotoUrl` | String | URL to the followed user's profile photo |
| `success` | Boolean | Indicates if the request was successful |
| `message` | Object | Localized response message |
| `message.arabic` | String | Arabic language message |
| `message.english` | String | English language message |

#### Example Request
```bash
curl -X GET "https://api.mazad.com/api/followers/following" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json"
```

#### Example Response
```json
{
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "name": "أحمد محمد",
      "phoneNumber": "+966501234567",
      "profilePhotoUrl": "https://api.mazad.com/uploads/profiles/ahmed_photo.jpg"
    },
    {
      "id": "550e8400-e29b-41d4-a716-446655440001", 
      "name": "سارة أحمد",
      "phoneNumber": "+966507654321",
      "profilePhotoUrl": "https://api.mazad.com/uploads/profiles/sara_photo.jpg"
    }
  ],
  "success": true,
  "message": {
    "arabic": "تم جلب المتابعين بنجاح",
    "english": "Following users retrieved successfully"
  }
}
```

## Error Handling

The API uses standard HTTP status codes and returns localized error messages:

### Common Error Responses

#### 401 Unauthorized
```json
{
  "success": false,
  "message": {
    "arabic": "غير مصرح بالوصول",
    "english": "Unauthorized access"
  }
}
```

#### 500 Internal Server Error
```json
{
  "success": false,
  "message": {
    "arabic": "فشل في جلب المتابعين",
    "english": "Failed to retrieve following users"
  },
  "error": "Internal server error details"
}
```

## Rate Limiting
Standard rate limiting applies to all endpoints. Please refer to the main API documentation for rate limiting details.

## Notes
- All responses include bilingual messages (Arabic and English)
- The API returns users in the order they were followed
- Profile photo URLs may be null if no photo is uploaded
- Phone numbers are returned in international format

## Data Models

### UserListDto
```csharp
public class UserListDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string ProfilePhotoUrl { get; set; }
}
```

### LocalizedMessage
```csharp
public class LocalizedMessage
{
    public string Arabic { get; set; }
    public string English { get; set; }
}
```

## SDKs and Integration
For easier integration, consider using the official Mazad SDKs available for:
- JavaScript/TypeScript
- C#/.NET
- Python
- PHP

## Support
For API support and questions, please contact the development team or refer to the main API documentation. 