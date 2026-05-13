from fastapi import FastAPI
from pydantic import BaseModel
import requests
import os
import json
from dotenv import load_dotenv

load_dotenv()

app = FastAPI()

OPENROUTER_API_KEY = os.getenv("OPENROUTER_API_KEY")
OPENROUTER_URL = "https://openrouter.ai/api/v1/chat/completions"


class IncidentRequest(BaseModel):
    serviceName: str
    severity: str
    message: str
    errorCount: int


def build_prompt(incident: IncidentRequest):
    return f"""
You are a Senior Site Reliability Engineer (SRE) working in a production-grade distributed microservices system.

Your job is to perform deep root cause analysis using engineering reasoning.

You MUST:
- Avoid generic answers
- Use system-level thinking (DB, network, queue, CPU, memory, retries, timeouts)
- Infer likely failure domain
- Provide actionable engineering fixes

---

INCIDENT DATA:
Service Name: {incident.serviceName}
Severity: {incident.severity}
Error Count: {incident.errorCount}
Log Message: {incident.message}

---

ANALYSIS REQUIREMENTS:

1. ROOT CAUSE
- Be specific (NOT "unknown anomaly")
- Identify likely subsystem:
  (Database / Network / Message Queue / External API / Application Bug / Resource Exhaustion)

2. TECHNICAL EXPLANATION
- Explain WHY this likely happened
- Mention failure chain (e.g. retry storm → connection pool exhaustion)

3. IMPACT ANALYSIS
- What breaks in system if unresolved
- Downstream effects

4. RECOMMENDED FIX
- Step-by-step engineering actions
- Configuration/code-level suggestions

5. PRIORITY RULES:
- HIGH → service outage or data loss risk
- MEDIUM → degraded performance or intermittent failure
- LOW → non-critical or recoverable issue

---

OUTPUT FORMAT (STRICT JSON ONLY):

{{
  "rootCause": "string (specific technical cause)",
  "technicalExplanation": "string (detailed reasoning)",
  "impactAnalysis": "string (system impact)",
  "recommendation": "string (actionable steps)",
  "priority": "HIGH|MEDIUM|LOW",
  "confidence": 0.0
}}
"""


@app.post("/analyze")
def analyze(incident: IncidentRequest):

    prompt = build_prompt(incident)

    payload = {
        "model": "meta-llama/llama-3.1-8b-instruct",
        "messages": [
            {
                "role": "system",
                "content": "You are a production-grade SRE incident analysis engine. Always return valid JSON only."
            },
            {
                "role": "user",
                "content": prompt
            }
        ],
        "temperature": 0.2
    }

    headers = {
        "Authorization": f"Bearer {OPENROUTER_API_KEY}",
        "Content-Type": "application/json",
        "HTTP-Referer": "http://localhost",
        "X-OpenRouter-Title": "Incident Intelligence Platform"
    }

    try:
        response = requests.post(OPENROUTER_URL, headers=headers, json=payload, timeout=20)
        response.raise_for_status()
        result = response.json()

        text = result["choices"][0]["message"]["content"]
        text = text.replace("```json", "").replace("```", "").strip()

        parsed = json.loads(text)

        # Ensure confidence exists
        if "confidence" not in parsed:
            parsed["confidence"] = 0.75

        return parsed

    except Exception as e:
        return {
            "rootCause": "AI service failure",
            "technicalExplanation": str(e),
            "impactAnalysis": "Unable to analyze incident due to AI error",
            "recommendation": "Check OpenRouter connectivity and API response format",
            "priority": "MEDIUM",
            "confidence": 0.0
        }