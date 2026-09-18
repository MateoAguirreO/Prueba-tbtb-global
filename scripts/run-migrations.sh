#!/usr/bin/env bash
set -euo pipefail

for f in $(ls /scripts/*.sql | sort); do
  echo "Aplicando $f"
  /opt/mssql-tools18/bin/sqlcmd -S "$DB_HOST" -U sa -P "$MSSQL_SA_PASSWORD" -C -b -i "$f"
done

echo "Migraciones aplicadas"
