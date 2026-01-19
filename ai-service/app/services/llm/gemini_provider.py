import os
import json
from typing import List, Dict, Any, Optional
from datetime import datetime
from google import genai
from google.genai import types

from .base_provider import BaseLLMProvider
from .prompts import build_system_prompt, build_user_prompt
from ..refinement import refine_and_filter, PenaltyConfig


class GeminiLLMProvider(BaseLLMProvider):
    def __init__(self, api_key: Optional[str] = None, model_name: Optional[str] = None):
        self.api_key = api_key or os.getenv("GEMINI_API_KEY")
        if not self.api_key:
            raise ValueError("GEMINI_API_KEY environment variable is required")
        
        self.model_name = model_name or os.getenv("GEMINI_MODEL", "gemini-1.5-flash")
        
        self.client = genai.Client(api_key=self.api_key)
    
    async def extract_expenses(
        self,
        raw_text: str,
        input_type: str,
        available_accounts: Optional[List[str]] = None,
        metadata: Optional[Dict[str, Any]] = None,
        min_confidence_threshold: float = 0.5,
        penalties: Optional[PenaltyConfig] = None
    ) -> Dict[str, Any]:
        try:
            system_prompt = build_system_prompt(available_accounts)
            user_prompt = build_user_prompt(raw_text, input_type)
            
            full_prompt = f"{system_prompt}\n\n{user_prompt}"
            
            contents = [
                types.Content(
                    role="user",
                    parts=[
                        types.Part.from_text(text=full_prompt),
                    ],
                ),
            ]
            
            generate_content_config = types.GenerateContentConfig(
                response_mime_type="application/json"
            )
            
            response = self.client.models.generate_content(
                model=self.model_name,
                contents=contents,
                config=generate_content_config,
            )
            
            if not response.text:
                return {
                    "proposals": [],
                    "error": "No response from LLM"
                }
            
            print(f"[DEBUG] LLM Raw Response: {response.text}")
            
            try:
                result = json.loads(response.text)
                
                if "proposals" not in result:
                    result = {"proposals": []}
                
                refined_proposals = refine_and_filter(
                    result.get("proposals", []),
                    raw_text,
                    min_confidence_threshold,
                    penalties
                )
                result["proposals"] = refined_proposals
                
                return result
                
            except json.JSONDecodeError as e:
                print(f"[ERROR] JSON Parse Error: {str(e)}")
                print(f"[ERROR] Raw Response: {response.text}")
                return {
                    "proposals": [],
                    "error": f"Failed to parse LLM response as JSON: {str(e)}",
                    "raw_response": response.text
                }
        
        except Exception as e:
            print(f"[ERROR] LLM Exception: {str(e)}")
            import traceback
            traceback.print_exc()
            return {
                "proposals": [],
                "error": f"LLM processing error: {str(e)}"
            }
