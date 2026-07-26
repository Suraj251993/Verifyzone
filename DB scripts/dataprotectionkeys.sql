-- Persists ASP.NET Core Data Protection keys (used to encrypt auth cookies, antiforgery/CSRF tokens)
-- so they survive container redeploys instead of regenerating (and invalidating in-flight tokens/cookies).
CREATE TABLE orgcheck.dataprotectionkeys (
    id integer GENERATED ALWAYS AS IDENTITY,
    friendlyname text,
    xml text,
    CONSTRAINT dataprotectionkeys_pkey PRIMARY KEY (id)
);
