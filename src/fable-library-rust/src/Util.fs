module Util_

let equals (x: 'T) (y: 'T): bool =
    x = y

let compare (x: 'T) (y: 'T): int =
    if x > y then 1
    elif x < y then -1
    else 0

let ignore (x: 'T): unit = ()

[<CompiledName("divRem")>]
let inline divRem (x: 'T) (y: 'T): struct ('T * 'T) =
    let quotient = x / y
    let remainder = x % y
    struct (quotient, remainder)

[<CompiledName("divRemOut")>]
let inline divRemOut (x: 'T) (y: 'T) (remainder: 'T outref): 'T =
    let quotient = x / y
    remainder <- x % y
    quotient
