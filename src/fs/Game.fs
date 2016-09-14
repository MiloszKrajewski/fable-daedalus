namespace Daedalus

open System
open System.Collections.Generic

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

open Daedalus.Js

module Game =
    type Location = int * int

    type Direction = | North | East | South | West

    let opposite direction = 
        match direction with 
        | North -> South | South -> North 
        | West -> East | East -> West

    let shift (x, y) direction =
        match direction with
        | North -> (x, y - 1) | South -> (x, y + 1) 
        | East -> (x + 1, y) | West -> (x - 1, y)

    type Action =
        | InitAt of Location
        | MoveTo of Location * Direction * Location

    let targetOf action = 
        match action with 
        | InitAt location -> location 
        | MoveTo (_, _, location) -> location

    let buildMaze width height = 
        let isValid (x, y) = x >= 0 && y >= 0 && x < width && y < height
        let visited = HashSet()
        let encode (x, y) = y*width + x 
        let mark location = location |> encode |> visited.Add |> ignore
        let test location = location |> encode |> visited.Contains

        // Location -> Action seq
        let fanout source =
            [| West; North; East; South |] 
            |> Array.map (fun direction -> MoveTo (source, direction, shift source direction))
            |> Array.filter (targetOf >> isValid)

        InitAt (0, 0) 
        |> DFS.stackless (targetOf >> mark) (targetOf >> test) (targetOf >> fanout >> Array.shuffle)
