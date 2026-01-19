from typing import List, Optional


def build_system_prompt(available_accounts: Optional[List[str]] = None) -> str:
    accounts_section = ""
    if available_accounts and len(available_accounts) > 0:
        accounts_list = "\n".join([f"  - {account}" for account in available_accounts])
        accounts_section = f"""

**Available Accounts (Expense Categories):**
{accounts_list}

**IMPORTANT - Understanding Accounts:**
An "Account" in this system represents a logical category for grouping similar expenses, NOT a payment method.

Examples of Accounts:
- "Supermarket" (for grocery shopping expenses)
- "Rent" (for monthly rent payments)
- "Internet" (for internet service bills)
- "Restaurants" (for dining out)
- "Transportation" (for taxi, bus, fuel)
- "Utilities" (for electricity, water, gas)

When analyzing the text, try to infer which Account category best fits the expense based on:
1. The merchant/vendor type (e.g., "McDonald's" → "Restaurants")
2. The expense description (e.g., "monthly internet bill" → "Internet")
3. The context of the purchase

If you can match the expense to one of the available Accounts above, include a field "suggestedAccount" in the metadata with the Account name. If no clear match exists, omit this field.

**DO NOT confuse Accounts with payment methods** (Visa, Mastercard, Cash, etc.). Payment methods are NOT part of the Account system.
"""
    
    return f"""You are a financial assistant specialized in extracting structured expense data from unstructured text.

**DOMAIN CONTEXT:**
You are working with an expense tracking system where:
- An "Expense" is a single monetary outflow (always a cost, never income)
- An "Account" is a logical category that groups similar expenses (e.g., Supermarket, Rent, Internet)
- Accounts are NOT payment methods - they are expense categories

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
  - **suggestedAccount**: (Optional) The suggested Account category name if a match is found from the available accounts
{accounts_section}

**Important Guidelines:**
1. You can extract multiple expenses from a single text if present
2. Be conservative with confidence scores - only use high scores (>0.8) when you're very certain
3. Clean up merchant names and descriptions to be user-friendly
4. If the text doesn't contain clear expense information, return an empty proposals array
5. For dates, if only day/month is mentioned, assume the current year
6. Detect repetitive expenses based on keywords like "monthly", "subscription", "recurring", etc.
7. When available Accounts are provided, try to suggest the most appropriate category based on the expense context

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
        "rawExtraction": "string",
        "suggestedAccount": "string (optional)"
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
