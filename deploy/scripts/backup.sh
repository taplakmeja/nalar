#!/bin/sh
# Dumps the PostgreSQL database to a gzipped SQL file.
# Usage: ./backup.sh [output-directory]
set -eu
cd "$(dirname "$0")/.."

OUTPUT_DIR="${1:-backups}"
mkdir -p "$OUTPUT_DIR"
STAMP="$(date +%F-%H%M)"
FILE="$OUTPUT_DIR/nalar-$STAMP.sql.gz"

docker compose exec -T db pg_dump -U "${POSTGRES_USER:-psikotes}" "${POSTGRES_DB:-psikotes}" | gzip > "$FILE"
echo "Backup written: $FILE"
