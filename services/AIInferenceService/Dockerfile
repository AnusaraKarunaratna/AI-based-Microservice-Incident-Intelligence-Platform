FROM python:3.11-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

# FIX: Expose 8080 to match docker-compose port mapping
EXPOSE 8080

# FIX: Run uvicorn on 0.0.0.0:8080
CMD ["uvicorn", "app:app", "--host", "0.0.0.0", "--port", "8080"]