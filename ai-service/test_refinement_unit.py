import sys
from app.services.refinement import calculate_confidence, refine_and_filter


def test_calculate_confidence():
    """Test the deterministic confidence calculation"""
    print("\n=== Test: Confidence Calculation ===")
    
    # Test 1: Perfect data (all fields present and valid)
    proposal_perfect = {
        "amount": 500,
        "currency": "UYU",
        "expenseType": "SPORADIC",
        "purchaseDate": "2026-01-15"
    }
    score = calculate_confidence(proposal_perfect, was_date_inferred=False)
    print(f"Perfect data: {score} (expected: 1.0)")
    assert score == 1.0, f"Expected 1.0, got {score}"
    
    # Test 2: Missing amount (amount = 0)
    proposal_no_amount = {
        "amount": 0,
        "currency": "UYU",
        "expenseType": "SPORADIC",
        "purchaseDate": "2026-01-15"
    }
    score = calculate_confidence(proposal_no_amount, was_date_inferred=False)
    print(f"No amount: {score} (expected: 0.75)")
    assert score == 0.75, f"Expected 0.75, got {score}"
    
    # Test 3: Unknown currency
    proposal_unknown_currency = {
        "amount": 500,
        "currency": "UNKNOWN",
        "expenseType": "SPORADIC",
        "purchaseDate": "2026-01-15"
    }
    score = calculate_confidence(proposal_unknown_currency, was_date_inferred=False)
    print(f"Unknown currency: {score} (expected: 0.75)")
    assert score == 0.75, f"Expected 0.75, got {score}"
    
    # Test 4: Unknown expense type
    proposal_unknown_type = {
        "amount": 500,
        "currency": "UYU",
        "expenseType": "UNKNOWN",
        "purchaseDate": "2026-01-15"
    }
    score = calculate_confidence(proposal_unknown_type, was_date_inferred=False)
    print(f"Unknown type: {score} (expected: 0.75)")
    assert score == 0.75, f"Expected 0.75, got {score}"
    
    # Test 5: Inferred date
    proposal_inferred_date = {
        "amount": 500,
        "currency": "UYU",
        "expenseType": "SPORADIC",
        "purchaseDate": "2026-01-19"
    }
    score = calculate_confidence(proposal_inferred_date, was_date_inferred=True)
    print(f"Inferred date: {score} (expected: 0.75)")
    assert score == 0.75, f"Expected 0.75, got {score}"
    
    # Test 6: Two penalties (no amount + unknown currency)
    proposal_two_penalties = {
        "amount": 0,
        "currency": "UNKNOWN",
        "expenseType": "SPORADIC",
        "purchaseDate": "2026-01-15"
    }
    score = calculate_confidence(proposal_two_penalties, was_date_inferred=False)
    print(f"Two penalties: {score} (expected: 0.5)")
    assert score == 0.5, f"Expected 0.5, got {score}"
    
    # Test 7: All penalties (should be filtered)
    proposal_all_penalties = {
        "amount": 0,
        "currency": "UNKNOWN",
        "expenseType": "UNKNOWN",
        "purchaseDate": "2026-01-19"
    }
    score = calculate_confidence(proposal_all_penalties, was_date_inferred=True)
    print(f"All penalties: {score} (expected: 0.0)")
    assert score == 0.0, f"Expected 0.0, got {score}"
    
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
            "description": "Unknown",
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
    
    # Test 4: Just below threshold (score = 0.25, should be filtered)
    proposals = [
        {
            "description": "Below threshold",
            "amount": 0,
            "currency": "UNKNOWN",
            "expenseType": "UNKNOWN",
            "purchaseDate": "2026-01-15"
        }
    ]
    
    refined = refine_and_filter(proposals)
    print(f"Below threshold (0.25): {len(refined)} proposals (expected: 0)")
    assert len(refined) == 0, f"Expected 0 proposals, got {len(refined)}"
    
    # Test 5: Mixed proposals (some pass, some fail)
    proposals = [
        {
            "description": "Good",
            "amount": 500,
            "currency": "UYU",
            "expenseType": "SPORADIC",
            "purchaseDate": "2026-01-15"
        },
        {
            "description": "Bad",
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
    
    # Test 6: Missing fields get defaults
    proposals = [
        {
            "description": "Incomplete"
        }
    ]
    
    refined = refine_and_filter(proposals)
    print(f"Incomplete proposal: {len(refined)} proposals (expected: 0)")
    assert len(refined) == 0, f"Expected 0 proposals (all defaults = score 0.0), got {len(refined)}"
    
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
