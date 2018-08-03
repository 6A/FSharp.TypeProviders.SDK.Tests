This repository contains some tests for the [`try .. with`, `try .. finally` and `use`](https://github.com/fsprojects/FSharp.TypeProviders.SDK/pull/242)
support in [FSharp.TypeProviders.SDK](https://github.com/fsprojects/FSharp.TypeProviders.SDK).

Right now, the test type provider uses [my repository](https://github.com/6A/FSharp.TypeProviders.SDK),
since upstream does not have the support for the feature yet.

To run the tests, simply restore the project (`.paket/paket.exe restore`), and run the tests
(`dotnet run Tests/Tests.fsproj`).
