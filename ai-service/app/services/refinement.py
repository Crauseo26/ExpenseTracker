from typing import List, Dict, Any
from datetime import datetime


def calculate_confidence(proposal: Dict[str, Any], was_date_inferred: bool) -> float:
    """
    Calculate deterministic confidence score based on data completeness.
    
    Base score: 1.0
    Penalties:
    - -0.25 if amount == 0
    - -0.25 if currency == "UNKNOWN"
    - -0.25 if expenseType == "UNKNOWN"
    - -0.25 if purchaseDate was inferred (defaulted to today)
    
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
        score -= 0.25
    
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
