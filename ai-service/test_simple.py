import requests
import json

url = "http://localhost:5000/process-text"
payload = {
    "rawText": "Compré en McDonald's por 450 pesos",
    "inputType": "TEXT",
    "availableAccounts": ["Restaurants", "Supermarket"]
}

response = requests.post(url, json=payload)
print(f"Status: {response.status_code}")
print(f"Response: {json.dumps(response.json(), indent=2)}")
