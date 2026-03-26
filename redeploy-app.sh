#!/usr/bin/env bash
echo "Pulling latest code..."
git pull --ff-only

echo "Building frontend/backend services..."
docker compose build depvisprocessing depviscore frontend

echo "Restarting only frontend/backend services..."
docker compose up -d --no-deps depvisprocessing depviscore frontend

echo "Current status:"
docker compose ps