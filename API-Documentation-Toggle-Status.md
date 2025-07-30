# Toggle Status API Documentation

## Overview
The Mazad system provides toggle activation endpoints for managing the active status of various entities. These endpoints follow a consistent pattern across different administrative resources.

## Common Features
- **Method**: PATCH
- **Authentication**: Admin authorization required
- **Localization**: Supports Arabic (ar) and English (en) via `Accept-Language` header
- **Response Format**: Standardized `ApiResponse<T>` structure

## Endpoints

### 1. Toggle Category Status

**Endpoint**: `PATCH /api/admin/categories/toggle-activation/{id}`

**Description**: Toggles the activation status of a category.

**Authorization**: Required - Admin policy

**Parameters**:
- `id` (int, path parameter) - The category ID to toggle

**Request Headers**:
```
Accept-Language: en|ar
Authorization: Bearer {token}
```

**Success Response (200 OK)**:
```json
{
  "success": true,
  "message": "Category activated successfully" | "Category deactivated successfully",
  "data": {
    "id": 1,
    "name": "Electronics", 
    "isActive": true
  },
  "exception": "",
  "stackTrace": ""
}
```

**Arabic Response Example**:
```json
{
  "success": true,
  "message": "تم تفعيل الفئة بنجاح" | "تم إلغاء تفعيل الفئة بنجاح",
  "data": {
    "id": 1,
    "name": "إلكترونيات",
    "isActive": true
  },
  "exception": "",
  "stackTrace": ""
}
```

**Error Response - Category Not Found (200 OK)**:
```json
{
  "success": false,
  "message": "Category not found" | "الفئة غير موجودة",
  "exception": "",
  "stackTrace": ""
}
```

**Error Response - Server Error (200 OK)**:
```json
{
  "success": false,
  "message": "Failed to toggle activation status" | "فشل في تغيير حالة التفعيل", 
  "exception": "Exception message",
  "stackTrace": "Full stack trace"
}
```

---

### 2. Toggle Slider Status

**Endpoint**: `PATCH /api/admin/sliders/toggle-activation/{id}`

**Description**: Toggles the activation status of a slider.

**Authorization**: Currently commented out (may require Admin policy in production)

**Parameters**:
- `id` (int, path parameter) - The slider ID to toggle

**Request Headers**:
```
Accept-Language: en|ar
```

**Success Response (200 OK)**:
```json
{
  "success": true,
  "message": "Slider activated successfully" | "Slider deactivated successfully",
  "data": {
    "id": 1,
    "name": "Homepage Banner",
    "isActive": true
  },
  "exception": "",
  "stackTrace": ""
}
```

**Arabic Response Example**:
```json
{
  "success": true,
  "message": "تم تفعيل السلايدر بنجاح" | "تم إلغاء تفعيل السلايدر بنجاح",
  "data": {
    "id": 1,
    "name": "بانر الصفحة الرئيسية",
    "isActive": true
  },
  "exception": "",
  "stackTrace": ""
}
```

**Error Response - Slider Not Found (200 OK)**:
```json
{
  "success": false,
  "message": "Slider not found" | "السلايدر غير موجود",
  "exception": "",
  "stackTrace": ""
}
```

---

### 3. Toggle Region Status

**Endpoint**: `PATCH /api/admin/regions/toggle-activation/{id}`

**Description**: Toggles the activation status of a region.

**Authorization**: Currently commented out (may require Admin policy in production)

**Parameters**:
- `id` (int, path parameter) - The region ID to toggle

**Request Headers**:
```
Accept-Language: en|ar
```

**Success Response (200 OK)**:
```json
{
  "success": true,
  "message": "Region activated successfully" | "Region deactivated successfully",
  "data": {
    "id": 1,
    "name": "Riyadh",
    "isActive": true
  },
  "exception": "",
  "stackTrace": ""
}
```

**Arabic Response Example**:
```json
{
  "success": true,
  "message": "تم تفعيل المنطقة بنجاح" | "تم إلغاء تفعيل المنطقة بنجاح", 
  "data": {
    "id": 1,
    "name": "الرياض",
    "isActive": true
  },
  "exception": "",
  "stackTrace": ""
}
```

**Error Response - Region Not Found (200 OK)**:
```json
{
  "success": false,
  "message": "Region not found" | "المنطقة غير موجودة",
  "exception": "",
  "stackTrace": ""
}
```

## Code Examples

### JavaScript/Fetch
```javascript
// Toggle category status
const response = await fetch('/api/admin/categories/toggle-activation/1', {
  method: 'PATCH',
  headers: {
    'Accept-Language': 'en',
    'Authorization': 'Bearer your-token-here',
    'Content-Type': 'application/json'
  }
});

const result = await response.json();
console.log(result);
```

### C# HttpClient
```csharp
using var client = new HttpClient();
client.DefaultRequestHeaders.Add("Accept-Language", "en");
client.DefaultRequestHeaders.Authorization = 
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

var response = await client.PatchAsync("/api/admin/categories/toggle-activation/1", null);
var content = await response.Content.ReadAsStringAsync();
```

### cURL
```bash
curl -X PATCH \
  'https://your-api-domain/api/admin/categories/toggle-activation/1' \
  -H 'Accept-Language: en' \
  -H 'Authorization: Bearer your-token-here'
```

## Common Behavior

### Localization
- All endpoints support bilingual responses (Arabic/English)
- Language is determined by the `Accept-Language` header
- Default language is English if header is not provided

### Response Structure
- All responses use the standardized `ApiResponse<T>` format
- Success is always indicated by `success: true/false`
- Error details are provided in `exception` and `stackTrace` fields
- Data payload varies by endpoint but follows consistent naming conventions

### Error Handling
- All endpoints use try-catch blocks for robust error handling
- Errors return structured responses rather than throwing exceptions
- HTTP status code is always 200 OK (errors are indicated in response body)

### Security Notes
- Category endpoint requires Admin authorization
- Slider and Region endpoints have authorization commented out (review in production)
- All endpoints should validate user permissions before execution

## Status Codes

| HTTP Status | Description |
|-------------|-------------|
| 200 OK      | Request processed (check `success` field for actual result) |

## Notes
- The toggle operation flips the current `IsActive` status
- Changes are immediately persisted to the database
- Response includes the updated entity data
- Entity name is returned in the appropriate language based on the request header 