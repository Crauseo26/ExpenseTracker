import requests
import json
from datetime import datetime

BASE_URL = "http://localhost:5000"

def test_health():
    print("\n" + "="*60)
    print("TEST 1: Health Check")
    print("="*60)
    
    response = requests.get(f"{BASE_URL}/health")
    print(f"Status Code: {response.status_code}")
    print(f"Response: {json.dumps(response.json(), indent=2)}")
    
    assert response.status_code == 200
    assert response.json()["status"] == "healthy"
    print("[PASS] Health check passed!")


def test_simple_expense():
    print("\n" + "="*60)
    print("TEST 2: Simple Expense Extraction")
    print("="*60)
    
    payload = {
        "rawText": "Compré en McDonald's por $450 pesos uruguayos el 15 de enero",
        "inputType": "TEXT",
        "availableAccounts": ["Restaurants", "Supermarket", "Transportation", "Utilities"],
        "metadata": {
            "receivedAt": datetime.now().isoformat()
        }
    }
    
    print(f"Request Payload:")
    print(json.dumps(payload, indent=2))
    
    response = requests.post(f"{BASE_URL}/process-text", json=payload)
    print(f"\nStatus Code: {response.status_code}")
    print(f"Response: {json.dumps(response.json(), indent=2)}")
    
    assert response.status_code == 200
    result = response.json()
    assert "proposals" in result
    assert "overallConfidence" in result
    assert "processingTimeMs" in result
    
    if result["proposals"]:
        print(f"\n[PASS] Extracted {len(result['proposals'])} expense(s)!")
        for i, proposal in enumerate(result["proposals"], 1):
            print(f"\nProposal {i}:")
            print(f"  - Description: {proposal['description']}")
            print(f"  - Amount: {proposal['amount']} {proposal['currency']}")
            print(f"  - Date: {proposal['purchaseDate']}")
            print(f"  - Type: {proposal['expenseType']}")
            print(f"  - Confidence: {proposal['confidence']:.2%}")
    else:
        print("[WARN] No proposals extracted")


def test_multiple_expenses():
    print("\n" + "="*60)
    print("TEST 3: Multiple Expenses Extraction")
    print("="*60)
    
    payload = {
        "rawText": """
        Hoy gasté $200 en el supermercado Ta-Ta.
        Después pagué $150 en la farmacia.
        También tengo una suscripción mensual de Netflix por USD 15.99.
        """,
        "inputType": "TEXT",
        "availableAccounts": ["Supermarket", "Healthcare", "Entertainment", "Restaurants"]
    }
    
    print(f"Request Payload:")
    print(json.dumps(payload, indent=2))
    
    response = requests.post(f"{BASE_URL}/process-text", json=payload)
    print(f"\nStatus Code: {response.status_code}")
    print(f"Response: {json.dumps(response.json(), indent=2)}")
    
    result = response.json()
    if result["proposals"]:
        print(f"\n[PASS] Extracted {len(result['proposals'])} expense(s)!")
        for i, proposal in enumerate(result["proposals"], 1):
            print(f"\nProposal {i}:")
            print(f"  - Description: {proposal['description']}")
            print(f"  - Amount: {proposal['amount']} {proposal['currency']}")
            print(f"  - Type: {proposal['expenseType']}")
            print(f"  - Confidence: {proposal['confidence']:.2%}")
    else:
        print("[WARN] No proposals extracted")


def test_notification_format():
    print("\n" + "="*60)
    print("TEST 4: Notification Format (Bank SMS)")
    print("="*60)
    
    payload = {
        "rawText": "VISA SANTANDER: Compra aprobada por $1,250.00 en TIENDA INGLESA el 18/01/2026 a las 14:30",
        "inputType": "NOTIFICATION",
        "availableAccounts": ["Supermarket", "Restaurants", "Clothing", "Electronics"]
    }
    
    print(f"Request Payload:")
    print(json.dumps(payload, indent=2))
    
    response = requests.post(f"{BASE_URL}/process-text", json=payload)
    print(f"\nStatus Code: {response.status_code}")
    print(f"Response: {json.dumps(response.json(), indent=2)}")
    
    result = response.json()
    if result["proposals"]:
        print(f"\n[PASS] Extracted {len(result['proposals'])} expense(s)!")
        proposal = result["proposals"][0]
        print(f"  - Description: {proposal['description']}")
        print(f"  - Amount: {proposal['amount']} {proposal['currency']}")
        print(f"  - Date: {proposal['purchaseDate']}")
        print(f"  - Confidence: {proposal['confidence']:.2%}")
        if proposal.get('metadata', {}).get('merchant'):
            print(f"  - Merchant: {proposal['metadata']['merchant']}")


if __name__ == "__main__":
    try:
        print("\n>>> Starting AI Service Tests...")
        print(f"Target: {BASE_URL}")
        
        test_health()
        test_simple_expense()
        test_multiple_expenses()
        test_notification_format()
        
        print("\n" + "="*60)
        print("[SUCCESS] ALL TESTS COMPLETED!")
        print("="*60)
        
    except requests.exceptions.ConnectionError:
        print("\n[ERROR] Could not connect to the service.")
        print("Make sure the service is running: python main.py")
    except AssertionError as e:
        print(f"\n[FAILED] TEST FAILED: {e}")
    except Exception as e:
        print(f"\n[ERROR] UNEXPECTED ERROR: {e}")
        import traceback
        traceback.print_exc()
