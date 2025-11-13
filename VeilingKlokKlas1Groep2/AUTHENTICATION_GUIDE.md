# VeilingKlok API - Authentication & Authorization Guide

## 🔐 Overview

This API implements **JWT (JSON Web Token) authentication** with **refresh tokens stored in secure HTTP-only cookies** for maximum security. The system includes:

- **Access Tokens** (15 minutes) - Short-lived tokens for API requests
- **Refresh Tokens** (7 days) - Stored in secure HTTP-only cookies (not accessible to JavaScript)
- **Password Hashing** - PBKDF2 algorithm with salt
- **Strong Password Validation** - Enforced password requirements
- **Role-Based Access Control** - Account type restrictions (Koper, Kweker, Veilingmeester)
- **Owner-Based Authorization** - Users can only access their own data
- **XSS Protection** - HttpOnly cookies prevent JavaScript access to refresh tokens
- **CSRF Protection** - SameSite=Strict cookie attribute

---

## 📦 Required NuGet Packages

Add these packages to your `.csproj` file:

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.2.1" />
```

Then run:

```bash
dotnet restore
```

---

## ⚙️ Configuration

### 1. Update `appsettings.json`

```json
{
	"JwtSettings": {
		"Secret": "YourSuperSecretKeyForJWT_MustBeAtLeast32CharactersLong!",
		"Issuer": "VeilingKlokApp",
		"Audience": "VeilingKlokApp",
		"AccessTokenExpirationMinutes": 15,
		"RefreshTokenExpirationDays": 7
	}
}
```

**⚠️ IMPORTANT:** Change the `Secret` to a random, secure key in production!

### 2. Run Database Migration

```bash
dotnet ef migrations add AddRefreshTokens
dotnet ef database update
```

---

## 🚀 API Endpoints

### Authentication Endpoints

#### 1. **Login** (Public)

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecureP@ssw0rd"
}
```

**Response:**

```json
{
	"accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
	"refreshToken": null,
	"accessTokenExpiresAt": "2025-11-12T15:30:00Z",
	"refreshTokenExpiresAt": "2025-11-19T15:00:00Z",
	"accountId": 1,
	"email": "user@example.com",
	"accountType": "Koper"
}
```

**⚠️ Note:** The refresh token is **NOT** in the response body. It's automatically stored in a secure HTTP-only cookie that JavaScript cannot access.

**Cookie Set (automatically):**

```
Set-Cookie: refreshToken=abc123...; HttpOnly; Secure; SameSite=Strict; Expires=...
```

#### 2. **Refresh Token** (Public)

**⚠️ Important:** No request body needed! The refresh token is automatically sent from the cookie.

```http
POST /api/auth/refresh
Cookie: refreshToken=abc123...
```

**Response:** Same as login (new access token + new refresh token in cookie)

#### 3. **Logout** (Public)

**⚠️ Important:** No request body needed! The refresh token is automatically sent from the cookie.

```http
POST /api/auth/logout
Cookie: refreshToken=abc123...
```

**Response:**

```json
{
	"message": "Logout successful"
}
```

The refresh token cookie is automatically deleted by the server.

#### 4. **Revoke All Tokens** (Protected)

```http
POST /api/auth/revoke-all
Authorization: Bearer {accessToken}
Cookie: refreshToken=abc123...
```

---

## 🔑 Using Access Tokens

Include the access token in the `Authorization` header for all protected endpoints:

```http
GET /api/koper/1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**⚠️ Frontend Note:** You must include `credentials: 'include'` in fetch requests to send/receive cookies!

```javascript
fetch('/api/auth/login', {
	method: 'POST',
	credentials: 'include', // Required for cookies!
	body: JSON.stringify({ email, password }),
});
```

### Example with cURL:

```bash
curl -X GET "https://localhost:5001/api/koper/1" \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  --cookie-jar cookies.txt \
  --cookie cookies.txt
```

### Example with JavaScript (Fetch):

```javascript
const accessToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...';

fetch('https://localhost:5001/api/koper/1', {
	method: 'GET',
	headers: {
		Authorization: `Bearer ${accessToken}`,
		'Content-Type': 'application/json',
	},
})
	.then((response) => response.json())
	.then((data) => console.log(data));
```

---

## 🔒 Password Requirements

All passwords must meet these requirements:

- ✅ Minimum 8 characters
- ✅ At least 1 uppercase letter (A-Z)
- ✅ At least 1 lowercase letter (a-z)
- ✅ At least 1 digit (0-9)
- ✅ At least 1 special character (!@#$%^&\*()\_+-=[]{}etc.)

**Valid Example:** `SecureP@ssw0rd123`

---

## 🛡️ Authorization Attributes

### 1. `[Authorize]` - Require Authentication

```csharp
[HttpGet("protected")]
[VeilingKlokKlas1Groep2.Attributes.Authorize]
public IActionResult ProtectedEndpoint()
{
    // Only authenticated users can access
}
```

### 2. `[AuthorizeAccountType]` - Role-Based Access

```csharp
[HttpGet("kweker-only")]
[VeilingKlokKlas1Groep2.Attributes.AuthorizeAccountType("Kweker")]
public IActionResult KwekerOnly()
{
    // Only Kweker accounts can access
}

// Multiple roles allowed
[HttpPost("create-product")]
[VeilingKlokKlas1Groep2.Attributes.AuthorizeAccountType("Kweker", "Veilingmeester")]
public IActionResult CreateProduct()
{
    // Both Kweker and Veilingmeester can access
}
```

### 3. `[AuthorizeOwner]` - Owner-Only Access

```csharp
[HttpGet("{accountId}")]
[VeilingKlokKlas1Groep2.Attributes.AuthorizeOwner("accountId")]
public IActionResult GetAccount(int accountId)
{
    // Users can only access their own account (accountId must match token)
}
```

### 4. **Combining Attributes**

```csharp
[HttpPut("update/{accountId}")]
[VeilingKlokKlas1Groep2.Attributes.AuthorizeOwner("accountId")]
[VeilingKlokKlas1Groep2.Attributes.AuthorizeAccountType("Koper")]
public IActionResult UpdateKoperAccount(int accountId, UpdateKoperProfile data)
{
    // Only Koper accounts can update their OWN profile
}
```

---

## 📋 Account Types

The system supports three account types:

| Account Type       | Description | Access Level                                       |
| ------------------ | ----------- | -------------------------------------------------- |
| **Koper**          | Buyer       | Can view/update own profile, view/create orders    |
| **Kweker**         | Grower      | Can view/update own profile, manage products       |
| **Veilingmeester** | Auctioneer  | Can view/update own profile, manage auction clocks |

---

## 🔄 Token Flow

### Initial Login

```
1. User submits email + password
2. Server validates credentials
3. Server generates access token (15 min) + refresh token (7 days)
4. Client stores both tokens
5. Client uses access token for API requests
```

### Token Refresh

```
1. Access token expires (15 min)
2. Client sends refresh token to /api/auth/refresh
3. Server validates refresh token
4. Server generates NEW access token + NEW refresh token
5. Server revokes old refresh token (token rotation)
6. Client updates stored tokens
```

### Security Best Practices

- ✅ Store tokens in **httpOnly cookies** (server-side) or **secure storage** (mobile apps)
- ❌ Never store tokens in localStorage (vulnerable to XSS)
- ✅ Use HTTPS in production
- ✅ Implement token rotation (already done)
- ✅ Revoke all tokens on password change

---

## 🧪 Testing with Swagger

1. **Start the application**

```bash
dotnet run
```

2. **Navigate to Swagger UI**

```
https://localhost:5001/swagger
```

3. **Login to get tokens**

   - Use `POST /api/auth/login`
   - Copy the `accessToken` from the response

4. **Authorize in Swagger**

   - Click the **🔒 Authorize** button (top right)
   - Enter: `Bearer YOUR_ACCESS_TOKEN`
   - Click **Authorize**

5. **Test protected endpoints**
   - All requests now include the authorization header automatically

---

## 💡 Getting User Info in Controllers

Access authenticated user information in your controllers:

```csharp
[HttpGet("my-info")]
[VeilingKlokKlas1Groep2.Attributes.Authorize]
public IActionResult GetMyInfo()
{
    // Get account ID from token
    var accountId = HttpContext.Items["AccountId"] as int?;

    // Get account type from token
    var accountType = HttpContext.Items["AccountType"] as string;

    return Ok(new { accountId, accountType });
}
```

---

## 🐛 Common Errors

### 401 Unauthorized

```json
{
	"name": "Unauthorized",
	"message": "Access token is required. Please provide a valid token in the Authorization header",
	"code": 401
}
```

**Solution:** Include `Authorization: Bearer {token}` header

### 403 Forbidden

```json
{
	"name": "Forbidden",
	"message": "You can only access your own account information",
	"code": 403
}
```

**Solution:** You're trying to access another user's data

### Token Expired

```json
{
	"name": "Unauthorized",
	"message": "Invalid or expired access token",
	"code": 401
}
```

**Solution:** Use refresh token endpoint to get a new access token

---

## 📚 Example Workflow

### 1. Register a New Koper Account

```http
POST /api/koper/create
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecureP@ss123",
  "firstName": "John",
  "lastName": "Doe",
  "adress": "123 Main St",
  "postCode": "12345",
  "regio": "Noord"
}
```

### 2. Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecureP@ss123"
}
```

### 3. Access Protected Resource

```http
GET /api/koper/1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 4. Refresh Token When Expired

```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "8xF3kLm9pQ2vR7sT1uY5wZ6..."
}
```

### 5. Logout

```http
POST /api/auth/logout
Content-Type: application/json

{
  "refreshToken": "8xF3kLm9pQ2vR7sT1uY5wZ6..."
}
```

---

## 🔐 Security Features Implemented

✅ **Password Hashing** - PBKDF2 with salt (10,000 iterations)  
✅ **Strong Password Validation** - Enforced complexity requirements  
✅ **JWT Access Tokens** - Short-lived (15 minutes)  
✅ **Refresh Token Rotation** - Old tokens automatically revoked  
✅ **Role-Based Access Control** - Account type restrictions  
✅ **Owner-Based Authorization** - Users can only access their own data  
✅ **Constant-Time Password Comparison** - Prevents timing attacks  
✅ **HTTPS Enforcement** - In production  
✅ **Token Expiration** - Automatic validation  
✅ **Swagger JWT Support** - Easy testing with authentication

---

## 📝 Notes

- **Access tokens** expire after 15 minutes (configurable)
- **Refresh tokens** expire after 7 days (configurable)
- Passwords are **never stored in plain text**
- All authentication errors return **generic messages** to prevent user enumeration
- Token rotation ensures **old refresh tokens cannot be reused**

---

## 🎯 Next Steps

1. Install the required NuGet packages
2. Run the database migration
3. Test authentication endpoints in Swagger
4. Implement frontend token management
5. Consider adding email verification for production
6. Implement "Forgot Password" functionality
7. Add rate limiting to prevent brute force attacks

---

**Happy Coding! 🚀**
