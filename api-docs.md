# API Documentation

## Admin User Controller

### Get Users

Retrieves a paginated list of users with the ability to filter by user type and search by name, email, or phone number.

**Endpoint:** `GET /api/AdminUser/users`

**Authorization:** `Admin` (Currently commented out in the code)

---

#### Query Parameters

| Parameter  | Type    | Required | Default | Description                                                                 |
| ---------- | ------- | -------- | ------- | --------------------------------------------------------------------------- |
| `Page`     | integer | No       | `1`     | The page number for pagination.                                             |
| `PageSize` | integer | No       | `10`    | The number of users to return per page.                                     |
| `UserType` | integer | No       |         | Filters users by their type. Can be `1` for Admin or `2` for User.        |
| `Search`   | string  | No       |         | A search term to filter users by name, email, or phone number.              |

---

#### Success Response (200 OK)

**Content-Type:** `application/json`

**Body:**

```json
{
  "data": {
    "Items": [
      {
        "Id": 1,
        "Name": "John Doe",
        "Email": "john.doe@example.com",
        "PhoneNumber": "1234567890",
        "UserType": 2,
        "ProfilePhotoUrl": "placeholder.png",
        "CreatedAt": "2023-10-27T10:00:00Z"
      }
    ],
    "TotalCount": 1,
    "Page": 1,
    "PageSize": 10,
    "TotalPages": 1
  },
  "success": true,
  "message": "تم جلب المستخدمين بنجاح"
}
```

---

#### Error Response (500 Internal Server Error)

**Content-Type:** `application/json`

**Body:**

```json
{
  "data": null,
  "success": false,
  "message": "فشل في جلب المستخدمين",
  "error": "Exception details here..."
}
```

---

#### Example Usage

**Get all users (defaults to page 1, size 10):**

```
GET /api/AdminUser/users
```

**Get second page of users:**

```
GET /api/AdminUser/users?Page=2&PageSize=20
```

**Get users with UserType 'User' who have 'john' in their name, email, or phone:**

```
GET /api/AdminUser/users?UserType=2&Search=john
``` 