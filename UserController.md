# User API Documentation

This document outlines the API endpoints available in the UserController.

## Base URL

```
/api/User
```

## Authentication

Some endpoints require authentication using a JWT token in the Authorization header.

## Endpoints

### Register User

Register a new user in the system.

- **URL**: `/register`
- **Method**: `POST`
- **Auth Required**: No

**Request Body**:
```json
{
  "name": "string",
  "phoneNumber": "string"
}
```

**Response**:
```json
{
  "success": true,
  "message": {
    "arabic": "تم تسجيل المستخدم بنجاح",
    "english": "User registered successfully"
  },
  "data": {
    // User object
  }
}
```

**Error Responses**:
- Phone number already registered:
```json
{
  "success": false,
  "message": {
    "arabic": "رقم الهاتف مستخدم بالفعل",
    "english": "Phone number already registered"
  }
}
```

### Login

Login with phone number to receive OTP.

- **URL**: `/login`
- **Method**: `POST`
- **Auth Required**: No

**Request Body**:
```json
{
  "phoneNumber": "string"
}
```

**Response**:
```json
{
  "success": true,
  "message": {
    "arabic": "تم إرسال رمز التحقق",
    "english": "Verification code has been sent"
  },
  "data": {
    "userId": 123
  }
}
```

**Error Responses**:
- Phone number not registered:
```json
{
  "success": false,
  "message": {
    "arabic": "رقم الهاتف غير مسجل",
    "english": "Phone number not registered"
  }
}
```

### Verify OTP

Verify the OTP sent during login to complete authentication.

- **URL**: `/verify-otp`
- **Method**: `POST`
- **Auth Required**: No

**Request Body**:
```json
{
  "userId": 123,
  "otp": "123456"
}
```

**Response**:
```json
{
  "success": true,
  "message": {
    "arabic": "تم تسجيل الدخول بنجاح",
    "english": "Login successful"
  },
  "data": {
    "token": "JWT_TOKEN",
    "user": {
      // User object
    }
  }
}
```

**Error Responses**:
- Invalid OTP:
```json
{
  "success": false,
  "message": {
    "arabic": "رمز التحقق غير صحيح",
    "english": "Invalid verification code"
  }
}
```

### Get User Profile

Get the current authenticated user's profile information.

- **URL**: `/profile`
- **Method**: `GET`
- **Auth Required**: Yes (User)

**Response**:
```json
{
  "success": true,
  "message": {
    "arabic": "تم جلب معلومات المستخدم بنجاح",
    "english": "User profile retrieved successfully"
  },
  "data": {
    "id": 123,
    "name": "string",
    "phoneNumber": "string",
    "userType": 0,
    "profilePhotoUrl": "string"
  }
}
```

**Error Responses**:
- User not found:
```json
{
  "success": false,
  "message": {
    "arabic": "المستخدم غير موجود",
    "english": "User not found"
  }
}
```

### Search Users

Search for users (Admin only).

- **URL**: `/search`
- **Method**: `GET`
- **Auth Required**: Yes (Admin)

**Query Parameters**:
- `page` (integer, default: 1) - Page number
- `pageSize` (integer, default: 10) - Number of items per page
- `searchTerm` (string, optional) - Search by name or phone number

**Response**:
```json
{
  "success": true,
  "message": {
    "arabic": "تم جلب المستخدمين بنجاح",
    "english": "Users retrieved successfully"
  },
  "data": {
    "items": [
      {
        "id": 123,
        "name": "string",
        "phoneNumber": "string",
        "userType": 0,
        "profilePhotoUrl": "string",
        "createdAt": "2023-05-01T12:00:00Z"
      }
    ],
    "totalCount": 100,
    "page": 1,
    "pageSize": 10,
    "totalPages": 10
  }
}
``` 