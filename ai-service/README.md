# FinancIA AI Service

Python-based AI processing service for extracting structured expense data from unstructured text inputs.

## Overview

This service provides REST endpoints for processing raw text (from notifications, emails, or manual input) and extracting structured expense proposals using AI/LLM orchestration.

## Tech Stack

- **Python 3.12+**
- **FastAPI** - Modern web framework for building APIs
- **Uvicorn** - ASGI server
- **Pydantic v2** - Data validation and settings management

## Project Structure

```
ai-service/
├── main.py              # FastAPI application entry point
├── requirements.txt     # Python dependencies
├── env.example         # Environment variables template
└── README.md           # This file
```

## Setup

### 1. Create Virtual Environment

```bash
python -m venv venv
```

### 2. Activate Virtual Environment

**Windows:**
```bash
venv\Scripts\activate
```

**Linux/Mac:**
```bash
source venv/bin/activate
```

### 3. Install Dependencies

```bash
pip install -r requirements.txt
```

### 4. Configure Environment (Optional)

```bash
cp env.example .env
# Edit .env with your configuration
```

## Running the Service

### Development Mode

```bash
uvicorn main:app --reload --host 0.0.0.0 --port 5000
```

Or simply:

```bash
python main.py
```

### Production Mode

```bash
uvicorn main:app --host 0.0.0.0 --port 5000 --workers 4
```

## API Endpoints

### Health Check

```
GET /health
```

Returns service health status and version.

**Response:**
```json
{
  "status": "healthy",
  "version": "1.0.0"
}
```

### Process Text

```
POST /process-text
```

Process raw text input and extract expense proposals.

**Request:**
```json
{
  "rawText": "Spent $50 at McDonalds yesterday",
  "inputType": "TEXT",
  "metadata": {
    "receivedAt": "2026-01-18T19:00:00Z"
  }
}
```

**Response:**
```json
{
  "proposals": [
    {
      "description": "McDonalds",
      "amount": 50.0,
      "currency": "USD",
      "purchaseDate": "2026-01-17",
      "expenseType": "SPORADIC",
      "confidence": 0.92,
      "metadata": {
        "merchant": "McDonalds",
        "rawExtraction": "Spent $50 at McDonalds yesterday"
      }
    }
  ],
  "overallConfidence": 0.92,
  "processingTimeMs": 123
}
```

## API Documentation

Once the service is running, visit:

- **Swagger UI:** http://localhost:5000/docs
- **ReDoc:** http://localhost:5000/redoc

## Development Status

### ✅ Completed
- FastAPI skeleton setup
- `/health` endpoint implementation
- `/process-text` endpoint structure
- Request/response models with Pydantic validation
- Basic error handling

### 🔄 In Progress
- LLM orchestration implementation
- Confidence scoring logic
- Multi-expense detection

### 📝 Pending
- AI provider integration (OpenAI/Gemini)
- Advanced text parsing
- Unit tests with Pytest
- Docker containerization

## Contract Compliance

This service implements the API contract defined in `specs/15_ai_service_api_contract.md`.

## Architecture

The service follows these principles:
- **Stateless:** No session or domain data persistence
- **Contract-first:** Strict adherence to defined schemas
- **Fail-safe:** Graceful error handling with meaningful messages
- **Replaceable:** AI provider can be swapped without changing the contract

## References

- API Contract: `specs/15_ai_service_api_contract.md`
- AI Pipeline: `specs/07_ai_pipeline.md`
- Constraints: `specs/02_constraints.md`
