-- Adds Google SSO account-linking support to orgcheck.logins.
-- googleid stores the Google "sub" claim (stable subject id) once a login has signed in
-- with Google at least once, so repeat sign-ins match directly instead of by email.
ALTER TABLE orgcheck.logins ADD COLUMN IF NOT EXISTS googleid text;

CREATE UNIQUE INDEX IF NOT EXISTS logins_googleid_key ON orgcheck.logins (googleid);
