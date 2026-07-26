-- Demo login accounts (all use password: Demo@1234)
INSERT INTO orgcheck.logins (loginname, password, usertypeid, customerid, customertypeid, status, displayname, emailid, contactnumber, designation)
VALUES
    ('demo@verifyzone.local', 'VRBkmyUlcoSltvS+mbqleuGmU4dzUJYchPkuyKfhw+8=', 1, NULL, NULL, 1, 'Demo Admin', 'demo@verifyzone.local', '9999999999', 'Demo Administrator'),
    ('democustomer@verifyzone.local', 'VRBkmyUlcoSltvS+mbqleuGmU4dzUJYchPkuyKfhw+8=', 2, 2, 3, 1, 'Demo Customer User', 'democustomer@verifyzone.local', '9999999991', 'Demo Customer'),
    ('demoinstitution@verifyzone.local', 'VRBkmyUlcoSltvS+mbqleuGmU4dzUJYchPkuyKfhw+8=', 3, 4, NULL, 1, 'Demo Institution User', 'demoinstitution@verifyzone.local', '9999999992', 'Demo Institution'),
    ('democompany@verifyzone.local', 'VRBkmyUlcoSltvS+mbqleuGmU4dzUJYchPkuyKfhw+8=', 4, NULL, NULL, 1, 'Demo Company User', 'democompany@verifyzone.local', '9999999993', 'Demo Company'),
    ('demosupport@verifyzone.local', 'VRBkmyUlcoSltvS+mbqleuGmU4dzUJYchPkuyKfhw+8=', 5, NULL, NULL, 1, 'Demo Support User', 'demosupport@verifyzone.local', '9999999994', 'Demo Support');
