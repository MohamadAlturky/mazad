# Create Offer API

## Endpoint

```
POST /create
```

## Authorization

Requires user authentication. Include a valid authentication token in the request header.

## Request

### Content Type

`multipart/form-data`

### Parameters

| Parameter   | Type          | Required | Description                                   |
|-------------|---------------|----------|-----------------------------------------------|
| Name        | string        | Yes      | The name/title of the offer                   |
| Description | string        | Yes      | Detailed description of the offer             |
| Price       | decimal       | Yes      | The price of the offer                        |
| CategoryId  | integer       | Yes      | The ID of the category for this offer         |
| RegionId    | integer       | Yes      | The ID of the region where the offer applies  |
| Images      | file array    | Yes      | At least one image must be provided           |

**Note:**
- The first image will be used as the main image
- Multiple images can be provided

## Response

### Success Response

```json
{
  "Success": true,
  "Message": "Offer created successfully",
  "Data": {
    "Id": 123,
    "Name": "Sample Offer",
    "Description": "This is a sample offer description",
    "Price": 999.99,
    "CategoryId": 1,
    "CategoryName": "Electronics",
    "RegionId": 2,
    "RegionName": "Dubai",
    "MainImageUrl": "/storage/offers/123/main/image.jpg",
    "AdditionalImages": [
      "/storage/offers/123/additional/image1.jpg",
      "/storage/offers/123/additional/image2.jpg"
    ],
    "CreatedAt": "2023-05-20T14:30:45Z",
    "IsActive": true
  },
  "Exception": "",
  "StackTrace": ""
}
```

### Error Response

```json
{
  "Success": false,
  "Message": "Failed to create offer",
  "Data": null,
  "Exception": "Error message details",
  "StackTrace": "Stack trace information"
}
```

### Common Error Cases

| Error                       | Message (English)              | Message (Arabic)                |
|-----------------------------|-----------------------------|------------------------------|
| Invalid Category ID         | "Invalid category ID"       | "معرف الفئة غير صالح"        |
| Invalid Region ID           | "Invalid region ID"         | "معرف المنطقة غير صالح"      |
| Category not found          | "Category not found"        | "الفئة غير موجودة"           |
| Region not found            | "Region not found"          | "المنطقة غير موجودة"         |
| No images provided          | "At least one image must be provided" | "يجب تقديم صورة واحدة على الأقل" |
| General failure             | "Failed to create offer"    | "فشل في إنشاء العرض"         | 