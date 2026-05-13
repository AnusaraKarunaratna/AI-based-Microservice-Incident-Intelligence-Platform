from fastapi import FastAPI
from pydantic import BaseModel
import requests

app = FastAPI()

OLLAMA_URL = "http://ollama:11434/api/generate"

class IncidentRequest(BaseModel):
    serviceName: str
    severity: str
    message: str
    errorCount: int

@app.post("/analyze")
async def analyze(incident: IncidentRequest):

    prompt = f"""
You are an expert SRE AI assistant.

Analyze this distributed system incident.

Service:
{incident.serviceName}

Severity:
{incident.severity}

Error Count:
{incident.errorCount}

Message:
{incident.message}

Provide:
1. Root cause
2. Recommendation
3. Priority

Respond in concise JSON format.
"""

    response = requests.post(
        OLLAMA_URL,
        json={
            "model": "llama3",
            "prompt": prompt,
            "stream": False
        }
    )

    result = response.json()["response"]

    return {
        "analysis": result
    }