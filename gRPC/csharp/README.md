Sample gRPC API client for C Sharp
==================================

PREREQUISITES
-------------

- `.NET Framework 4.5+`
- `Visual Studio 2013 or 2015`
- `Atomia Billing 17.3 or greater`

INSTALL
-------

  - Clone the AtomiaByExample repository

  ```sh
  $ # Get the AtomiaByExample repository
  $ git clone https://github.com/atomia/AtomiaByExample
  ```

  - Restore NuGet packages

    1. Open solution `Client.sln` in the AtomiaByExample\gRPC\csharp folder with Visual Studio.
    2. Right click on the solution in the `Solution Explorer` and select `Restore NuGet Packages`.

  - Generate c# bindings from the proto file

  ```sh
  > cd AtomiaByExample\gRPC\csharp
  > generate_proto.bat
  ```

  - Build the solution from Visual Studio

TRY IT!
-------

  ```sh
  > cd AtomiaByExample\gRPC\csharp\Client\bin\Debug
  > client.exe -a billingapi.atomiadev.com ^
    -p 50051 ^
    -c root.pem ^
    -i identity.atomiadev.com ^
    -u Administrator ^
    -w Administrator ^
    -m "This is a sample echo request"
  ```

