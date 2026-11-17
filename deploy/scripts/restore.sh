#!/bin/sh
# Restores a gzipped SQL dump produced by backup.sh.
# Usage: ./restore.sh <backup-file.sql.gz>
set -eu
cd "$(dirname "$0")/.."

FILE="${1:?Usage: ./restore.sh <backup-file.sql.gz>}"
[ -f "$FILE" ] || { echo "File not found: $FILE"; exit 1; }

echo "WARNING: this overwrites the current database. Press Ctrl+C to abort."
read -r dummy

gunzip -c "$FILE" | docker compose exec -T db psql -U "${POSTGRES_USER:-psikotes}" -d "${POSTGRES_DB:-psikotes}"
echo "Restore complete."
