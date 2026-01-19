from fastapi import FastAPI, HTTPException
from pydantic import BaseModel, Field, field_validator
from typing import List, Optional, Dict, Any
from datetime import datetime
from enum import Enum

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


class ProcessTextRequest(BaseModel):
    rawText: str = Field(..., description="Raw text input to process")
    inputType: InputType = Field(..., description="Type of input")
    metadata: Optional[Dict[str, Any]] = Field(default=None, description="Optional metadata")

    @field_validator('rawText')
    @classmethod
    def validate_raw_text(cls, v: str) -> str:
        if not v or not v.strip():
            raise ValueError("rawText cannot be empty")
        return v


class ExpenseProposalMetadata(BaseModel):
    merchant: Optional[str] = Field(default=None, description="Detected merchant name")
    rawExtraction: Optional[str] = Field(default=None, description="Raw extracted text")


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
async def process_text(request: ProcessTextRequest):
    """
    Process raw text input and extract structured expense proposals.
    
    This is a placeholder implementation that will be replaced with actual AI processing logic.
    """
    try:
        start_time = datetime.now()
        
        # Placeholder implementation
        # TODO: Implement actual AI processing logic with LLM orchestration
        proposals = []
        overall_confidence = 0.0
        
        # For now, return empty proposals to indicate the endpoint is functional
        # but not yet implemented
        
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
