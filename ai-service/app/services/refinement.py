from typing import List, Dict, Any
from datetime import datetime


GENERIC_DESCRIPTION_BLACKLIST = [
    "gasto", "compra", "pago", "ticket",
    "expense", "purchase", "payment", "shop"
]


def calculate_confidence(proposal: Dict[str, Any], was_date_inferred: bool) -> float:
    """
    Calculate deterministic confidence score based on data completeness and quality.
    
    Base score: 1.0
    Penalties:
    - -0.25 if amount == 0
    - -0.25 if currency == "UNKNOWN"
    - -0.25 if expenseType == "UNKNOWN"
    - -0.05 if purchaseDate was inferred (defaulted to today)
    - -0.20 if description is empty or whitespace only
    - -0.10 if description length < 3 characters (and not empty)
    - -0.15 if description matches generic blacklist
    
    Args:
        proposal: The expense proposal dictionary from LLM
        was_date_inferred: Whether the date was inferred/defaulted
        
    Returns:
        Confidence score between 0.0 and 1.0
    """
    score = 1.0
    
    if proposal.get("amount", 0) == 0:
        score -= 0.25
    
    if proposal.get("currency", "UNKNOWN") == "UNKNOWN":
        score -= 0.25
    
    if proposal.get("expenseType", "UNKNOWN") == "UNKNOWN":
        score -= 0.25
    
    if was_date_inferred:
        score -= 0.05
    
    description = proposal.get("description", "")
    if not description or not description.strip():
        score -= 0.20
    elif len(description) < 3:
        score -= 0.10
    elif description.lower().strip() in GENERIC_DESCRIPTION_BLACKLIST:
        score -= 0.15
    
    return max(0.0, min(1.0, score))


def refine_and_filter(proposals: List[Dict[str, Any]], raw_text: str = "") -> List[Dict[str, Any]]:
    """
    Apply deterministic scoring and filter out low-confidence proposals.
    
    Args:
        proposals: List of expense proposals from LLM
        raw_text: Original raw text (for date inference detection)
        
    Returns:
        Filtered list of proposals with confidence >= 0.5
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
        
        confidence = calculate_confidence(proposal, was_date_inferred)
        proposal["confidence"] = confidence
        
        if confidence >= 0.5:
            refined_proposals.append(proposal)
    
    return refined_proposals
