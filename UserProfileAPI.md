# User Profile API

## Get User Profile

Retrieves the profile information for the authenticated user.

**URL:** `api/User/profile`

**Method:** `GET`

**Auth required:** Yes (JWT token)

### Request Headers

| Name | Required | Description |
|------|----------|-------------|
| Authorization | Yes | Bearer {token} |

### Response

**Success Response (200 OK)**

```json
{
  "data": {
    "id": 1,
    "name": "User Name",
    "phoneNumber": "+123456789",
    "userType": 2,
    "profilePhotoUrl": "https://example.com/photo.jpg",
    "followedUsers": [
      {
        "id": 2,
        "name": "Followed User",
        "phoneNumber": "+987654321",
        "profilePhotoUrl": "https://example.com/photo2.jpg"
      }
    ],
    "offers": [
      {
        "id": 1,
        "name": "Offer Name",
        "description": "Offer Description",
        "price": 100.0,
        "categoryId": 1,
        "categoryName": "Category Name",
        "regionId": 1,
        "regionName": "Region Name",
        "mainImageUrl": "https://example.com/offer.jpg",
        "createdAt": "2023-07-20T14:30:00"
      }
    ]
  },
  "success": true,
  "message": {
    "arabic": "تم جلب معلومات المستخدم بنجاح",
    "english": "User profile retrieved successfully"
  }
}
```

**Error Response (401 Unauthorized)**

```json
{
  "data": null,
  "success": false,
  "message": {
    "arabic": "غير مصرح",
    "english": "Unauthorized"
  }
}
```

**Error Response (500 Internal Server Error)**

```json
{
  "data": null,
  "success": false,
  "message": {
    "arabic": "فشل في جلب معلومات المستخدم",
    "english": "Failed to retrieve user profile"
  }
}
``` 