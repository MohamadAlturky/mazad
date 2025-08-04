# Statistics Controller

This controller is designed to provide various statistical data for the admin dashboard, offering insights into the platform's activity.

## Get Statistics

Retrieves a comprehensive set of statistics for the admin dashboard.

- **URL:** `/api/Statistics`
- **Method:** `GET`
- **Authorization:** This endpoint is protected and requires an `Admin` role. The JWT token must be included in the `Authorization` header.
  - `Authorization: Bearer {your_jwt_token}`

### Success Response

- **Code:** `200 OK`
- **Content:** An object containing a wide range of statistics.

#### Response Body Example

```json
{
    "data": {
        "totalUsers": 150,
        "totalOffers": 750,
        "totalCategories": 25,
        "recentUsers": [
            {
                "id": 10,
                "name": "Alice Johnson",
                "email": "alice.j@example.com",
                "createdAt": "2023-10-28T14:30:00Z"
            },
            {
                "id": 9,
                "name": "Bob Williams",
                "email": "bob.w@example.com",
                "createdAt": "2023-10-28T12:00:00Z"
            }
        ],
        "recentOffers": [
            {
                "id": 101,
                "name": "Handmade Leather Wallet",
                "price": 75.50,
                "createdAt": "2023-10-28T16:00:00Z"
            },
            {
                "id": 100,
                "name": "Vintage Wooden Chair",
                "price": 120.00,
                "createdAt": "2023-10-28T15:45:00Z"
            }
        ],
        "activeUsers": 140,
        "inactiveUsers": 10,
        "mostViewedOffers": [
            {
                "id": 50,
                "name": "Rare Stamp Collection",
                "numberOfViews": 2500,
                "createdAt": "2023-10-25T18:00:00Z"
            },
            {
                "id": 65,
                "name": "Signed Sports Memorabilia",
                "numberOfViews": 1800,
                "createdAt": "2023-10-24T20:00:00Z"
            }
        ]
    },
    "success": true,
    "message": "Statistics retrieved successfully"
}
```
