Sample gRPC API clients
=======================

OVERVIEW
--------

The examples here will help get you started with how to write code that uses the gRPC endpoints for the Atomia Platform.

BEFORE YOU BEGIN
----------------

Before you begin you will have to export the root certificate that Atomia Identity uses. You can find out the name of your root certificate by editing your `unattended.ini` file and look for the `RootCertName` option. This file is usually located at `C:\Program Files (x86)\Atomia\Common\unattended.ini`.

Follow the steps below to export the certificate:
1. Connect to your Identity server.

2. Launch `mmc.exe`.

3. Go to `File > Add/Remove Snap-in...`.

4. Select `Certificates` and click the `Add` button.

5. Select `Computer account` in the window and click the `Next` button.

6. Click the `Finish` button.

7. Click the `OK` button.

8. Navigate to `Certificates > Trusted Root Certification Authorities > Certificates`.

9. Right click on the root certificate that Atomia Identity uses.

10. Select `All Tasks > Export...`.

11. Click the `Next` button.

12. Select `Base-64 encoded X.509 (.CER)` and click the `Next` button.

13. Save the certificate as `root.pem` on your computer and click the `Next` button.

14. Make sure that everything looks OK and click the `Finish` button or go back.


TRY IT!
-------

There are examples for C Sharp, Node.js, PHP and Python. Navigate in to the appropriate directory and check the README.md for further instructions.

