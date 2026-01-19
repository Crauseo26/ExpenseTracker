# Testing Guide - AI Service

## Prerequisites

1. **Configure your API Key**
   - Copy `env.example` to `.env`
   - Add your Gemini API Key to `.env`:
     ```bash
     GEMINI_API_KEY=your_actual_api_key_here
     ```
   - Get your API key from: https://aistudio.google.com/app/apikey

2. **Install Dependencies**
   ```bash
   pip install -r requirements.txt
   ```

## Running the Service

### Start the Server

```bash
python main.py
```

The service will start on `http://localhost:5000`

You should see:
```
INFO:     Started server process [xxxxx]
INFO:     Waiting for application startup.
INFO:     Application startup complete.
INFO:     Uvicorn running on http://0.0.0.0:5000 (Press CTRL+C to quit)
```

### Stop the Server

Press `CTRL+C` in the terminal where the service is running.

## Running Tests

### Automated Test Suite

In a **new terminal** (while the service is running):

```bash
python test_service.py
```

This will run 4 tests:
1. **Health Check** - Verifies the service is running
2. **Simple Expense** - Tests single expense extraction
3. **Multiple Expenses** - Tests extracting multiple expenses from one text
4. **Notification Format** - Tests bank SMS notification parsing

### Manual Testing with cURL

#### Health Check
```bash
curl http://localhost:5000/health
```

#### Process Text (Simple)
```bash
curl -X POST http://localhost:5000/process-text \
  -H "Content-Type: application/json" \
  -d "{\"rawText\": \"Compré en McDonald's por $450 pesos uruguayos\", \"inputType\": \"TEXT\", \"availableAccounts\": [\"Restaurants\", \"Supermarket\", \"Transportation\"]}"
```

#### Process Text (Multiple Expenses)
```bash
curl -X POST http://localhost:5000/process-text \
  -H "Content-Type: application/json" \
  -d "{\"rawText\": \"Gasté $200 en el supermercado y $150 en la farmacia\", \"inputType\": \"TEXT\"}"
```

### Manual Testing with PowerShell

#### Health Check
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/health" -Method Get
```

#### Process Text
```powershell
$body = @{
    rawText = "Compré en McDonald's por $450 pesos uruguayos"
    inputType = "TEXT"
    availableAccounts = @("Restaurants", "Supermarket", "Transportation")
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/process-text" -Method Post -Body $body -ContentType "application/json"
```

## Understanding Accounts

**IMPORTANT:** In this system, an "Account" is NOT a payment method (like Visa or Cash).

An **Account** is a logical category for grouping similar expenses:

- **Supermarket** - for grocery shopping
- **Restaurants** - for dining out
- **Rent** - for monthly rent payments
- **Internet** - for internet service bills
- **Transportation** - for taxi, bus, fuel
- **Utilities** - for electricity, water, gas
- **Healthcare** - for medical expenses, pharmacy
- **Entertainment** - for subscriptions (Netflix, Spotify), movies, etc.

When you provide `availableAccounts` in your request, the AI will try to suggest which category best fits each expense based on the merchant type and context.

## Interactive API Documentation

Once the service is running, visit:

- **Swagger UI**: http://localhost:5000/docs
- **ReDoc**: http://localhost:5000/redoc

These provide interactive API documentation where you can test endpoints directly in your browser.

## Troubleshooting

### Error: "GEMINI_API_KEY environment variable is required"

**Solution**: Make sure you have:
1. Created the `.env` file (copy from `env.example`)
2. Added your actual API key to `.env`
3. Restarted the service after creating/updating `.env`

### Error: "Could not connect to the service"

**Solution**: Make sure the service is running with `python main.py`

### Error: Connection timeout or slow responses

**Solution**: 
- Check your internet connection (Gemini API requires internet)
- Try using a faster model: `GEMINI_MODEL=gemini-1.5-flash` in `.env`
- Check Gemini API status: https://status.cloud.google.com/

### Error: Invalid API Key

**Solution**:
- Verify your API key is correct
- Check if the API key has the necessary permissions
- Generate a new key if needed: https://aistudio.google.com/app/apikey

## Configuration Options

Edit `.env` to customize:

```bash
# Server
HOST=0.0.0.0
PORT=5000

# LLM Provider
LLM_PROVIDER=gemini
GEMINI_API_KEY=your_key_here
GEMINI_MODEL=gemini-1.5-flash

# Available models:
# - gemini-1.5-flash (fast, recommended)
# - gemini-1.5-pro (more capable, slower)
# - gemini-2.0-flash-exp (experimental)

# Logging
LOG_LEVEL=INFO
```

## Expected Test Results

When running `test_service.py`, you should see output like:

```
>>> Starting AI Service Tests...
Target: http://localhost:5000

============================================================
TEST 1: Health Check
============================================================
Status Code: 200
[PASS] Health check passed!

============================================================
TEST 2: Simple Expense Extraction
============================================================
Status Code: 200
[PASS] Extracted 1 expense(s)!

Proposal 1:
  - Description: McDonald's
  - Amount: 450.0 UYU
  - Date: 2026-01-15
  - Type: SPORADIC
  - Confidence: 85.00%

...

============================================================
[SUCCESS] ALL TESTS COMPLETED!
============================================================
```
