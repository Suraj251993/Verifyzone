-- Stores per-customer email template overrides used by CustomerService.GetEmailTemplate.
-- Missing from the restored schema dump because this feature was added after it was taken.
CREATE TABLE orgcheck.customeremailsetting (
    id                integer NOT NULL GENERATED ALWAYS AS IDENTITY,
    customerid        integer,
    templateid        integer NOT NULL,
    templatecontent   text,
    createdby         integer NOT NULL,
    createdcustomerid integer NOT NULL,
    createddate       timestamp with time zone NOT NULL,
    CONSTRAINT customeremailsetting_pkey PRIMARY KEY (id),
    CONSTRAINT ces_login     FOREIGN KEY (createdby)         REFERENCES orgcheck.logins (id),
    CONSTRAINT ces_customer2 FOREIGN KEY (createdcustomerid) REFERENCES orgcheck.customer (id),
    CONSTRAINT ces_customer  FOREIGN KEY (customerid)        REFERENCES orgcheck.customer (id)
);

CREATE INDEX idx_customeremailsetting_createdcustomerid ON orgcheck.customeremailsetting (createdcustomerid);
CREATE INDEX idx_customeremailsetting_templateid ON orgcheck.customeremailsetting (templateid);
