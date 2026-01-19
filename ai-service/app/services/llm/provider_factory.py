import os
from typing import Optional

from .base_provider import BaseLLMProvider
from .gemini_provider import GeminiLLMProvider


def get_llm_provider(provider_name: Optional[str] = None) -> BaseLLMProvider:
    if provider_name is None:
        provider_name = os.getenv("LLM_PROVIDER", "gemini").lower()
    
    if provider_name == "gemini":
        return GeminiLLMProvider()
    else:
        raise ValueError(f"Unsupported LLM provider: {provider_name}")
