Sample gRPC API client for Node.js
==================================

PREREQUISITES
-------------

- `node 4.6.x or above`
- `Atomia Billing 17.3 or above`

INSTALL
-------

  ```sh
  $ # Get the AtomiaByExample repository
  $ git clone https://github.com/atomia/AtomiaByExample
  $ cd AtomiaByExample/gRPC/node
  $ npm install
  ```

TRY IT!
-------

  ```sh
  $ # From this directory
  $ node app.js -a billingapi.atomiadev.com \
    -p 50051 \
    -c root.pem \
    -i identity.atomiadev.com \
    -u Administrator \
    -w Administrator \
    -m "This is a sample echo request"
  ```

