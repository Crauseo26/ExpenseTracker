from abc import ABC, abstractmethod
from typing import List, Dict, Any, Optional


class BaseLLMProvider(ABC):
    @abstractmethod
    async def extract_expenses(
        self,
        raw_text: str,
        input_type: str,
        available_accounts: Optional[List[str]] = None,
        metadata: Optional[Dict[str, Any]] = None
    ) -> Dict[str, Any]:
        pass
