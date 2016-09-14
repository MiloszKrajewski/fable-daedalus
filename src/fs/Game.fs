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

    type Action =
        | InitAt of Location
        | MoveTo of Location * Direction * Location

    type Room = 
        {
            Location: Location
            mutable Visited: bool
            Exits: ResizeArray<Direction>
        }

    type World = 
        {
            Size: Location
            Rooms: Room[][]
        }

    let opposite direction = 
        match direction with 
        | North -> South | South -> North 
        | West -> East | East -> West

    let shift direction (x, y) =
        match direction with
        | North -> (x, y - 1) | South -> (x, y + 1) 
        | East -> (x + 1, y) | West -> (x - 1, y)

    let targetOf action = 
        match action with 
        | InitAt location -> location 
        | MoveTo (_, _, location) -> location

    let createRoom x y = { Location = (x, y); Visited = false; Exits = ResizeArray() }

    let createRooms width height =
        Array.init height (fun y -> Array.init width (fun x -> createRoom x y))

    let createWorld width height = { Size = (width, height); Rooms = createRooms width height }

    let enumerateActions x y (world: World) = 
        let isValid (x, y) = let w, h = world.Size in x >= 0 && y >= 0 && x < w && y < h
        let roomAt (x, y) = world.Rooms.[y].[x]
        let mark action = (action |> targetOf |> roomAt).Visited <- true
        let test action = (action |> targetOf |> roomAt).Visited

        let createAction source direction = 
            source 
            |> shift direction |> Some 
            |> Option.filter isValid 
            |> Option.map (fun target -> MoveTo (source, direction, target))

        let fanout action = 
            [| West; North; East; South |] 
            |> Array.choose (action |> targetOf |> createAction)

        let path = InitAt (x, y) |> DFS.stackless mark test (fanout >> Array.shuffle)

        let updateExits action =
            match action with
            | InitAt _ -> ()
            | MoveTo (source, direction, target) ->
                direction |> (roomAt source).Exits.Add |> ignore 
                opposite direction |> (roomAt target).Exits.Add |> ignore 
 
        path |> Seq.map (apply updateExits) 
