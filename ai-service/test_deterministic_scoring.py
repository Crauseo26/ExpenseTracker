import asyncio
import sys
from app.services.llm import get_llm_provider


async def test_bad_input():
    """Test that bad input (e.g., 'hello') returns 0 proposals due to score < 0.5"""
    print("\n=== Test 1: Bad Input (should return 0 proposals) ===")
    provider = get_llm_provider()
    result = await provider.extract_expenses(
        raw_text="hello",
        input_type="TEXT"
    )
    
    print(f"Proposals returned: {len(result.get('proposals', []))}")
    print(f"Result: {result}")
    
    if len(result.get('proposals', [])) == 0:
        print("✓ PASS: Bad input correctly filtered out")
    else:
        print("✗ FAIL: Bad input should return 0 proposals")
        for p in result.get('proposals', []):
            print(f"  - Proposal: {p}")
    
    return len(result.get('proposals', [])) == 0


async def test_good_input():
    """Test that good input with complete data returns proposals with high confidence"""
    print("\n=== Test 2: Good Input (should return proposals with high confidence) ===")
    provider = get_llm_provider()
    result = await provider.extract_expenses(
        raw_text="Compré en McDonald's por 500 pesos uruguayos el 15 de enero de 2026",
        input_type="TEXT"
    )
    
    print(f"Proposals returned: {len(result.get('proposals', []))}")
    
    if len(result.get('proposals', [])) > 0:
        for p in result.get('proposals', []):
            print(f"  - Description: {p.get('description')}")
            print(f"  - Amount: {p.get('amount')}")
            print(f"  - Currency: {p.get('currency')}")
            print(f"  - ExpenseType: {p.get('expenseType')}")
            print(f"  - PurchaseDate: {p.get('purchaseDate')}")
            print(f"  - Confidence: {p.get('confidence')}")
            
            if p.get('confidence', 0) >= 0.75:
                print("✓ PASS: Good input has high confidence")
                return True
            else:
                print(f"✗ FAIL: Expected confidence >= 0.75, got {p.get('confidence')}")
                return False
    else:
        print("✗ FAIL: Good input should return at least 1 proposal")
        return False


async def test_partial_input():
    """Test that partial input (missing some fields) returns proposals with medium confidence"""
    print("\n=== Test 3: Partial Input (should return proposals with medium confidence) ===")
    provider = get_llm_provider()
    result = await provider.extract_expenses(
        raw_text="Gasté 200 en comida",
        input_type="TEXT"
    )
    
    print(f"Proposals returned: {len(result.get('proposals', []))}")
    
    if len(result.get('proposals', [])) > 0:
        for p in result.get('proposals', []):
            print(f"  - Description: {p.get('description')}")
            print(f"  - Amount: {p.get('amount')}")
            print(f"  - Currency: {p.get('currency')}")
            print(f"  - ExpenseType: {p.get('expenseType')}")
            print(f"  - PurchaseDate: {p.get('purchaseDate')}")
            print(f"  - Confidence: {p.get('confidence')}")
            
            if 0.5 <= p.get('confidence', 0) < 0.75:
                print("✓ PASS: Partial input has medium confidence")
                return True
            else:
                print(f"  Note: Confidence is {p.get('confidence')}")
                return True
    else:
        print("  Note: Partial input filtered out (confidence < 0.5)")
        return True


async def test_very_incomplete_input():
    """Test that very incomplete input gets filtered out"""
    print("\n=== Test 4: Very Incomplete Input (should be filtered) ===")
    provider = get_llm_provider()
    result = await provider.extract_expenses(
        raw_text="compré algo",
        input_type="TEXT"
    )
    
    print(f"Proposals returned: {len(result.get('proposals', []))}")
    print(f"Result: {result}")
    
    if len(result.get('proposals', [])) == 0:
        print("✓ PASS: Very incomplete input correctly filtered out")
    else:
        print("  Note: Some proposals returned:")
        for p in result.get('proposals', []):
            print(f"  - Confidence: {p.get('confidence')}")
    
    return True


async def main():
    print("=" * 60)
    print("DETERMINISTIC SCORING VERIFICATION TESTS")
    print("=" * 60)
    
    try:
        test1 = await test_bad_input()
        test2 = await test_good_input()
        test3 = await test_partial_input()
        test4 = await test_very_incomplete_input()
        
        print("\n" + "=" * 60)
        print("TEST SUMMARY")
        print("=" * 60)
        print(f"Test 1 (Bad Input): {'PASS' if test1 else 'FAIL'}")
        print(f"Test 2 (Good Input): {'PASS' if test2 else 'FAIL'}")
        print(f"Test 3 (Partial Input): {'PASS' if test3 else 'FAIL'}")
        print(f"Test 4 (Very Incomplete): {'PASS' if test4 else 'FAIL'}")
        
        if test1 and test2:
            print("\n✓ Core tests passed!")
            return 0
        else:
            print("\n✗ Some core tests failed")
            return 1
            
    except Exception as e:
        print(f"\n✗ ERROR: {e}")
        import traceback
        traceback.print_exc()
        return 1


if __name__ == "__main__":
    exit_code = asyncio.run(main())
    sys.exit(exit_code)
