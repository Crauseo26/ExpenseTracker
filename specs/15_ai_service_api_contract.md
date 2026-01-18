# AI Service API Contract

## Purpose
This document defines the REST API contract between the **.NET Backend** (Consumer) and the **Python AI Service** (Provider).

---

## Base URL
The service is expected to run at: `http://localhost:5000` (configurable via Backend settings).

---

## Endpoints

### 1. Process Text Input
Extract structured expense proposals from raw text.

- **URL:** `/process-text`
- **Method:** `POST`
- **Content-Type:** `application/json`

**Request Body:**
```json
{
  "rawText": "string",
  "inputType": "TEXT | NOTIFICATION | EMAIL",
  "metadata": {
    "receivedAt": "ISO-8601 string"
  }
}
```

**Successful Response (200 OK):**
```json
{
  "proposals": [
    {
      "description": "string",
      "amount": 0.00,
      "currency": "UYU | USD",
      "purchaseDate": "YYYY-MM-DD",
      "expenseType": "SPORADIC | REPETITIVE",
      "confidence": 0.0 to 1.0,
      "metadata": {
        "merchant": "string",
        "rawExtraction": "string"
      }
    }
  ],
  "overallConfidence": 0.0 to 1.0,
  "processingTimeMs": 123
}
```

**Error Response (400 Bad Request / 500 Internal Server Error):**
```json
{
  "error": {
    "code": "string",
    "message": "string",
    "details": {}
  }
}
```

---

## Domain Rules & Constraints

1. **Confidence Normalization:** The `confidence` score must be a float between `0.0` and `1.0`.
2. **Currency Detection:** If the currency cannot be detected, it should default to `UYU` (per project context) or be omitted if the backend allows.
3. **Date Parsing:** `purchaseDate` should be in `YYYY-MM-DD` format. If not explicitly found, use the current date or `receivedAt` if provided.
4. **Multi-Expense:** The service should be capable of detecting multiple expenses in a single text block and returning multiple proposals.
5. **Descriptions:** The description should be clean (e.g., "McDonalds" instead of "MCDONALDS STORE #1234").

---

## Health Check
Verify the service is up and running.

- **URL:** `/health`
- **Method:** `GET`

**Response (200 OK):**
```json
{
  "status": "healthy",
  "version": "1.0.0"
}
```
