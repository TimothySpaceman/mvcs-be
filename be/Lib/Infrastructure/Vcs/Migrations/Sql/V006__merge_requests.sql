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
    VALUES ('20260508120759_init', '10.0.8');
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
    VALUES ('20260509154336_hex_hashes', '10.0.8');
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
    VALUES ('20260513103725_refs', '10.0.8');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608105046_multibranch') THEN
    ALTER TABLE commits ADD "Kind" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608105046_multibranch') THEN
    ALTER TABLE commits ADD "SecondParentId" character varying(32);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608105046_multibranch') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260608105046_multibranch', '10.0.8');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260609182417_commits_composite_pk') THEN
    ALTER TABLE commits DROP CONSTRAINT "PK_commits";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260609182417_commits_composite_pk') THEN
    ALTER TABLE commits ADD CONSTRAINT "PK_commits" PRIMARY KEY ("Id", "ProjectId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260609182417_commits_composite_pk') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260609182417_commits_composite_pk', '10.0.8');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611105538_merge_requests') THEN
    CREATE TABLE merge_requests (
        "Id" uuid NOT NULL,
        "AuthorId" uuid NOT NULL,
        "ProjectId" uuid NOT NULL,
        "Title" character varying(4096) NOT NULL,
        "TargetRefName" character varying(256) NOT NULL,
        "SourceRefName" character varying(256) NOT NULL,
        "MergeCommitId" character varying(32) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_merge_requests" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611105538_merge_requests') THEN
    CREATE INDEX "IX_merge_requests_ProjectId" ON merge_requests ("ProjectId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611105538_merge_requests') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260611105538_merge_requests', '10.0.8');
    END IF;
END $EF$;
COMMIT;

