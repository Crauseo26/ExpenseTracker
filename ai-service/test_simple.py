import requests
import json

url = "http://localhost:5000/process-text"
payload = {
    "rawText": "Compré en McDonald's por 450 pesos",
    "inputType": "TEXT",
    "availableAccounts": ["Restaurants", "Supermarket", "Transportation", "Healthcare"]
}

print("Testing with availableAccounts:")
print(json.dumps(payload, indent=2))
print("\n" + "="*60 + "\n")

response = requests.post(url, json=payload)
print(f"Status: {response.status_code}")
print(f"Response: {json.dumps(response.json(), indent=2)}")

if response.status_code == 200:
    result = response.json()
    if result.get("proposals"):
        for i, proposal in enumerate(result["proposals"], 1):
            print(f"\n--- Proposal {i} ---")
            print(f"Description: {proposal.get('description')}")
            print(f"Amount: {proposal.get('amount')} {proposal.get('currency')}")
            if proposal.get('metadata'):
                print(f"Metadata: {json.dumps(proposal['metadata'], indent=2)}")
                if 'suggestedAccount' in proposal['metadata']:
                    print(f"✓ Suggested Account: {proposal['metadata']['suggestedAccount']}")
                else:
                    print("✗ No suggestedAccount in metadata")
