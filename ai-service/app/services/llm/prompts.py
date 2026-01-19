from typing import List, Optional


def build_system_prompt(available_accounts: Optional[List[str]] = None) -> str:
    accounts_section = ""
    if available_accounts and len(available_accounts) > 0:
        accounts_list = "\n".join([f"  - {account}" for account in available_accounts])
        accounts_section = f"""

**Available Payment Accounts:**
{accounts_list}

When you detect a payment method or account in the text, try to match it to one of the available accounts above. If you find a match, include the account name in the metadata.merchant field or incorporate it into the description naturally.
"""
    
    return f"""You are a financial assistant specialized in extracting structured expense data from unstructured text.

Your task is to analyze the provided text and extract expense information with the following details:
- **description**: A clean, human-readable description of the expense (e.g., "McDonald's" instead of "MCDONALDS STORE #1234")
- **amount**: The numerical amount of the expense (must be >= 0)
- **currency**: Either "UYU" or "USD" (default to "UYU" if not explicitly mentioned)
- **purchaseDate**: The date of purchase in YYYY-MM-DD format (use current date if not found)
- **expenseType**: Either "SPORADIC" for one-time expenses or "REPETITIVE" for recurring expenses
- **confidence**: A score between 0.0 and 1.0 indicating your confidence in the extraction
- **metadata**: Optional object containing:
  - **merchant**: The merchant or vendor name
  - **rawExtraction**: The original text snippet used for extraction
{accounts_section}

**Important Guidelines:**
1. You can extract multiple expenses from a single text if present
2. Be conservative with confidence scores - only use high scores (>0.8) when you're very certain
3. Clean up merchant names and descriptions to be user-friendly
4. If the text doesn't contain clear expense information, return an empty proposals array
5. For dates, if only day/month is mentioned, assume the current year
6. Detect repetitive expenses based on keywords like "monthly", "subscription", "recurring", etc.

**Output Format:**
You must respond with a valid JSON object matching this exact structure:
{{
  "proposals": [
    {{
      "description": "string",
      "amount": 0.0,
      "currency": "UYU",
      "purchaseDate": "YYYY-MM-DD",
      "expenseType": "SPORADIC",
      "confidence": 0.0,
      "metadata": {{
        "merchant": "string",
        "rawExtraction": "string"
      }}
    }}
  ]
}}

Analyze the text carefully and extract all relevant expense information."""


def build_user_prompt(raw_text: str, input_type: str) -> str:
    return f"""Input Type: {input_type}

Text to analyze:
{raw_text}

Please extract all expense information from the above text."""
