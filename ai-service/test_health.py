import sys
import time
import subprocess
import requests

def test_service():
    print("Starting FastAPI service...")
    process = subprocess.Popen(
        [sys.executable, "main.py"],
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE
    )
    
    time.sleep(5)
    
    try:
        print("Testing /health endpoint...")
        response = requests.get("http://localhost:5000/health", timeout=5)
        
        print(f"Status Code: {response.status_code}")
        print(f"Response: {response.json()}")
        
        if response.status_code == 200:
            data = response.json()
            if data.get("status") == "healthy" and data.get("version") == "1.0.0":
                print("\n✓ Health check passed!")
                return True
        
        print("\n✗ Health check failed!")
        return False
        
    except Exception as e:
        print(f"\n✗ Error testing service: {e}")
        return False
        
    finally:
        print("\nTerminating service...")
        process.terminate()
        process.wait(timeout=5)

if __name__ == "__main__":
    success = test_service()
    sys.exit(0 if success else 1)
