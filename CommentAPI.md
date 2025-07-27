# Mazad Comment API Documentation

## Table of Contents
- [Create Comment](#create-comment)
- [Get Offer Comments](#get-offer-comments)
- [Get Comment Replies](#get-comment-replies)

## Create Comment

Creates a new comment on an offer or replies to an existing comment.

### Endpoint

```
POST /api/offers/{offerId}/comments
```

### Authorization

Requires authentication with `User` policy.

### Path Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| offerId   | int  | Yes      | ID of the offer to comment on |

### Request Body

```json
{
  "comment": "string",
  "replyToCommentId": null or int
}
```

| Field           | Type       | Required | Description |
|----------------|------------|----------|-------------|
| comment        | string     | Yes      | The comment text |
| replyToCommentId | int?     | No       | ID of the comment being replied to (null for top-level comments) |

### Example Request

```http
POST /api/offers/123/comments
Content-Type: application/json
Authorization: Bearer <token>

{
  "comment": "This is a great offer!",
  "replyToCommentId": null
}
```

### Response

```json
{
  "success": true,
  "message": "Comment added successfully",
  "data": {
    "id": 456,
    "comment": "This is a great offer!",
    "offerId": 123,
    "userId": 789,
    "userName": "John Doe",
    "userProfilePhotoUrl": "https://example.com/profile.jpg",
    "replyToCommentId": null,
    "createdAt": "2023-08-15T14:30:45Z",
    "repliesCount": 0
  }
}
```

### Error Responses

**Offer not found:**
```json
{
  "success": false,
  "message": "Offer not found",
  "exception": "",
  "stackTrace": ""
}
```

**Parent comment not found:**
```json
{
  "success": false,
  "message": "Parent comment not found",
  "exception": "",
  "stackTrace": ""
}
```

## Get Offer Comments

Retrieves top-level comments for a specific offer with cursor-based pagination.

### Endpoint

```
GET /api/offers/{offerId}/comments
```

### Path Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| offerId   | int  | Yes      | ID of the offer to get comments for |

### Query Parameters

| Parameter      | Type    | Required | Default | Description |
|---------------|---------|----------|---------|-------------|
| cursor        | int?    | No       | null    | ID of the last comment from previous page |
| limit         | int     | No       | 10      | Number of comments to return (max 50) |
| sortDescending | boolean | No       | true    | Sort by newest comments first if true |

### Example Request

```http
GET /api/offers/123/comments?limit=20&sortDescending=true
```

### Response

```json
{
  "success": true,
  "message": "Comments retrieved successfully",
  "data": {
    "items": [
      {
        "id": 456,
        "comment": "This is a great offer!",
        "offerId": 123,
        "userId": 789,
        "userName": "John Doe",
        "userProfilePhotoUrl": "https://example.com/profile.jpg",
        "replyToCommentId": null,
        "createdAt": "2023-08-15T14:30:45Z",
        "repliesCount": 2
      },
      // More comments...
    ],
    "hasMore": true,
    "nextCursor": 442
  }
}
```

### Error Response

**Offer not found:**
```json
{
  "success": false,
  "message": "Offer not found",
  "exception": "",
  "stackTrace": ""
}
```

## Get Comment Replies

Retrieves replies to a specific comment with cursor-based pagination.

### Endpoint

```
GET /api/offers/comments/{commentId}/replies
```

### Path Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| commentId | int  | Yes      | ID of the comment to get replies for |

### Query Parameters

| Parameter      | Type    | Required | Default | Description |
|---------------|---------|----------|---------|-------------|
| cursor        | int?    | No       | null    | ID of the last reply from previous page |
| limit         | int     | No       | 10      | Number of replies to return (max 50) |
| sortDescending | boolean | No       | true    | Sort by newest replies first if true |

### Example Request

```http
GET /api/offers/comments/456/replies?limit=20&sortDescending=true
```

### Response

```json
{
  "success": true,
  "message": "Replies retrieved successfully",
  "data": {
    "items": [
      {
        "id": 789,
        "comment": "I agree with you!",
        "offerId": 123,
        "userId": 101,
        "userName": "Jane Smith",
        "userProfilePhotoUrl": "https://example.com/jane.jpg",
        "replyToCommentId": 456,
        "createdAt": "2023-08-15T15:20:10Z",
        "repliesCount": 0
      },
      // More replies...
    ],
    "hasMore": false,
    "nextCursor": null
  }
}
```

### Error Response

**Comment not found:**
```json
{
  "success": false,
  "message": "Comment not found",
  "exception": "",
  "stackTrace": ""
}
``` 