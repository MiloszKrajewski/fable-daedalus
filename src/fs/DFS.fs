namespace Daedalus

open Fable.Core
open System.Collections.Generic

open Daedalus.Js

module DFS =
    let rec traverse mark test fanout node = seq {
        match node |> test with
        | false -> 
            yield node |> apply mark
            yield! node |> fanout |> Seq.collect (traverse mark test fanout)
        | _ -> ()  
    }
