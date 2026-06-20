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

