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

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260331095516_sessions_to_users_fix') THEN
    DROP INDEX "IX_sessions_UserId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260331095516_sessions_to_users_fix') THEN
    CREATE INDEX "IX_sessions_UserId" ON sessions ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260331095516_sessions_to_users_fix') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260331095516_sessions_to_users_fix', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260408160729_storages') THEN
    CREATE TABLE storage_types (
        "Id" uuid NOT NULL,
        "Key" character varying(64) NOT NULL,
        "Label" character varying(64) NOT NULL,
        "Description" character varying(256) NOT NULL,
        "ConfigSchema" jsonb NOT NULL,
        CONSTRAINT "PK_storage_types" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260408160729_storages') THEN
    CREATE TABLE storages (
        "Id" uuid NOT NULL,
        "Name" character varying(128) NOT NULL,
        "StorageTypeId" uuid NOT NULL,
        "Config" jsonb NOT NULL,
        "IsPublic" boolean NOT NULL,
        "IsDefault" boolean NOT NULL DEFAULT FALSE,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_storages" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_storages_storage_types_StorageTypeId" FOREIGN KEY ("StorageTypeId") REFERENCES storage_types ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260408160729_storages') THEN
    CREATE TABLE storage_access (
        "StorageId" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "AccessType" integer NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_storage_access" PRIMARY KEY ("StorageId", "UserId"),
        CONSTRAINT "FK_storage_access_storages_StorageId" FOREIGN KEY ("StorageId") REFERENCES storages ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260408160729_storages') THEN
    CREATE INDEX "IX_storage_access_UserId" ON storage_access ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260408160729_storages') THEN
    CREATE UNIQUE INDEX "IX_storage_types_Key" ON storage_types ("Key");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260408160729_storages') THEN
    CREATE INDEX "IX_storage_types_Label" ON storage_types ("Label");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260408160729_storages') THEN
    CREATE INDEX "IX_storages_StorageTypeId" ON storages ("StorageTypeId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260408160729_storages') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260408160729_storages', '10.0.5');
    END IF;
END $EF$;
COMMIT;

