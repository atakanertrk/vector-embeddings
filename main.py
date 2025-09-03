import certifi
from sentence_transformers import SentenceTransformer
import urllib.request, ssl
import ssl
import requests
from sympy import false
from http_proxy_configuration import disable_ssl_verification_globally
import json
import hashlib
import random
from http.server import BaseHTTPRequestHandler, HTTPServer

HOST = "localhost"
PORT = 1453
_MODEL = None

def get_model():
    global _MODEL
    if _MODEL is None:
        print("Loading model for the first time...")
        _MODEL = SentenceTransformer("sentence-transformers/all-MiniLM-L6-v2")
    return _MODEL

def embed(sentences):
    # sentences = ["This is an example sentence", "Each sentence is converted"]
    embeddings = get_model().encode(sentences).tolist()
    return embeddings

class Handler(BaseHTTPRequestHandler):
    def _send_json(self, status: int, payload: dict):
        body = json.dumps(payload).encode("utf-8")
        self.send_response(status)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(body)))
        # Simple CORS (optional; remove if not needed)
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Access-Control-Allow-Headers", "Content-Type")
        self.end_headers()
        self.wfile.write(body)


    def do_OPTIONS(self):
        # Basic CORS preflight
        self.send_response(204)
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Access-Control-Allow-Headers", "Content-Type")
        self.send_header("Access-Control-Allow-Methods", "POST, OPTIONS")
        self.end_headers()


    def do_POST(self):
        if self.path.rstrip("/") != "/embed":
            self._send_json(404, {"error": "Not Found"})
            return

        try:
            length = int(self.headers.get("Content-Length", "0"))
            raw = self.rfile.read(length)
            data = json.loads(raw.decode("utf-8"))
        except Exception as e:
            self._send_json(400, {"error": f"Invalid JSON: {e}"})
            return

        # Accept either {"texts": [...]} or raw JSON array [...]
        texts = None
        if isinstance(data, dict) and "texts" in data and isinstance(data["texts"], list):
            texts = data["texts"]
        elif isinstance(data, list):
            texts = data
        else:
            self._send_json(400, {"error": 'Expected {"texts": ["...", ...]} or ["...", ...]'})
            return

        # Validate items are strings
        if not all(isinstance(t, str) for t in texts):
            self._send_json(400, {"error": "All items must be strings"})
            return

        # Compute vectors
        vectors = embed(texts)
        self._send_json(200, {"Vectors": vectors})

if __name__ == '__main__':
    disable_ssl_verification_globally()
    server = HTTPServer((HOST, PORT), Handler)
    print(f"Listening on http://{HOST}:{PORT}  (POST /embed)")
    try:
        server.serve_forever()
    except KeyboardInterrupt:
        pass
    finally:
        server.server_close()
