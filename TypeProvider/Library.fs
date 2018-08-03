namespace FSharp.TypeProviders.SDK.Testing

open ProviderImplementation.ProvidedTypes
open FSharp.Core.CompilerServices

open System
open System.Reflection

#nowarn "0025" // Incomplete pattern matches


[<TypeProvider>]
type TryWithFinallyUsingProvider(config : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces(config)

    let ns = "Testing"
    let asm = Assembly.GetExecutingAssembly()

    let provider = ProvidedTypeDefinition(asm, ns, "Provider", Some typeof<obj>, hideObjectMethods = true)

    do provider.DefineStaticParameters( [ProvidedStaticParameter("Unused", typeof<string>)],
                                        fun name _ ->
        
        let provided = ProvidedTypeDefinition(asm, ns, name, Some typeof<obj>, hideObjectMethods = true)

        ProvidedMethod("TestDisposable", [ ProvidedParameter("disp", typeof<IDisposable>) ], typeof<System.Void>, fun [ arg ] ->
            <@@
                use __ = (%%arg : IDisposable)

                failwith "This will throw and dispose the given argument."
                0
            @@>
        , isStatic = true) |> provided.AddMember

        ProvidedMethod("TestTryWithFinally", [], typeof<int>, fun [] ->
            <@@
                try
                    raise <| ArgumentException("Throwing exception...")
                with
                | :? ArgumentException as exn when exn.Message = "Throwing exception..." ->
                    let mutable status = 3

                    try
                        try
                            failwith "Failing"
                        finally
                            status <- 2
                    with _ ->
                        assert(status = 2)

                        status <- 1
                    
                    assert(status = 1)

                    try
                        try
                            failwith "This will not get caught."
                        with
                        | :? ArgumentException -> status <- 5
                        | exn when exn.Message = "This will get caught." -> status <- 6
                    with
                    | _ -> assert(status = 1) // Status shouldn't have changed.

                    assert(status = 1)

                    status <-
                        try
                            try
                                failwith "Nope."
                            with
                            | _ -> 0
                        with
                        | _ -> -1
                    
                    status
            @@>
        , isStatic = true) |> provided.AddMember

        provided
    )

    do this.AddNamespace(ns, [provider])


[<assembly: TypeProviderAssembly>]
do ()
