#if INTERACTIVE
#r "../TypeProvider/bin/Debug/netstandard2.0/TypeProvider.dll"
#endif

open System

type TestType = Testing.Provider<"<unused>">

#if !INTERACTIVE
[<EntryPoint>]
let main _ =
#else
do ignore <|
#endif
    let mutable wasDisposed = false

    let notifyOnDeath = { new IDisposable with
        member __.Dispose() = wasDisposed <- true
    }

    // Test 'use __ = __' syntax.
    try
        TestType.TestDisposable(notifyOnDeath)

        failwith "We shouldn't get this far."
    with
    | exn when exn.Message <> "We shouldn't get this far." -> ()

    // Test different 'try..catch' scenarios (the call shouldn't throw).
    let status = TestType.TestTryWithFinally()

    assert(status = 0)

    // Everything worked.
    printf "Tests successfully passed."
    
    0

