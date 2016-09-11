namespace Daedalus

open System
open System.Collections.Generic

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

open Fable.Helpers.Virtualdom
open Fable.Helpers.Virtualdom.App
open Fable.Helpers.Virtualdom.Html

open Daedalus.Js

module Game =
    type Coords = int * int
    type Direction = | North | East | South | West

    type Action =
        | InitAt of Coords
        | MoveTo of Coords * Direction * Coords

    type Room = 
        {
            Position: Coords
            Visited: bool
            Exits: Direction list
        }

    type World = 
        {
            Size: Coords
            Rooms: Map<Coords, Room>
        }

    let newRoom x y = { 
        Position = (x, y)
        Visited = false
        Exits = []
    }

    let createRooms width height = seq {
        for y = 0 to height - 1 do
            for x = 0 to width - 1 do
                yield newRoom x y
    }

    let newWorld width height = {
        Size = (width, height)
        Rooms = createRooms width height |> Seq.map (fun r -> r.Position, r) |> Map.ofSeq
    }

    let opposite direction = 
        match direction with 
        | North -> South | South -> North 
        | West -> East | East -> West

    let buildMaze x y (world: World) = 
        let w, h = world.Size

        let valid (x, y) = x >= 0 && y >= 0 && x < w && y < h

        let shift direction (x, y) =
            match direction with
            | North -> (x, y - 1) | South -> (x, y + 1) 
            | East -> (x + 1, y) | West -> (x - 1, y)
            |> Some |> Option.filter valid

        let sourcexy action = 
            match action with | MoveTo (xy, _, _) -> Some xy | _ -> None 

        let targetxy action = 
            match action with | InitAt xy -> xy | MoveTo (_, _, xy) -> xy

        let next location direction = 
            location |> shift direction |> Option.map (fun xy -> MoveTo (location, direction, xy))

        let fanout action = 
            [| West; North; East; South |] |> Array.choose (action |> targetxy |> next)

        let encode (x, y) = y*w + x 

        let shuffle a = a
        let visited = HashSet()
        let mark = targetxy >> encode >> visited.Add >> ignore
        let test = targetxy >> encode >> visited.Contains
        let path = InitAt (x, y) |> DFS.traverse mark test (fanout >> shuffle)

        let updateSource action rooms = 
            match action with
            | InitAt _ -> rooms
            | MoveTo (location, direction, _) -> 
                rooms |> Map.update location (fun room -> 
                    { room with Exits = direction :: room.Exits })

        let updateTarget action rooms =
            match action with
            | InitAt location -> 
                rooms |> Map.update location (fun room -> { room with Visited = true })
            | MoveTo (location, direction, _) -> 
                rooms |> Map.update location (fun room -> 
                    { room with Visited = true; Exits = (opposite direction) :: room.Exits })

        let fold world action =
            printfn "%A" action
            { world with Rooms = world.Rooms |> updateSource action |> updateTarget action }

        path |> Seq.scan fold world
