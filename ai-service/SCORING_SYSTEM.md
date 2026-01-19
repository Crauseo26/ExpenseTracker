# Deterministic Confidence Scoring System

## Overview

This document describes the deterministic confidence scoring system implemented in the FinancIA AI Service for evaluating expense extraction proposals. The system calculates confidence scores based on **data completeness and quality**, rather than relying on LLM-generated confidence values.

---

## Philosophy & Design Decisions

### 1. **Honest LLM Approach**
The LLM is instructed to be **honest** and **not guess**. If it doesn't know a value:
- **Amount**: Returns `0`
- **Currency**: Returns `"UNKNOWN"`
- **ExpenseType**: Returns `"UNKNOWN"`
- **PurchaseDate**: Omits the field entirely (system defaults to today)

The LLM does **NOT** generate confidence scores. This responsibility belongs to the Python service.

### 2. **Deterministic Scoring**
Confidence is calculated using a **penalty-based system** starting from a perfect score of `1.0`. Each missing or low-quality field applies a specific penalty, making the scoring:
- **Transparent**: Easy to understand and debug
- **Consistent**: Same inputs always produce same scores
- **Tunable**: Penalties can be adjusted based on business needs

### 3. **Quality Over Guessing**
The system prioritizes **data quality** over quantity. It's better to return fewer high-quality proposals than many low-confidence hallucinations.

---

## Scoring Rules

### Base Score
All proposals start with a confidence score of **1.0** (100%).

### Penalties

#### **Data Completeness Penalties** (High Impact)
These penalties reflect missing or unknown critical data:

| Condition | Penalty | Rationale |
|-----------|---------|-----------|
| `amount == 0` | **-0.25** | Amount is the most critical field for an expense |
| `currency == "UNKNOWN"` | **-0.25** | Currency ambiguity makes the expense unusable |
| `expenseType == "UNKNOWN"` | **-0.25** | Type classification is important for categorization |

#### **Date Inference Penalty** (Low Impact)
| Condition | Penalty | Rationale |
|-----------|---------|-----------|
| `purchaseDate` was inferred (defaulted to today) | **-0.05** | Dates are often implicit; minor penalty reflects this reality |

**Design Decision**: Originally `-0.25`, reduced to `-0.05` in iteration 2 to be more permissive. Many legitimate expenses don't explicitly mention dates (e.g., "I bought groceries for $50").

#### **Description Quality Penalties** (Variable Impact)
These penalties assess the quality of the description field:

| Condition | Penalty | Rationale |
|-----------|---------|-----------|
| Description is **empty** or whitespace only | **-0.20** | No description makes the expense hard to identify |
| Description length **< 3 characters** | **-0.10** | Very short descriptions lack context (e.g., "A", "Ok") |
| Description matches **generic blacklist** | **-0.05** | Generic terms provide minimal information |

**Generic Description Blacklist**:
```python
["gasto", "compra", "pago", "ticket", "expense", "purchase", "payment", "shop"]
```

**Design Decision**: Generic penalty originally `-0.15`, reduced to `-0.05` in iteration 3. Generic descriptions like "gasto" are common in casual expense reporting and still provide some value.

---

## Filtering Threshold

Proposals with a final confidence score **< 0.5** are **discarded** and not returned to the backend.

**Rationale**: A score below 0.5 indicates the proposal has at least 2 major penalties or multiple minor issues, suggesting the data is too incomplete or low-quality to be useful.

---

## Scoring Examples

### Example 1: Perfect Proposal
```json
{
  "description": "McDonald's",
  "amount": 450,
  "currency": "UYU",
  "expenseType": "SPORADIC",
  "purchaseDate": "2026-01-15"
}
```
**Score**: `1.0` (no penalties)

---

### Example 2: Valid Expense with Inferred Date
```json
{
  "description": "Supermarket groceries",
  "amount": 1200,
  "currency": "UYU",
  "expenseType": "SPORADIC",
  "purchaseDate": "2026-01-19"  // inferred (today)
}
```
**Score**: `1.0 - 0.05 = 0.95`
- Date was inferred: `-0.05`

---

### Example 3: Generic Description + Inferred Date
```json
{
  "description": "Gasto",
  "amount": 500,
  "currency": "UYU",
  "expenseType": "SPORADIC",
  "purchaseDate": "2026-01-19"  // inferred
}
```
**Score**: `1.0 - 0.05 - 0.05 = 0.90`
- Generic description ("gasto"): `-0.05`
- Date was inferred: `-0.05`

---

### Example 4: Short Description + Inferred Date
```json
{
  "description": "A",
  "amount": 300,
  "currency": "UYU",
  "expenseType": "SPORADIC",
  "purchaseDate": "2026-01-19"  // inferred
}
```
**Score**: `1.0 - 0.10 - 0.05 = 0.85`
- Short description (< 3 chars): `-0.10`
- Date was inferred: `-0.05`

---

### Example 5: Missing Amount
```json
{
  "description": "Coffee shop",
  "amount": 0,
  "currency": "UYU",
  "expenseType": "SPORADIC",
  "purchaseDate": "2026-01-15"
}
```
**Score**: `1.0 - 0.25 = 0.75`
- Amount is 0: `-0.25`

---

### Example 6: Unknown Currency and Type
```json
{
  "description": "Online purchase",
  "amount": 50,
  "currency": "UNKNOWN",
  "expenseType": "UNKNOWN",
  "purchaseDate": "2026-01-15"
}
```
**Score**: `1.0 - 0.25 - 0.25 = 0.50` (borderline, passes filter)
- Unknown currency: `-0.25`
- Unknown type: `-0.25`

---

### Example 7: Multiple Major Issues (Filtered Out)
```json
{
  "description": "Below",
  "amount": 0,
  "currency": "UNKNOWN",
  "expenseType": "UNKNOWN",
  "purchaseDate": "2026-01-19"
}
```
**Score**: `1.0 - 0.25 - 0.25 - 0.25 = 0.25` ❌ **FILTERED OUT** (< 0.5)
- Amount is 0: `-0.25`
- Unknown currency: `-0.25`
- Unknown type: `-0.25`

---

### Example 8: Empty Description + All Defaults (Filtered Out)
```json
{
  "description": "",
  "amount": 0,
  "currency": "UNKNOWN",
  "expenseType": "UNKNOWN",
  "purchaseDate": "2026-01-19"  // inferred
}
```
**Score**: `1.0 - 0.20 - 0.25 - 0.25 - 0.25 - 0.05 = 0.00` ❌ **FILTERED OUT**
- Empty description: `-0.20`
- Amount is 0: `-0.25`
- Unknown currency: `-0.25`
- Unknown type: `-0.25`
- Date was inferred: `-0.05`

---

## Implementation Details

### Location
- **Module**: `ai-service/app/services/refinement.py`
- **Function**: `calculate_confidence(proposal, was_date_inferred)`
- **Filter Function**: `refine_and_filter(proposals, raw_text)`

### Integration Flow
1. **LLM Response**: Gemini returns proposals without confidence scores
2. **Refinement**: `refine_and_filter()` processes each proposal:
   - Applies default values for missing fields
   - Detects date inference
   - Calculates confidence score
   - Filters out proposals with score < 0.5
3. **API Response**: Only high-quality proposals (≥ 0.5) are returned

---

## Evolution History

### Version 1: Initial Implementation (F09-T03)
- Base score: 1.0
- All penalties: -0.25 (amount, currency, type, date)
- No description quality checks
- Filter threshold: 0.5

### Version 2: Adjusted Date Penalty
- **Change**: Date penalty reduced from `-0.25` to `-0.05`
- **Rationale**: Dates are often implicit in natural language; being too strict filtered out valid expenses
- Added description quality checks:
  - Empty: `-0.20`
  - Short (< 3 chars): `-0.10`
  - Generic (blacklist): `-0.15`

### Version 3: Refined Generic Penalty (Current)
- **Change**: Generic description penalty reduced from `-0.15` to `-0.05`
- **Rationale**: Generic terms like "gasto" or "compra" are common in casual Spanish expense reporting and still provide value

---

## Testing

Comprehensive unit tests verify all scoring scenarios:
- **Location**: `ai-service/test_refinement_unit.py`
- **Coverage**:
  - All individual penalties
  - Penalty combinations
  - Blacklist word matching (case-insensitive)
  - Filtering threshold behavior
  - Edge cases (empty descriptions, multiple penalties)

Run tests:
```bash
python test_refinement_unit.py
```

---

## Future Considerations

### Potential Adjustments
1. **Dynamic Thresholds**: Adjust filter threshold based on input type (TEXT vs NOTIFICATION)
2. **Contextual Penalties**: Different penalties for different expense categories
3. **Positive Signals**: Add bonuses for high-quality data (e.g., merchant detected, account suggested)
4. **Learning from Feedback**: Adjust penalties based on user acceptance rates

### Metrics to Monitor
- **Acceptance Rate**: % of proposals accepted by users
- **False Positives**: High-confidence proposals rejected by users
- **False Negatives**: Valid expenses filtered out (score < 0.5)
- **Score Distribution**: Histogram of confidence scores

---

## Contact & Maintenance

For questions or proposed changes to the scoring system:
1. Review this document
2. Run existing tests to understand current behavior
3. Propose changes with clear rationale
4. Update tests to reflect new expectations
5. Update this document with version history

**Last Updated**: 2026-01-19  
**Current Version**: 3 (Generic penalty -0.05)
