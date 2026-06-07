#!/bin/bash
set -e

create_db_if_not_exists() {
  local db=$1
  
  local exists=$(psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" \
    -tAc "SELECT 1 FROM pg_database WHERE datname = '$db';")

  if [ "$exists" != "1" ]; then
    psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" \
      -c "CREATE DATABASE $db ENCODING 'UTF8' LC_COLLATE 'C' LC_CTYPE 'C' TEMPLATE template0;"
  fi
}

create_db_if_not_exists "${POSTGRES_DB_APP}"
create_db_if_not_exists "${POSTGRES_DB_VCS}"

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL

  DO \$\$
  BEGIN
    IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = '${POSTGRES_APP_USER}') THEN
      CREATE USER ${POSTGRES_APP_USER} WITH PASSWORD '${POSTGRES_APP_PASS}';
    END IF;
  END
  \$\$;

  GRANT ALL PRIVILEGES ON DATABASE ${POSTGRES_DB_APP} TO ${POSTGRES_APP_USER};
  GRANT ALL PRIVILEGES ON DATABASE ${POSTGRES_DB_VCS} TO ${POSTGRES_APP_USER};

EOSQL

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "${POSTGRES_DB_APP}" \
  -c "GRANT ALL ON SCHEMA public TO ${POSTGRES_APP_USER};"

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "${POSTGRES_DB_VCS}" \
  -c "GRANT ALL ON SCHEMA public TO ${POSTGRES_APP_USER};"