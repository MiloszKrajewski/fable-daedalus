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

    type State = {
        Current: Coords option
        World: World
        Stack: Coords list 
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

    let shift direction (x, y) = 
        match direction with
        | North -> (x, y - 1)
        | East -> (x + 1, y)
        | South -> (x, y + 1)
        | West -> (x - 1, y)
         
    // let rec dfs1 visited next start = seq {
    //     match start |> visited with
    //     | false -> yield start; yield! start |> next |> Seq.collect (dfs1 visited next)
    //     | true -> ()
    // }

    let dfs2 visited fanout start = seq {
        let mutable stack = [start]
        let push item = stack <- item :: stack
        let pop () = match stack with | [] -> None | head :: tail -> stack <- tail; Some head
        let next = pop >> Option.filter (visited >> not)

        let rec loop () = seq {
            match next () with
            | None -> ()
            | Some item -> 
                yield item
                item |> fanout |> Seq.iter push
                yield! loop ()
        }

        yield! loop ()
    }
