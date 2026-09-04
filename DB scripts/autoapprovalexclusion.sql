-- Stores employees excluded from autopilot re-verification approval.
-- Missing from the restored schema dump because this feature was added after it was taken.
CREATE TABLE orgcheck.autoapprovalexclusion (
    id          integer NOT NULL GENERATED ALWAYS AS IDENTITY,
    customerid  integer,
    employeeid  integer,
    createdby   integer NOT NULL,
    createddate timestamp with time zone NOT NULL,
    CONSTRAINT autoapprovalexclusion_pkey PRIMARY KEY (id),
    CONSTRAINT aae_login    FOREIGN KEY (createdby)  REFERENCES orgcheck.logins (id),
    CONSTRAINT aae_customer FOREIGN KEY (customerid) REFERENCES orgcheck.customer (id),
    CONSTRAINT aae_employee FOREIGN KEY (employeeid) REFERENCES orgcheck.employee (id)
);
