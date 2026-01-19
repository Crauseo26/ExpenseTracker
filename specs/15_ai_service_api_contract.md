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
  "availableAccounts": ["string"],
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
      "currency": "UYU | USD | UNKNOWN",
      "purchaseDate": "YYYY-MM-DD",
      "expenseType": "SPORADIC | REPETITIVE | UNKNOWN",
      "confidence": 0.0 to 1.0,
      "metadata": {
        "merchant": "string",
        "rawExtraction": "string",
        "suggestedAccount": "string"
      }
    }
  ],
  "overallConfidence": 0.0 to 1.0,
  "processingTimeMs": 123
}
```

*Note: If `currency` or `expenseType` cannot be determined, the service returns "UNKNOWN".*

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

### 1. Deterministic Confidence Scoring
The AI Service calculates confidence based on data completeness, NOT by asking the LLM.
- **Base Score:** 1.0
- **Penalties:**
  - **-0.25** if `amount` is 0.
  - **-0.25** if `currency` is "UNKNOWN".
  - **-0.25** if `expenseType` is "UNKNOWN".
  - **-0.25** if `purchaseDate` was inferred (defaulted to today) rather than extracted.
- **Filtering:** Any proposal with a final score **< 0.5** is discarded and not returned to the Backend.

### 2. Default Values
- **Amount:** Defaults to `0` if undefined.
- **Currency:** Defaults to `UNKNOWN` if undefined.
- **ExpenseType:** Defaults to `UNKNOWN` if undefined.
- **Date:** Defaults to current date if undefined (but triggers penalty).

### 3. Multi-Expense
The service should be capable of detecting multiple expenses in a single text block.

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