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
         
    // let rec dfs visited fanout start = seq {
    //     match start |> visited with
    //     | false -> yield start; yield! start |> fanout |> Seq.filter (visited >> not) |> Seq.collect (dfs1 visited fanout)
    //     | true -> ()
    // }

    let dfs visited fanout start =
        let rec loop stack = seq {
            match stack with
            | [] -> ()
            | head :: tail when visited head -> yield! loop tail 
            | head :: tail ->
                yield head
                let head' = head |> fanout |> Seq.filter (visited >> not) |> List.ofSeq
                yield! loop (head' @ tail)
        }
        loop [start]
