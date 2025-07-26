

### Get Offers List

Retrieves a paginated list of offers with optional filtering.

**URL**: `GET /api/offers`

**Authorization**: None required

**Parameters**:

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| cursor | integer | No | null | Pagination cursor (ID of the last item from previous page) |
| limit | integer | No | 10 | Number of items per page (max 50) |
| sortDescending | boolean | No | true | Sort order by ID |
| categoryId | integer | No | 0 | Filter by category ID |
| regionId | integer | No | 0 | Filter by region ID |
| minPrice | double | No | 0 | Filter by minimum price |
| maxPrice | double | No | 0 | Filter by maximum price |
| searchTerm | string | No | null | Search in name and description |

**Response**:

```json
{
  "data": {
    "items": [
      {
        "id": 42,
        "name": "iPhone 13 Pro",
        "description": "Brand new iPhone 13 Pro for sale",
        "price": 3500.0,
        "categoryId": 2,
        "categoryName": "Mobile Phones",
        "regionId": 3,
        "regionName": "Dubai",
        "mainImageUrl": "https://example.com/images/offers/42/main.jpg",
        "createdAt": "2023-07-15T10:30:00Z"
      },
      // More items...
    ],
    "hasMore": true,
    "nextCursor": 36
  },
  "success": true,
  "message": {
    "arabic": "تم استرجاع العروض بنجاح",
    "english": "Offers retrieved successfully"
  }
}
```

**Error Responses**:

- **Server Error**:
  ```json
  {
    "data": false,
    "success": false,
    "message": {
      "arabic": "فشل في استرجاع العروض",
      "english": "Failed to retrieve offers"
    }
  }
  ```

### Create Offer

Creates a new offer.

**URL**: `POST /api/offers/create`

**Authorization**: Required (User)

**Content Type**: `multipart/form-data`

**Request Body**:

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Offer name |
| description | string | Yes | Offer description |
| price | double | Yes | Offer price |
| categoryId | integer | Yes | Category ID |
| regionId | integer | Yes | Region ID |
| images | file[] | Yes | At least one image is required. First image will be used as main image |

**Response**:

```json
{
  "data": {
    "id": 43,
    "name": "New Laptop",
    "description": "Brand new laptop for sale",
    "price": 2500.0,
    "categoryId": 3,
    "categoryName": "Laptops",
    "regionId": 2,
    "regionName": "Abu Dhabi",
    "mainImageUrl": "https://example.com/images/offers/43/main.jpg",
    "additionalImages": [
      "https://example.com/images/offers/43/additional1.jpg",
      "https://example.com/images/offers/43/additional2.jpg"
    ],
    "createdAt": "2023-07-16T14:25:00Z",
    "isActive": true
  },
  "success": true,
  "message": {
    "arabic": "تم إنشاء العرض بنجاح",
    "english": "Offer created successfully"
  }
}
```

**Error Responses**:

- **Invalid Category**:
  ```json
  {
    "data": false,
    "success": false,
    "message": {
      "arabic": "الفئة غير موجودة",
      "english": "Category not found"
    }
  }
  ```

- **Invalid Region**:
  ```json
  {
    "data": false,
    "success": false,
    "message": {
      "arabic": "المنطقة غير موجودة",
      "english": "Region not found"
    }
  }
  ```

- **No Images Provided**:
  ```json
  {
    "data": false,
    "success": false,
    "message": {
      "arabic": "يجب تقديم صورة واحدة على الأقل",
      "english": "At least one image must be provided"
    }
  }
  ```

- **Server Error**:
  ```json
  {
    "data": false,
    "success": false,
    "message": {
      "arabic": "فشل في إنشاء العرض",
      "english": "Failed to create offer"
    }
  }
  ```

## Notes

- The API supports localization. The language is determined from the request headers.
- Images are stored in the file system with paths relative to the offers and their IDs.
- Pagination is implemented using cursor-based pagination for better performance. 