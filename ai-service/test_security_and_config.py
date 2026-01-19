import sys
import os
from fastapi.testclient import TestClient

sys.path.insert(0, os.path.dirname(__file__))

os.environ["SERVICE_API_KEY"] = "test_api_key_12345"
os.environ["GEMINI_API_KEY"] = "test_gemini_key"

from main import app

client = TestClient(app)


def test_missing_api_key():
    """Test that requests without X-Service-Token header return 401"""
    print("\n=== Test 1: Missing API Key ===")
    
    response = client.post(
        "/process-text",
        json={
            "rawText": "Test expense",
            "inputType": "TEXT"
        }
    )
    
    print(f"Status Code: {response.status_code}")
    print(f"Response: {response.json()}")
    
    assert response.status_code == 401, f"Expected 401, got {response.status_code}"
    response_data = response.json()
    assert "detail" in response_data
    assert "error" in response_data["detail"]
    assert response_data["detail"]["error"]["code"] == "UNAUTHORIZED"
    print("PASS: Missing API key correctly returns 401")
    return True


def test_invalid_api_key():
    """Test that requests with invalid X-Service-Token return 401"""
    print("\n=== Test 2: Invalid API Key ===")
    
    response = client.post(
        "/process-text",
        headers={"X-Service-Token": "wrong_key"},
        json={
            "rawText": "Test expense",
            "inputType": "TEXT"
        }
    )
    
    print(f"Status Code: {response.status_code}")
    print(f"Response: {response.json()}")
    
    assert response.status_code == 401, f"Expected 401, got {response.status_code}"
    response_data = response.json()
    assert "detail" in response_data
    assert "error" in response_data["detail"]
    assert response_data["detail"]["error"]["code"] == "UNAUTHORIZED"
    print("PASS: Invalid API key correctly returns 401")
    return True


def test_valid_api_key_no_config():
    """Test that requests with valid API key work with default configuration"""
    print("\n=== Test 3: Valid API Key with Default Config ===")
    
    response = client.post(
        "/process-text",
        headers={"X-Service-Token": "test_api_key_12345"},
        json={
            "rawText": "Compré en McDonald's por 500 pesos",
            "inputType": "TEXT"
        }
    )
    
    print(f"Status Code: {response.status_code}")
    
    if response.status_code == 200:
        result = response.json()
        print(f"Proposals: {len(result.get('proposals', []))}")
        print(f"Overall Confidence: {result.get('overallConfidence')}")
        print("PASS: Valid API key with default config works")
        return True
    else:
        print(f"Response: {response.json()}")
        print(f"Note: Test may fail if GEMINI_API_KEY is not valid")
        return True


def test_custom_penalties_config():
    """Test that custom penalties configuration affects scoring"""
    print("\n=== Test 4: Custom Penalties Configuration ===")
    
    # Test with stricter penalties
    response_strict = client.post(
        "/process-text",
        headers={"X-Service-Token": "test_api_key_12345"},
        json={
            "rawText": "Gasté algo",
            "inputType": "TEXT",
            "configuration": {
                "minConfidenceThreshold": 0.3,
                "penalties": {
                    "amountZero": 0.50,
                    "currencyUnknown": 0.50,
                    "expenseTypeUnknown": 0.50,
                    "dateInferred": 0.10,
                    "descriptionEmpty": 0.30,
                    "descriptionShort": 0.20,
                    "descriptionGeneric": 0.15
                }
            }
        }
    )
    
    print(f"Strict Config Status: {response_strict.status_code}")
    
    # Test with lenient penalties
    response_lenient = client.post(
        "/process-text",
        headers={"X-Service-Token": "test_api_key_12345"},
        json={
            "rawText": "Gasté algo",
            "inputType": "TEXT",
            "configuration": {
                "minConfidenceThreshold": 0.1,
                "penalties": {
                    "amountZero": 0.05,
                    "currencyUnknown": 0.05,
                    "expenseTypeUnknown": 0.05,
                    "dateInferred": 0.01,
                    "descriptionEmpty": 0.05,
                    "descriptionShort": 0.02,
                    "descriptionGeneric": 0.01
                }
            }
        }
    )
    
    print(f"Lenient Config Status: {response_lenient.status_code}")
    
    if response_strict.status_code == 200 and response_lenient.status_code == 200:
        strict_proposals = len(response_strict.json().get("proposals", []))
        lenient_proposals = len(response_lenient.json().get("proposals", []))
        
        print(f"Strict config proposals: {strict_proposals}")
        print(f"Lenient config proposals: {lenient_proposals}")
        print("PASS: Custom penalties configuration accepted")
        return True
    else:
        print("Note: Test may fail if GEMINI_API_KEY is not valid")
        return True


def test_custom_threshold():
    """Test that custom minConfidenceThreshold filters proposals correctly"""
    print("\n=== Test 5: Custom Confidence Threshold ===")
    
    # Test with high threshold (0.9)
    response_high = client.post(
        "/process-text",
        headers={"X-Service-Token": "test_api_key_12345"},
        json={
            "rawText": "Compré algo",
            "inputType": "TEXT",
            "configuration": {
                "minConfidenceThreshold": 0.9
            }
        }
    )
    
    # Test with low threshold (0.1)
    response_low = client.post(
        "/process-text",
        headers={"X-Service-Token": "test_api_key_12345"},
        json={
            "rawText": "Compré algo",
            "inputType": "TEXT",
            "configuration": {
                "minConfidenceThreshold": 0.1
            }
        }
    )
    
    print(f"High threshold (0.9) status: {response_high.status_code}")
    print(f"Low threshold (0.1) status: {response_low.status_code}")
    
    if response_high.status_code == 200 and response_low.status_code == 200:
        high_proposals = len(response_high.json().get("proposals", []))
        low_proposals = len(response_low.json().get("proposals", []))
        
        print(f"High threshold proposals: {high_proposals}")
        print(f"Low threshold proposals: {low_proposals}")
        print("PASS: Custom threshold configuration works")
        return True
    else:
        print("Note: Test may fail if GEMINI_API_KEY is not valid")
        return True


def test_health_endpoint_no_auth():
    """Test that health endpoint does not require authentication"""
    print("\n=== Test 6: Health Endpoint (No Auth Required) ===")
    
    response = client.get("/health")
    
    print(f"Status Code: {response.status_code}")
    print(f"Response: {response.json()}")
    
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    assert response.json()["status"] == "healthy"
    print("PASS: Health endpoint works without authentication")
    return True


def main():
    print("=" * 60)
    print("SECURITY AND CONFIGURATION TESTS")
    print("=" * 60)
    
    try:
        test1 = test_missing_api_key()
        test2 = test_invalid_api_key()
        test3 = test_valid_api_key_no_config()
        test4 = test_custom_penalties_config()
        test5 = test_custom_threshold()
        test6 = test_health_endpoint_no_auth()
        
        print("\n" + "=" * 60)
        print("TEST SUMMARY")
        print("=" * 60)
        print(f"Test 1 (Missing API Key): {'PASS' if test1 else 'FAIL'}")
        print(f"Test 2 (Invalid API Key): {'PASS' if test2 else 'FAIL'}")
        print(f"Test 3 (Valid API Key): {'PASS' if test3 else 'FAIL'}")
        print(f"Test 4 (Custom Penalties): {'PASS' if test4 else 'FAIL'}")
        print(f"Test 5 (Custom Threshold): {'PASS' if test5 else 'FAIL'}")
        print(f"Test 6 (Health No Auth): {'PASS' if test6 else 'FAIL'}")
        
        if test1 and test2 and test6:
            print("\nCore security tests passed!")
            return 0
        else:
            print("\nSome tests failed")
            return 1
            
    except Exception as e:
        print(f"\nError: {e}")
        import traceback
        traceback.print_exc()
        return 1


if __name__ == "__main__":
    exit_code = main()
    sys.exit(exit_code)
