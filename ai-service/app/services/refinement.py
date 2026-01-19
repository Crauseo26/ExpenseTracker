from typing import List, Dict, Any, Optional
from datetime import datetime


GENERIC_DESCRIPTION_BLACKLIST = [
    "gasto", "compra", "pago", "ticket",
    "expense", "purchase", "payment", "shop"
]


class PenaltyConfig:
    """Configuration for scoring penalties"""
    def __init__(
        self,
        amount_zero: float = 0.25,
        currency_unknown: float = 0.25,
        expense_type_unknown: float = 0.25,
        date_inferred: float = 0.05,
        description_empty: float = 0.20,
        description_short: float = 0.10,
        description_generic: float = 0.05
    ):
        self.amount_zero = amount_zero
        self.currency_unknown = currency_unknown
        self.expense_type_unknown = expense_type_unknown
        self.date_inferred = date_inferred
        self.description_empty = description_empty
        self.description_short = description_short
        self.description_generic = description_generic


def calculate_confidence(
    proposal: Dict[str, Any],
    was_date_inferred: bool,
    penalties: Optional[PenaltyConfig] = None
) -> float:
    """
    Calculate deterministic confidence score based on data completeness and quality.
    
    Base score: 1.0
    Penalties (configurable):
    - amount == 0
    - currency == "UNKNOWN"
    - expenseType == "UNKNOWN"
    - purchaseDate was inferred (defaulted to today)
    - description is empty or whitespace only
    - description length < 3 characters (and not empty)
    - description matches generic blacklist
    
    Args:
        proposal: The expense proposal dictionary from LLM
        was_date_inferred: Whether the date was inferred/defaulted
        penalties: Optional penalty configuration (uses defaults if not provided)
        
    Returns:
        Confidence score between 0.0 and 1.0
    """
    if penalties is None:
        penalties = PenaltyConfig()
    
    score = 1.0
    
    if proposal.get("amount", 0) == 0:
        score -= penalties.amount_zero
    
    if proposal.get("currency", "UNKNOWN") == "UNKNOWN":
        score -= penalties.currency_unknown
    
    if proposal.get("expenseType", "UNKNOWN") == "UNKNOWN":
        score -= penalties.expense_type_unknown
    
    if was_date_inferred:
        score -= penalties.date_inferred
    
    description = proposal.get("description", "")
    if not description or not description.strip():
        score -= penalties.description_empty
    elif len(description) < 3:
        score -= penalties.description_short
    elif description.lower().strip() in GENERIC_DESCRIPTION_BLACKLIST:
        score -= penalties.description_generic
    
    return max(0.0, min(1.0, score))


def refine_and_filter(
    proposals: List[Dict[str, Any]],
    raw_text: str = "",
    min_confidence_threshold: float = 0.5,
    penalties: Optional[PenaltyConfig] = None
) -> List[Dict[str, Any]]:
    """
    Apply deterministic scoring and filter out low-confidence proposals.
    
    Args:
        proposals: List of expense proposals from LLM
        raw_text: Original raw text (for date inference detection)
        min_confidence_threshold: Minimum confidence threshold for filtering
        penalties: Optional penalty configuration (uses defaults if not provided)
        
    Returns:
        Filtered list of proposals with confidence >= min_confidence_threshold
    """
    refined_proposals = []
    
    for proposal in proposals:
        was_date_inferred = False
        
        if "purchaseDate" not in proposal or not proposal.get("purchaseDate"):
            proposal["purchaseDate"] = datetime.now().strftime("%Y-%m-%d")
            was_date_inferred = True
        else:
            try:
                datetime.strptime(proposal["purchaseDate"], "%Y-%m-%d")
            except ValueError:
                proposal["purchaseDate"] = datetime.now().strftime("%Y-%m-%d")
                was_date_inferred = True
        
        if "amount" not in proposal:
            proposal["amount"] = 0
        
        if "currency" not in proposal:
            proposal["currency"] = "UNKNOWN"
        
        if "expenseType" not in proposal:
            proposal["expenseType"] = "UNKNOWN"
        
        confidence = calculate_confidence(proposal, was_date_inferred, penalties)
        proposal["confidence"] = confidence
        
        if confidence >= min_confidence_threshold:
            refined_proposals.append(proposal)
    
    return refined_proposals
