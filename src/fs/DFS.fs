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

    let rec stackless mark test fanout shake node = seq {
        let mutable stack = [[node]]
        while stack.Length > 0 do
            let head, stack' = 
                match shake stack with
                | [] -> None, [] 
                | [] :: rest -> None, rest
                | (head :: tail) :: rest ->
                    if test head then None, tail :: rest
                    else head |> apply mark |> Some, (head |> fanout |> List.ofSeq) :: tail :: rest
            match head with | Some n -> yield n | _ -> ()
            stack <- stack'
    }
