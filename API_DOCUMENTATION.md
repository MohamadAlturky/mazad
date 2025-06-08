# Dynamic Attributes API Documentation

**Base URL:** `http://localhost:5032`

## Response Format
All API responses follow this structure:
```json
{
    "success": boolean,
    "message": string,
    "data": T,  // Generic type that varies by endpoint
    "exception": string,
    "stackTrace": string
}
```

## Endpoints

### 1. Get All Dynamic Attributes
- **URL:** `/api/dynamic-attributes`
- **Method:** `GET`
- **Description:** Retrieves all dynamic attributes
- **Response:** List of dynamic attributes
- **Example Response:**
```json
{
    "success": true,
    "message": "",
    "data": [
        {
            // Dynamic attribute objects
        }
    ],
    "exception": "",
    "stackTrace": ""
}
```

### 2. Create Dynamic Attribute
- **URL:** `/api/dynamic-attributes`
- **Method:** `POST`
- **Description:** Creates a new dynamic attribute
- **Request Body:**
```json
{
    // CreateDynamicAttributeApiRequest properties
}
```
- **Response:** Created dynamic attribute
- **Example Response:**
```json
{
    "success": true,
    "message": "",
    "data": {
        // Created dynamic attribute object
    },
    "exception": "",
    "stackTrace": ""
}
```

### 3. Update Dynamic Attribute
- **URL:** `/api/dynamic-attributes`
- **Method:** `PUT`
- **Description:** Updates an existing dynamic attribute
- **Request Body:**
```json
{
    // UpdateDynamicAttributeApiRequest properties
}
```
- **Response:** Updated dynamic attribute
- **Example Response:**
```json
{
    "success": true,
    "message": "",
    "data": {
        // Updated dynamic attribute object
    },
    "exception": "",
    "stackTrace": ""
}
```

### 4. Delete Dynamic Attribute
- **URL:** `/api/dynamic-attributes`
- **Method:** `DELETE`
- **Description:** Deletes a dynamic attribute
- **Request Body:**
```json
{
    // DeleteDynamicAttributeApiRequest properties
}
```
- **Response:** Success status
- **Example Response:**
```json
{
    "success": true,
    "message": "",
    "data": null,
    "exception": "",
    "stackTrace": ""
}
```

### 5. Toggle Dynamic Attribute Activation
- **URL:** `/api/dynamic-attributes/toggle-activation/{id}`
- **Method:** `PUT`
- **Description:** Toggles the activation status of a dynamic attribute
- **URL Parameters:**
  - `id`: The ID of the dynamic attribute to toggle
- **Response:** Updated dynamic attribute
- **Example Response:**
```json
{
    "success": true,
    "message": "",
    "data": {
        // Updated dynamic attribute object
    },
    "exception": "",
    "stackTrace": ""
}
```

## Notes
1. All endpoints require authentication (based on the `BaseController` inheritance)
2. The API automatically handles language preferences and user identification
3. Error responses will include details in the `exception` and `stackTrace` fields
4. All endpoints return a consistent response wrapper structure 