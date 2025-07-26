# Offers API Documentation

## Get Offers (Paginated)

Retrieves a paginated list of offers with optional filtering.

### Endpoint

```
GET /api/offers
```

### Request Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| cursor | integer | No | null | Reference point for pagination. Pass the `nextCursor` value from previous response to get the next page. |
| limit | integer | No | 10 | Number of items per page (max 50). |
| sortDescending | boolean | No | true | Sort direction. `true` for newest first, `false` for oldest first. |
| categoryId | integer | No | 0 | Filter by category ID. |
| regionId | integer | No | 0 | Filter by region ID. |
| minPrice | number | No | 0 | Filter by minimum price. |
| maxPrice | number | No | 0 | Filter by maximum price. |
| searchTerm | string | No | null | Search in offer name and description. |

### Response

```json
{
  "success": true,
  "message": "Offers retrieved successfully",
  "data": {
    "items": [
      {
        "id": 123,
        "name": "Sample Offer",
        "description": "This is a sample offer description",
        "price": 1500.00,
        "categoryId": 5,
        "categoryName": "Electronics",
        "regionId": 3,
        "regionName": "Dubai",
        "mainImageUrl": "uploads/offers/123/main/image.jpg",
        "createdAt": "2023-06-15T14:30:45Z"
      },
      // More items...
    ],
    "hasMore": true,
    "nextCursor": 120
  }
}
```

### Example Usage

#### First Page Request
```
GET /api/offers?limit=10&categoryId=5&minPrice=1000
```

#### Next Page Request
```
GET /api/offers?limit=10&categoryId=5&minPrice=1000&cursor=120
```

### Pagination Flow

1. Make initial request without a cursor
2. Check if `hasMore` is `true` in the response
3. Use the `nextCursor` value in your next request to get the next page
4. Repeat until `hasMore` is `false`
