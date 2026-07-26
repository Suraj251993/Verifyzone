-- Employee Consent Management module
-- New tables: orgcheck.lookup_consentstatus, orgcheck.consentrequests, orgcheck.consentauditlogs

CREATE TABLE orgcheck.lookup_consentstatus (
    id integer NOT NULL,
    name text NOT NULL,
    CONSTRAINT lookup_consentstatus_pkey PRIMARY KEY (id)
);

INSERT INTO orgcheck.lookup_consentstatus (id, name) VALUES
    (1, 'Pending'),
    (2, 'Approved'),
    (3, 'Expired'),
    (4, 'Cancelled');

CREATE TABLE orgcheck.consentrequests (
    id integer GENERATED ALWAYS AS IDENTITY,
    consentrequestid text NOT NULL,
    customerid integer NOT NULL,
    employeefirstname text NOT NULL,
    employeelastname text NOT NULL,
    employeecode text,
    employeeemail text NOT NULL,
    optionalemail text,
    statusid integer NOT NULL DEFAULT 1,
    token text NOT NULL,
    tokenconsumed boolean NOT NULL DEFAULT false,
    tokenexpirydate timestamp with time zone NOT NULL,
    consentdate timestamp with time zone,
    ipaddress text,
    device text,
    browser text,
    createdby integer NOT NULL,
    createddate timestamp with time zone NOT NULL DEFAULT now(),
    modifiedby integer,
    modifieddate timestamp with time zone,
    CONSTRAINT consentrequests_pkey PRIMARY KEY (id),
    CONSTRAINT consentrequests_consentrequestid_key UNIQUE (consentrequestid),
    CONSTRAINT consentrequests_token_key UNIQUE (token),
    CONSTRAINT consreq_customer FOREIGN KEY (customerid) REFERENCES orgcheck.customer (id),
    CONSTRAINT consreq_status FOREIGN KEY (statusid) REFERENCES orgcheck.lookup_consentstatus (id),
    CONSTRAINT consreq_createdby FOREIGN KEY (createdby) REFERENCES orgcheck.logins (id),
    CONSTRAINT consreq_modifiedby FOREIGN KEY (modifiedby) REFERENCES orgcheck.logins (id)
);

CREATE INDEX idx_consentrequests_customerid ON orgcheck.consentrequests (customerid);
CREATE INDEX idx_consentrequests_token ON orgcheck.consentrequests (token);

CREATE TABLE orgcheck.consentauditlogs (
    id integer GENERATED ALWAYS AS IDENTITY,
    consentrequestid integer NOT NULL,
    action text NOT NULL,
    oldstatusid integer,
    newstatusid integer,
    performedby integer,
    ipaddress text,
    useragent text,
    remarks text,
    createddate timestamp with time zone NOT NULL DEFAULT now(),
    CONSTRAINT consentauditlogs_pkey PRIMARY KEY (id),
    CONSTRAINT conslog_consreq FOREIGN KEY (consentrequestid) REFERENCES orgcheck.consentrequests (id),
    CONSTRAINT conslog_performedby FOREIGN KEY (performedby) REFERENCES orgcheck.logins (id),
    CONSTRAINT conslog_oldstatus FOREIGN KEY (oldstatusid) REFERENCES orgcheck.lookup_consentstatus (id),
    CONSTRAINT conslog_newstatus FOREIGN KEY (newstatusid) REFERENCES orgcheck.lookup_consentstatus (id)
);

CREATE INDEX idx_consentauditlogs_consentrequestid ON orgcheck.consentauditlogs (consentrequestid);
