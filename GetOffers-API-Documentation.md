# Get Offers API Documentation

## Overview
Retrieves a paginated list of offers with optional filtering and search capabilities.

## Endpoint
```
GET /api/offers
```

## Authentication
No authentication required (public endpoint)

## Request Parameters

All parameters are optional and passed as query string parameters.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `cursor` | integer | null | Pagination cursor for retrieving next/previous page |
| `limit` | integer | 5 | Number of items to return (maximum: 50, minimum: 1) |
| `sortDescending` | boolean | true | Sort order by ID (true = newest first, false = oldest first) |
| `categoryId` | integer | null | Filter offers by category ID |
| `regionId` | integer | null | Filter offers by region ID |
| `minPrice` | number | null | Minimum price filter (inclusive) |
| `maxPrice` | number | null | Maximum price filter (inclusive) |
| `searchTerm` | string | null | Search term to filter by offer name or description |

### Pagination Logic
- **cursor**: Used for cursor-based pagination
  - If `sortDescending = true`: Returns offers with ID < cursor
  - If `sortDescending = false`: Returns offers with ID > cursor
- **limit**: Controls page size, automatically capped at 50

### Example Request URLs

```bash
# Basic request - get first 10 offers
GET /api/offers?limit=10

# Filter by category and region
GET /api/offers?categoryId=1&regionId=2&limit=20

# Search with price range
GET /api/offers?searchTerm=laptop&minPrice=100&maxPrice=1000

# Pagination - get next page
GET /api/offers?cursor=45&limit=10&sortDescending=true

# Complex filtering
GET /api/offers?categoryId=3&regionId=1&minPrice=50&maxPrice=500&searchTerm=phone&limit=15
```

## Response Format

The API uses a standardized response wrapper with the following structure:

### Success Response
```json
{
  "success": true,
  "message": "Offers retrieved successfully", // English
  "message": "تم استرجاع العروض بنجاح", // Arabic (based on Accept-Language header)
  "data": {
    "items": [
      {
        "id": 1,
        "name": "Sample Offer",
        "description": "Offer description",
        "price": 299.99,
        "categoryId": 1,
        "categoryName": "Electronics", // Localized based on language
        "regionId": 2,
        "regionName": "New York", // Localized based on language
        "mainImageUrl": "/uploads/offers/1/main/image.jpg",
        "createdAt": "2024-01-15T10:30:00Z",
        "numberOfFavorites": 12,
        "numberOfViews": 156,
        "numberOfComments": 8
      }
      // ... more offer items
    ],
    "hasMore": true,
    "nextCursor": 45
  }
}
```

### Error Response
```json
{
  "success": false,
  "message": "Failed to retrieve offers", // English
  "message": "فشل في استرجاع العروض", // Arabic
  "data": null,
  "exception": "Error message details",
  "stackTrace": "Detailed stack trace for debugging"
}
```

## Response Fields

### Root Response Object
| Field | Type | Description |
|-------|------|-------------|
| `success` | boolean | Indicates if the request was successful |
| `message` | string | Localized success/error message |
| `data` | object/null | Response data (null on error) |
| `exception` | string | Error message (only present on errors) |
| `stackTrace` | string | Detailed error trace (only present on errors) |

### Data Object
| Field | Type | Description |
|-------|------|-------------|
| `items` | array | List of offer objects |
| `hasMore` | boolean | Whether there are more results available |
| `nextCursor` | integer/null | Cursor value for fetching next page |

### Offer Item Object
| Field | Type | Description |
|-------|------|-------------|
| `id` | integer | Unique offer identifier |
| `name` | string | Offer title |
| `description` | string | Offer description |
| `price` | number | Offer price |
| `categoryId` | integer | Category identifier |
| `categoryName` | string | Localized category name |
| `regionId` | integer | Region identifier |
| `regionName` | string | Localized region name |
| `mainImageUrl` | string | URL to the main offer image |
| `createdAt` | string | ISO 8601 timestamp of creation |
| `numberOfFavorites` | integer | Count of users who favorited this offer |
| `numberOfViews` | integer | View count |
| `numberOfComments` | integer | Comment count |

## Localization

The API supports Arabic and English localization:
- **Request Header**: Include `Accept-Language: ar` for Arabic or `Accept-Language: en` for English
- **Localized Fields**: `message`, `categoryName`, `regionName`
- **Default**: English if no language header is provided

## HTTP Status Codes

| Status Code | Description |
|-------------|-------------|
| 200 | Success - Request processed successfully (check `success` field for actual result) |
| 500 | Internal Server Error - Unexpected server error |

## Example Usage

### JavaScript/Fetch
```javascript
// Basic request
const response = await fetch('/api/offers?limit=20');
const data = await response.json();

if (data.success) {
  console.log('Offers:', data.data.items);
  console.log('Has more:', data.data.hasMore);
} else {
  console.error('Error:', data.message);
}

// With filtering
const filteredResponse = await fetch('/api/offers?categoryId=1&searchTerm=phone&minPrice=100');
const filteredData = await filteredResponse.json();
```

### cURL
```bash
# Basic request
curl "https://api.example.com/api/offers?limit=10"

# With Arabic language
curl -H "Accept-Language: ar" "https://api.example.com/api/offers?categoryId=1"

# Complex filtering
curl "https://api.example.com/api/offers?categoryId=2&regionId=1&minPrice=50&maxPrice=500&searchTerm=laptop&limit=20"
```

## Pagination Example

```javascript
let cursor = null;
let allOffers = [];

do {
  const url = cursor 
    ? `/api/offers?cursor=${cursor}&limit=20`
    : '/api/offers?limit=20';
    
  const response = await fetch(url);
  const data = await response.json();
  
  if (data.success) {
    allOffers.push(...data.data.items);
    cursor = data.data.nextCursor;
  }
} while (data.data.hasMore);

console.log('All offers loaded:', allOffers.length);
```

## Performance Notes

- Maximum limit is 50 items per request to prevent performance issues
- Cursor-based pagination is more efficient than offset-based for large datasets
- Only active and non-deleted offers are returned
- Database queries are optimized with proper indexing on filtered fields

## Error Handling

Always check the `success` field in the response:
- `success: true` - Request completed successfully
- `success: false` - An error occurred, check `message` and `exception` fields

Common error scenarios:
- Invalid parameter values
- Database connection issues
- Internal server errors 