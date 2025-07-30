# Get User Profile by ID

## Endpoint
`GET /api/User/profile/{userId}`

## Description
Retrieves public profile information for a specific user by their ID. This endpoint returns comprehensive user data including basic profile information, statistics, and their offers.

## Parameters

### Path Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `userId` | `integer` | Yes | The unique identifier of the user whose profile is being requested |

### Headers
| Header | Type | Required | Description |
|--------|------|----------|-------------|
| `Accept-Language` | `string` | No | Language preference (`ar` for Arabic, `en` for English). Defaults to English if not provided |

## Response Format

### Success Response (200 OK)
```json
{
  "success": true,
  "message": "User profile retrieved successfully", // or Arabic equivalent
  "data": {
    "id": 123,
    "name": "John Doe",
    "phoneNumber": "+1234567890",
    "profilePhotoUrl": "https://example.com/profile.jpg",
    "memberSince": "2023-01-15T10:30:00Z",
    "offersCount": 15,
    "followersCount": 25,
    "followingCount": 10,
    "offers": [
      {
        "id": 456,
        "name": "Sample Offer",
        "description": "This is a sample offer description",
        "price": 100.50,
        "categoryId": 2,
        "categoryName": "Electronics", // Localized based on Accept-Language
        "regionId": 1,
        "regionName": "New York", // Localized based on Accept-Language
        "mainImageUrl": "https://example.com/offer-image.jpg",
        "createdAt": "2023-12-01T14:20:00Z"
      }
    ]
  }
}
```

### Error Response (200 OK with success: false)
```json
{
  "success": false,
  "message": "User not found", // or Arabic equivalent
  "data": null,
  "exception": "Error details if any",
  "stackTrace": "Stack trace if available"
}
```

## Response Schema

### PublicUserProfileDto
| Field | Type | Description |
|-------|------|-------------|
| `id` | `integer` | Unique identifier of the user |
| `name` | `string` | Full name of the user |
| `phoneNumber` | `string` | User's phone number |
| `profilePhotoUrl` | `string` | URL to user's profile photo (nullable) |
| `memberSince` | `string (ISO 8601)` | Date when user joined the platform |
| `offersCount` | `integer` | Total number of offers created by the user |
| `followersCount` | `integer` | Number of users following this user |
| `followingCount` | `integer` | Number of users this user is following |
| `offers` | `array[OfferDto]` | List of user's offers (ordered by creation date, newest first) |

### OfferDto
| Field | Type | Description |
|-------|------|-------------|
| `id` | `integer` | Unique identifier of the offer |
| `name` | `string` | Name/title of the offer |
| `description` | `string` | Detailed description of the offer |
| `price` | `number` | Price of the offer |
| `categoryId` | `integer` | ID of the offer's category |
| `categoryName` | `string` | Localized name of the category |
| `regionId` | `integer` | ID of the offer's region |
| `regionName` | `string` | Localized name of the region |
| `mainImageUrl` | `string` | URL to the main image of the offer |
| `createdAt` | `string (ISO 8601)` | Date and time when the offer was created |

## Status Codes
| Code | Description |
|------|-------------|
| 200 | Success - Returns user profile data or error message |

## Error Cases
- **User Not Found**: When the provided `userId` doesn't exist or doesn't correspond to a regular user (UserType.User)
- **Server Error**: When an unexpected error occurs during processing

## Localization
- The response message and category/region names are localized based on the `Accept-Language` header
- Supported languages: Arabic (`ar`) and English (`en`)
- Default language: English

## Example Usage

### Request
```http
GET /api/User/profile/123 HTTP/1.1
Host: api.example.com
Accept-Language: en
```

### Response
```json
{
  "success": true,
  "message": "User profile retrieved successfully",
  "data": {
    "id": 123,
    "name": "Ahmed Mohamed",
    "phoneNumber": "+201234567890",
    "profilePhotoUrl": "https://api.example.com/uploads/profiles/ahmed.jpg",
    "memberSince": "2023-01-15T10:30:00Z",
    "offersCount": 8,
    "followersCount": 12,
    "followingCount": 5,
    "offers": [
      {
        "id": 789,
        "name": "iPhone 14 Pro",
        "description": "Brand new iPhone 14 Pro in excellent condition",
        "price": 1200.00,
        "categoryId": 1,
        "categoryName": "Electronics",
        "regionId": 2,
        "regionName": "Cairo",
        "mainImageUrl": "https://api.example.com/uploads/offers/iphone14.jpg",
        "createdAt": "2023-12-01T14:20:00Z"
      },
      {
        "id": 790,
        "name": "Gaming Laptop",
        "description": "High-performance gaming laptop with RTX graphics",
        "price": 1500.00,
        "categoryId": 1,
        "categoryName": "Electronics",
        "regionId": 2,
        "regionName": "Cairo",
        "mainImageUrl": "https://api.example.com/uploads/offers/laptop.jpg",
        "createdAt": "2023-11-28T09:15:00Z"
      }
    ]
  }
}
```

## Notes
- This endpoint is publicly accessible and doesn't require authentication
- Only users with `UserType.User` are returned; admin users are filtered out
- Offers are sorted by creation date in descending order (newest first)
- All nullable fields can return `null` values
- The endpoint always returns HTTP 200, with success/failure indicated by the `success` field in the response body 