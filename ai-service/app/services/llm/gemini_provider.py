import os
import json
from typing import List, Dict, Any, Optional
from datetime import datetime
import google.generativeai as genai

from .base_provider import BaseLLMProvider
from .prompts import build_system_prompt, build_user_prompt


class GeminiLLMProvider(BaseLLMProvider):
    def __init__(self, api_key: Optional[str] = None):
        self.api_key = api_key or os.getenv("GEMINI_API_KEY")
        if not self.api_key:
            raise ValueError("GEMINI_API_KEY environment variable is required")
        
        genai.configure(api_key=self.api_key)
        self.model = genai.GenerativeModel(
            model_name="gemini-1.5-flash",
            generation_config={
                "response_mime_type": "application/json"
            }
        )
    
    async def extract_expenses(
        self,
        raw_text: str,
        input_type: str,
        available_accounts: Optional[List[str]] = None,
        metadata: Optional[Dict[str, Any]] = None
    ) -> Dict[str, Any]:
        try:
            system_prompt = build_system_prompt(available_accounts)
            user_prompt = build_user_prompt(raw_text, input_type)
            
            full_prompt = f"{system_prompt}\n\n{user_prompt}"
            
            response = self.model.generate_content(full_prompt)
            
            if not response.text:
                return {
                    "proposals": [],
                    "error": "No response from LLM"
                }
            
            try:
                result = json.loads(response.text)
                
                if "proposals" not in result:
                    result = {"proposals": []}
                
                for proposal in result.get("proposals", []):
                    if "purchaseDate" in proposal:
                        try:
                            datetime.strptime(proposal["purchaseDate"], "%Y-%m-%d")
                        except ValueError:
                            proposal["purchaseDate"] = datetime.now().strftime("%Y-%m-%d")
                    else:
                        proposal["purchaseDate"] = datetime.now().strftime("%Y-%m-%d")
                    
                    if "confidence" not in proposal:
                        proposal["confidence"] = 0.5
                    
                    proposal["confidence"] = max(0.0, min(1.0, proposal["confidence"]))
                    
                    if "currency" not in proposal:
                        proposal["currency"] = "UYU"
                    
                    if "expenseType" not in proposal:
                        proposal["expenseType"] = "SPORADIC"
                
                return result
                
            except json.JSONDecodeError as e:
                return {
                    "proposals": [],
                    "error": f"Failed to parse LLM response as JSON: {str(e)}",
                    "raw_response": response.text
                }
        
        except Exception as e:
            return {
                "proposals": [],
                "error": f"LLM processing error: {str(e)}"
            }
