CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260329135232_init') THEN
    CREATE TABLE users (
        "Id" uuid NOT NULL,
        "Username" character varying(64) NOT NULL,
        "DisplayName" character varying(128) NOT NULL,
        "Email" character varying(256) NOT NULL,
        "IsEmailVerified" boolean NOT NULL DEFAULT FALSE,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        "DeletedAt" timestamp with time zone,
        CONSTRAINT "PK_users" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260329135232_init') THEN
    CREATE TABLE user_avatars (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "StorageKey" character varying(512) NOT NULL,
        "Url" character varying(512) NOT NULL,
        "SizeBytes" bigint NOT NULL,
        "MimeType" character varying(128) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_user_avatars" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_user_avatars_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260329135232_init') THEN
    CREATE UNIQUE INDEX "IX_user_avatars_UserId" ON user_avatars ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260329135232_init') THEN
    CREATE UNIQUE INDEX "IX_users_Email" ON users ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260329135232_init') THEN
    CREATE UNIQUE INDEX "IX_users_Username" ON users ("Username");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260329135232_init') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260329135232_init', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260330123419_auth_basics') THEN
    CREATE TABLE sessions (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        device_user_agent character varying(512),
        device_name character varying(128),
        device_os character varying(128),
        device_browser character varying(128),
        "IpAddress" character varying(64) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "LastActiveAt" timestamp with time zone NOT NULL,
        "ExpiresAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_sessions" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_sessions_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260330123419_auth_basics') THEN
    CREATE TABLE user_credentials (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "PasswordHash" character varying(512) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_user_credentials" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_user_credentials_users_UserId" FOREIGN KEY ("UserId") REFERENCES users ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260330123419_auth_basics') THEN
    CREATE TABLE refresh_tokens (
        "Id" uuid NOT NULL,
        "SessionId" uuid NOT NULL,
        "TokenHash" character varying(128) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "ExpiresAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_refresh_tokens" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_refresh_tokens_sessions_SessionId" FOREIGN KEY ("SessionId") REFERENCES sessions ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260330123419_auth_basics') THEN
    CREATE UNIQUE INDEX "IX_refresh_tokens_SessionId" ON refresh_tokens ("SessionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260330123419_auth_basics') THEN
    CREATE UNIQUE INDEX "IX_sessions_UserId" ON sessions ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260330123419_auth_basics') THEN
    CREATE UNIQUE INDEX "IX_user_credentials_UserId" ON user_credentials ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260330123419_auth_basics') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260330123419_auth_basics', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260330222608_sessions_revoke') THEN
    ALTER TABLE sessions DROP COLUMN "ExpiresAt";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260330222608_sessions_revoke') THEN
    ALTER TABLE sessions ADD "RevokedAt" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260330222608_sessions_revoke') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260330222608_sessions_revoke', '10.0.5');
    END IF;
END $EF$;
COMMIT;

