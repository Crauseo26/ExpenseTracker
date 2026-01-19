from fastapi import FastAPI, HTTPException, Security, Depends
from fastapi.security import APIKeyHeader
from pydantic import BaseModel, Field, field_validator
from typing import List, Optional, Dict, Any
from datetime import datetime
from enum import Enum
from dotenv import load_dotenv
import os
from app.services.llm import get_llm_provider

load_dotenv()

SERVICE_API_KEY = os.getenv("SERVICE_API_KEY")
if not SERVICE_API_KEY:
    raise ValueError("SERVICE_API_KEY environment variable is required. Service cannot start.")

api_key_header = APIKeyHeader(name="X-Service-Token", auto_error=False)


async def verify_api_key(api_key: str = Security(api_key_header)):
    if not api_key or api_key != SERVICE_API_KEY:
        raise HTTPException(
            status_code=401,
            detail={
                "error": {
                    "code": "UNAUTHORIZED",
                    "message": "Invalid or missing API key",
                    "details": {}
                }
            }
        )
    return api_key

app = FastAPI(
    title="FinancIA AI Service",
    description="AI processing service for expense extraction from unstructured text",
    version="1.0.0"
)


class InputType(str, Enum):
    TEXT = "TEXT"
    NOTIFICATION = "NOTIFICATION"
    EMAIL = "EMAIL"


class Currency(str, Enum):
    UYU = "UYU"
    USD = "USD"


class ExpenseType(str, Enum):
    SPORADIC = "SPORADIC"
    REPETITIVE = "REPETITIVE"


class PenaltiesConfig(BaseModel):
    amountZero: float = Field(default=0.25, ge=0.0, le=1.0, description="Penalty when amount is 0")
    currencyUnknown: float = Field(default=0.25, ge=0.0, le=1.0, description="Penalty when currency is UNKNOWN")
    expenseTypeUnknown: float = Field(default=0.25, ge=0.0, le=1.0, description="Penalty when expenseType is UNKNOWN")
    dateInferred: float = Field(default=0.05, ge=0.0, le=1.0, description="Penalty when date is inferred")
    descriptionEmpty: float = Field(default=0.20, ge=0.0, le=1.0, description="Penalty when description is empty")
    descriptionShort: float = Field(default=0.10, ge=0.0, le=1.0, description="Penalty when description is too short")
    descriptionGeneric: float = Field(default=0.05, ge=0.0, le=1.0, description="Penalty when description is generic")


class ScoringConfig(BaseModel):
    minConfidenceThreshold: float = Field(default=0.5, ge=0.0, le=1.0, description="Minimum confidence threshold for filtering")
    penalties: PenaltiesConfig = Field(default_factory=PenaltiesConfig, description="Penalty configuration")


class ProcessTextRequest(BaseModel):
    rawText: str = Field(..., description="Raw text input to process")
    inputType: InputType = Field(..., description="Type of input")
    availableAccounts: Optional[List[str]] = Field(default=None, description="List of available account names for matching")
    metadata: Optional[Dict[str, Any]] = Field(default=None, description="Optional metadata")
    configuration: Optional[ScoringConfig] = Field(default=None, description="Optional scoring configuration")

    @field_validator('rawText')
    @classmethod
    def validate_raw_text(cls, v: str) -> str:
        if not v or not v.strip():
            raise ValueError("rawText cannot be empty")
        return v


class ExpenseProposalMetadata(BaseModel):
    merchant: Optional[str] = Field(default=None, description="Detected merchant name")
    rawExtraction: Optional[str] = Field(default=None, description="Raw extracted text")
    suggestedAccount: Optional[str] = Field(default=None, description="Suggested account category from available accounts")


class ExpenseProposal(BaseModel):
    description: str = Field(..., description="Clean expense description")
    amount: float = Field(..., ge=0, description="Expense amount")
    currency: Currency = Field(..., description="Currency code")
    purchaseDate: str = Field(..., description="Purchase date in YYYY-MM-DD format")
    expenseType: ExpenseType = Field(..., description="Type of expense")
    confidence: float = Field(..., ge=0.0, le=1.0, description="Confidence score between 0.0 and 1.0")
    metadata: Optional[ExpenseProposalMetadata] = Field(default=None, description="Additional metadata")


class ProcessTextResponse(BaseModel):
    proposals: List[ExpenseProposal] = Field(default_factory=list, description="List of expense proposals")
    overallConfidence: float = Field(..., ge=0.0, le=1.0, description="Overall confidence score")
    processingTimeMs: int = Field(..., ge=0, description="Processing time in milliseconds")


class ErrorDetail(BaseModel):
    code: str = Field(..., description="Error code")
    message: str = Field(..., description="Error message")
    details: Optional[Dict[str, Any]] = Field(default=None, description="Additional error details")


class ErrorResponse(BaseModel):
    error: ErrorDetail


class HealthResponse(BaseModel):
    status: str = Field(default="healthy", description="Service health status")
    version: str = Field(default="1.0.0", description="Service version")


@app.get("/health", response_model=HealthResponse, tags=["Health"])
async def health_check():
    """
    Health check endpoint to verify the service is running.
    """
    return HealthResponse(status="healthy", version="1.0.0")


@app.post("/process-text", response_model=ProcessTextResponse, tags=["Processing"])
async def process_text(request: ProcessTextRequest, api_key: str = Depends(verify_api_key)):
    """
    Process raw text input and extract structured expense proposals.
    """
    try:
        start_time = datetime.now()
        
        config = request.configuration or ScoringConfig()
        
        from app.services.refinement import PenaltyConfig
        penalty_config = PenaltyConfig(
            amount_zero=config.penalties.amountZero,
            currency_unknown=config.penalties.currencyUnknown,
            expense_type_unknown=config.penalties.expenseTypeUnknown,
            date_inferred=config.penalties.dateInferred,
            description_empty=config.penalties.descriptionEmpty,
            description_short=config.penalties.descriptionShort,
            description_generic=config.penalties.descriptionGeneric
        )
        
        llm_provider = get_llm_provider()
        
        result = await llm_provider.extract_expenses(
            raw_text=request.rawText,
            input_type=request.inputType.value,
            available_accounts=request.availableAccounts,
            metadata=request.metadata,
            min_confidence_threshold=config.minConfidenceThreshold,
            penalties=penalty_config
        )
        
        proposals = []
        for proposal_data in result.get("proposals", []):
            try:
                proposal = ExpenseProposal(
                    description=proposal_data.get("description", "Unknown expense"),
                    amount=float(proposal_data.get("amount", 0.0)),
                    currency=Currency(proposal_data.get("currency", "UYU")),
                    purchaseDate=proposal_data.get("purchaseDate", datetime.now().strftime("%Y-%m-%d")),
                    expenseType=ExpenseType(proposal_data.get("expenseType", "SPORADIC")),
                    confidence=float(proposal_data.get("confidence", 0.0)),
                    metadata=ExpenseProposalMetadata(
                        merchant=proposal_data.get("metadata", {}).get("merchant"),
                        rawExtraction=proposal_data.get("metadata", {}).get("rawExtraction"),
                        suggestedAccount=proposal_data.get("metadata", {}).get("suggestedAccount")
                    ) if proposal_data.get("metadata") else None
                )
                proposals.append(proposal)
            except Exception as e:
                continue
        
        overall_confidence = 0.0
        if proposals:
            overall_confidence = sum(p.confidence for p in proposals) / len(proposals)
        
        end_time = datetime.now()
        processing_time_ms = int((end_time - start_time).total_seconds() * 1000)
        
        return ProcessTextResponse(
            proposals=proposals,
            overallConfidence=overall_confidence,
            processingTimeMs=processing_time_ms
        )
    
    except ValueError as e:
        raise HTTPException(
            status_code=400,
            detail={
                "error": {
                    "code": "VALIDATION_ERROR",
                    "message": str(e),
                    "details": {}
                }
            }
        )
    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail={
                "error": {
                    "code": "INTERNAL_ERROR",
                    "message": "An unexpected error occurred during processing",
                    "details": {"exception": str(e)}
                }
            }
        )


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=5000)
