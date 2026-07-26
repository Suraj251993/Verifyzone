-- Employee Consent Management module - v2
-- Adds audit fields for which consent document version was shown/accepted
-- and the channel the employee used to reach the consent page.

ALTER TABLE orgcheck.consentrequests
    ADD COLUMN consentdocumentversion text;

ALTER TABLE orgcheck.consentrequests
    ADD COLUMN consentsource text NOT NULL DEFAULT 'EmailLink';
