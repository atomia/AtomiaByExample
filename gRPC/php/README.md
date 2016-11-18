Sample gRPC API client for PHP
==============================

PREREQUISITES
-------------

- `php 7`
- `phpize`
- `pecl`
- `php-curl`
- `Atomia Billing 17.3 or above`

INSTALL
-------

  - Install the protoc binary

  ```sh
  $ wget https://github.com/google/protobuf/releases/download/v3.0.0/protoc-3.0.0-linux-x86_64.zip
  $ unzip -p protoc-3.0.0-linux-x86_64.zip bin/protoc > protoc
  $ [sudo] mv protoc /usr/local/bin
  $ [sudo] chmod 755 /usr/local/bin/protoc
  ```

  - Install the gRPC PHP extension

  ```sh
  $ [sudo] pecl install grpc
  $ # Append extension=grpc.so to your php.ini
  $ # The line below has been tried on Ubuntu 16.04
  $ [sudo] echo extension=grpc.so >> /etc/php/7.0/cli/php.ini
  ```

  - Install the PHP protobuf compiler

  ```sh
  $ git clone https://github.com/stanley-cheung/Protobuf-PHP
  $ cd Protobuf-PHP
  $ [sudo] gem install rake ronn
  $ rake pear:package version=1.0
  $ [sudo] pear install Protobuf-1.0.tgz
  ```

  - Clone the AtomiaByExample repository

  ```sh
  $ # Get the AtomiaByExample repository
  $ git clone https://github.com/atomia/AtomiaByExample
  $ cd AtomiaByExample
  ```

  - Install composer

  ```sh
  $ cd gRPC/php
  $ curl -sS https://getcomposer.org/installer | php
  $ php composer.phar install
  ```

  - Generate PHP bindings from the proto file

  ```sh
  $ protoc-gen-php -i ../proto -o . ../proto/AtomiaGrpcBaseTypes.proto
  $ protoc-gen-php -i ../proto -o . ../proto/AtomiaGrpcAccount.proto
  $ protoc-gen-php -i ../proto -o . ../proto/AtomiaGrpcBilling.proto
  ```

TRY IT!
-------

  ```sh
  $ # From this directory
  $ php client.php -a billingapi.atomiadev.com \
    -p 50051 \
    -c root.pem \
    -i identity.atomiadev.com \
    -u Administrator \
    -w Administrator \
    -m "This is a sample echo request"
  ```

