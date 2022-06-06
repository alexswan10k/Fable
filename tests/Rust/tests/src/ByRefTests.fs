module Fable.Tests.ByRefTests

open Util.Testing

let byrefIntFn (x: int inref) =
    x + 1

[<Fact>]
let ``pass int by ref works`` () =
    let a = 1
    byrefIntFn &a |> equal 2
    a |> equal 1 // a is not modified & prevent inlining

type Obj = {
    X: int
}

let byrefObjFn (x: Obj inref) =
    x.X + 1

[<Fact>]
let ``pass obj by ref works`` () =
    let a = { X = 1 }
    let b = { X = 2 }
    byrefObjFn &a |> equal 2
    byrefObjFn &b |> equal 3
    a |> equal a //prevent inlining