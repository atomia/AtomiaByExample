Sample gRPC API client for python
=================================

PREREQUISITES
-------------

- `python 2.7 or above`
- `Atomia Billing 17.3 or above`

INSTALL
-------

  - Install the required python libs

  ```sh
  $ pip install grpcio-tools requests
  ```

  - Clone the AtomiaByExample repository

  ```sh
  $ # Get the AtomiaByExample repository
  $ git clone https://github.com/atomia/AtomiaByExample
  $ cd AtomiaByExample
  ```

  - Generate python bindings from the proto file

  ```sh
  $ cd gRPC/python
  $ python -m grpc.tools.protoc \
    -I ../proto \
    --python_out=. \
    --grpc_python_out=. \
    ../proto/AtomiaGrpcBilling.proto
  ```

TRY IT!
-------

  ```sh
  $ # From this directory
  $ python client.py -a billingapi.atomiadev.com \
    -p 50051 \
    -c root.pem \
    -i identity.atomiadev.com \
    -u Administrator \
    -w Administrator \
    -m "This is a sample echo request"
  ```

