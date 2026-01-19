import sys
from app.services.refinement import calculate_confidence, refine_and_filter


def test_calculate_confidence():
    """Test the deterministic confidence calculation"""
    print("\n=== Test: Confidence Calculation ===")
    
    # Test 1: Perfect data (all fields present and valid)
    proposal_perfect = {
        "description": "McDonald's",
        "amount": 500,
        "currency": "UYU",
        "expenseType": "SPORADIC",
        "purchaseDate": "2026-01-15"
    }
    score = calculate_confidence(proposal_perfect, was_date_inferred=False)
    print(f"Perfect data: {score} (expected: 1.0)")
    assert score == 1.0, f"Expected 1.0, got {score}"
    
    # Test 2: Valid expense + inferred date (NEW RULE: -0.05 for date)
    proposal_inferred_date = {
        "description": "McDonald's",
        "amount": 500,
        "currency": "UYU",
        "expenseType": "SPORADIC",
        "purchaseDate": "2026-01-19"
    }
    score = calculate_confidence(proposal_inferred_date, was_date_inferred=True)
    print(f"Valid expense + inferred date: {score} (expected: 0.95)")
    assert score == 0.95, f"Expected 0.95, got {score}"
    
    # Test 3: Generic description + inferred date
    proposal_generic = {
        "description": "Gasto",
        "amount": 500,
        "currency": "UYU",
        "expenseType": "SPORADIC",
        "purchaseDate": "2026-01-19"
    }
    score = calculate_confidence(proposal_generic, was_date_inferred=True)
    print(f"Generic 'Gasto' + inferred date: {score} (expected: 0.80)")
    assert abs(score - 0.80) < 0.001, f"Expected 0.80 (1.0 - 0.15 - 0.05), got {score}"
    
    # Test 4: Short description + inferred date
    proposal_short = {
        "description": "A",
        "amount": 500,
        "currency": "UYU",
        "expenseType": "SPORADIC",
        "purchaseDate": "2026-01-19"
    }
    score = calculate_confidence(proposal_short, was_date_inferred=True)
    print(f"Short 'A' + inferred date: {score} (expected: 0.85)")
    assert score == 0.85, f"Expected 0.85 (1.0 - 0.10 - 0.05), got {score}"
    
    # Test 5: Empty description
    proposal_empty_desc = {
        "description": "",
        "amount": 500,
        "currency": "UYU",
        "expenseType": "SPORADIC",
        "purchaseDate": "2026-01-15"
    }
    score = calculate_confidence(proposal_empty_desc, was_date_inferred=False)
    print(f"Empty description: {score} (expected: 0.80)")
    assert score == 0.80, f"Expected 0.80 (1.0 - 0.20), got {score}"
    
    # Test 6: Missing amount (amount = 0)
    proposal_no_amount = {
        "description": "McDonald's",
        "amount": 0,
        "currency": "UYU",
        "expenseType": "SPORADIC",
        "purchaseDate": "2026-01-15"
    }
    score = calculate_confidence(proposal_no_amount, was_date_inferred=False)
    print(f"No amount: {score} (expected: 0.75)")
    assert score == 0.75, f"Expected 0.75, got {score}"
    
    # Test 7: Unknown currency
    proposal_unknown_currency = {
        "description": "McDonald's",
        "amount": 500,
        "currency": "UNKNOWN",
        "expenseType": "SPORADIC",
        "purchaseDate": "2026-01-15"
    }
    score = calculate_confidence(proposal_unknown_currency, was_date_inferred=False)
    print(f"Unknown currency: {score} (expected: 0.75)")
    assert score == 0.75, f"Expected 0.75, got {score}"
    
    # Test 8: Unknown expense type
    proposal_unknown_type = {
        "description": "McDonald's",
        "amount": 500,
        "currency": "UYU",
        "expenseType": "UNKNOWN",
        "purchaseDate": "2026-01-15"
    }
    score = calculate_confidence(proposal_unknown_type, was_date_inferred=False)
    print(f"Unknown type: {score} (expected: 0.75)")
    assert score == 0.75, f"Expected 0.75, got {score}"
    
    # Test 9: All generic blacklist words
    for word in ["gasto", "compra", "pago", "ticket", "expense", "purchase", "payment", "shop"]:
        proposal_blacklist = {
            "description": word,
            "amount": 500,
            "currency": "UYU",
            "expenseType": "SPORADIC",
            "purchaseDate": "2026-01-15"
        }
        score = calculate_confidence(proposal_blacklist, was_date_inferred=False)
        print(f"  Blacklist word '{word}': {score} (expected: 0.85)")
        assert score == 0.85, f"Expected 0.85 for '{word}', got {score}"
    
    # Test 10: Case insensitive blacklist check
    proposal_upper = {
        "description": "GASTO",
        "amount": 500,
        "currency": "UYU",
        "expenseType": "SPORADIC",
        "purchaseDate": "2026-01-15"
    }
    score = calculate_confidence(proposal_upper, was_date_inferred=False)
    print(f"Uppercase 'GASTO': {score} (expected: 0.85)")
    assert score == 0.85, f"Expected 0.85, got {score}"
    
    # Test 11: Multiple penalties
    proposal_multiple = {
        "description": "gasto",
        "amount": 0,
        "currency": "UNKNOWN",
        "expenseType": "UNKNOWN",
        "purchaseDate": "2026-01-19"
    }
    score = calculate_confidence(proposal_multiple, was_date_inferred=True)
    expected = 1.0 - 0.25 - 0.25 - 0.25 - 0.05 - 0.15
    print(f"Multiple penalties: {score} (expected: {expected})")
    assert score == expected, f"Expected {expected}, got {score}"
    
    print("PASS: All confidence calculations correct")
    return True


def test_refine_and_filter():
    """Test the refinement and filtering logic"""
    print("\n=== Test: Refine and Filter ===")
    
    # Test 1: Good proposal (should pass)
    proposals = [
        {
            "description": "McDonald's",
            "amount": 500,
            "currency": "UYU",
            "expenseType": "SPORADIC",
            "purchaseDate": "2026-01-15"
        }
    ]
    
    refined = refine_and_filter(proposals)
    print(f"Good proposal: {len(refined)} proposals (expected: 1)")
    assert len(refined) == 1, f"Expected 1 proposal, got {len(refined)}"
    assert refined[0]["confidence"] == 1.0, f"Expected confidence 1.0, got {refined[0]['confidence']}"
    
    # Test 2: Bad proposal (should be filtered out)
    proposals = [
        {
            "description": "",
            "amount": 0,
            "currency": "UNKNOWN",
            "expenseType": "UNKNOWN"
        }
    ]
    
    refined = refine_and_filter(proposals)
    print(f"Bad proposal: {len(refined)} proposals (expected: 0)")
    assert len(refined) == 0, f"Expected 0 proposals, got {len(refined)}"
    
    # Test 3: Borderline proposal (score = 0.5, should pass)
    proposals = [
        {
            "description": "Partial",
            "amount": 0,
            "currency": "UNKNOWN",
            "expenseType": "SPORADIC",
            "purchaseDate": "2026-01-15"
        }
    ]
    
    refined = refine_and_filter(proposals)
    print(f"Borderline proposal (0.5): {len(refined)} proposals (expected: 1)")
    assert len(refined) == 1, f"Expected 1 proposal, got {len(refined)}"
    assert refined[0]["confidence"] == 0.5, f"Expected confidence 0.5, got {refined[0]['confidence']}"
    
    # Test 4: Just below threshold (should be filtered)
    proposals = [
        {
            "description": "Below",
            "amount": 0,
            "currency": "UNKNOWN",
            "expenseType": "UNKNOWN",
            "purchaseDate": "2026-01-15"
        }
    ]
    
    refined = refine_and_filter(proposals)
    expected_score = 1.0 - 0.25 - 0.25 - 0.25
    print(f"Below threshold ({expected_score}): {len(refined)} proposals (expected: 0)")
    assert len(refined) == 0, f"Expected 0 proposals (score={expected_score} < 0.5), got {len(refined)}"
    
    # Test 5: Mixed proposals (some pass, some fail)
    proposals = [
        {
            "description": "Good expense",
            "amount": 500,
            "currency": "UYU",
            "expenseType": "SPORADIC",
            "purchaseDate": "2026-01-15"
        },
        {
            "description": "",
            "amount": 0,
            "currency": "UNKNOWN",
            "expenseType": "UNKNOWN"
        },
        {
            "description": "Medium",
            "amount": 200,
            "currency": "UNKNOWN",
            "purchaseDate": "2026-01-15"
        }
    ]
    
    refined = refine_and_filter(proposals)
    print(f"Mixed proposals: {len(refined)} proposals (expected: 2)")
    assert len(refined) == 2, f"Expected 2 proposals, got {len(refined)}"
    
    # Test 6: Missing fields get defaults (empty description + all defaults)
    proposals = [
        {
            "description": ""
        }
    ]
    
    refined = refine_and_filter(proposals)
    expected_score = 1.0 - 0.20 - 0.25 - 0.25 - 0.25 - 0.05
    print(f"Incomplete proposal (score={expected_score}): {len(refined)} proposals (expected: 0)")
    assert len(refined) == 0, f"Expected 0 proposals (score < 0.5), got {len(refined)}"
    
    # Test 7: Generic description with good data (should pass with lower confidence)
    proposals = [
        {
            "description": "gasto",
            "amount": 500,
            "currency": "UYU",
            "expenseType": "SPORADIC",
            "purchaseDate": "2026-01-15"
        }
    ]
    
    refined = refine_and_filter(proposals)
    print(f"Generic description: {len(refined)} proposals (expected: 1)")
    assert len(refined) == 1, f"Expected 1 proposal, got {len(refined)}"
    assert refined[0]["confidence"] == 0.85, f"Expected confidence 0.85, got {refined[0]['confidence']}"
    
    print("PASS: All refinement and filtering tests passed")
    return True


def main():
    print("=" * 60)
    print("REFINEMENT SERVICE UNIT TESTS")
    print("=" * 60)
    
    try:
        test1 = test_calculate_confidence()
        test2 = test_refine_and_filter()
        
        print("\n" + "=" * 60)
        print("TEST SUMMARY")
        print("=" * 60)
        print(f"Confidence Calculation: {'PASS' if test1 else 'FAIL'}")
        print(f"Refine and Filter: {'PASS' if test2 else 'FAIL'}")
        
        if test1 and test2:
            print("\nAll tests passed!")
            return 0
        else:
            print("\nSome tests failed")
            return 1
            
    except AssertionError as e:
        print(f"\nAssertion Error: {e}")
        return 1
    except Exception as e:
        print(f"\nError: {e}")
        import traceback
        traceback.print_exc()
        return 1


if __name__ == "__main__":
    exit_code = main()
    sys.exit(exit_code)
