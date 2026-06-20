CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260508120759_init') THEN
    CREATE TABLE blob_metadata (
        "Id" bytea NOT NULL,
        "ProjectId" uuid NOT NULL,
        "Length" bigint NOT NULL,
        CONSTRAINT "PK_blob_metadata" PRIMARY KEY ("Id", "ProjectId")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260508120759_init') THEN
    CREATE TABLE commits (
        "Id" bytea NOT NULL,
        "ProjectId" uuid NOT NULL,
        "ParentId" bytea,
        "Message" character varying(4096) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "AuthorId" uuid,
        "AuthorName" character varying(256) NOT NULL,
        "AuthorEmail" character varying(512),
        "Changes" jsonb NOT NULL,
        CONSTRAINT "PK_commits" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260508120759_init') THEN
    CREATE INDEX "IX_blob_metadata_ProjectId" ON blob_metadata ("ProjectId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260508120759_init') THEN
    CREATE INDEX "IX_commits_ProjectId" ON commits ("ProjectId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260508120759_init') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260508120759_init', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260509154336_hex_hashes') THEN
    ALTER TABLE commits ALTER COLUMN "ParentId" TYPE character varying(32);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260509154336_hex_hashes') THEN
    ALTER TABLE commits ALTER COLUMN "Id" TYPE character varying(32);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260509154336_hex_hashes') THEN
    ALTER TABLE blob_metadata ALTER COLUMN "Id" TYPE character varying(32);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260509154336_hex_hashes') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260509154336_hex_hashes', '10.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260513103725_refs') THEN
    CREATE TABLE refs (
        "ProjectId" uuid NOT NULL,
        "Name" character varying(256) NOT NULL,
        "CommitId" character varying(32),
        CONSTRAINT "PK_refs" PRIMARY KEY ("ProjectId", "Name")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260513103725_refs') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260513103725_refs', '10.0.5');
    END IF;
END $EF$;
COMMIT;

