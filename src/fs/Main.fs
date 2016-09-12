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
open Daedalus.Game
open Daedalus.VDOM

module Main =
    let [<Literal>] WORLD_WIDTH = 20
    let [<Literal>] WORLD_HEIGHT = 20
    let [<Literal>] ROOM_SIZE = 10
    let [<Literal>] DOOR_SIZE = 2

    type Model = {
        World: World
        Engine: Enumerator<World> option
    }

    type Event = | Restart | Tick

    let newModel () =
        let world = newWorld WORLD_WIDTH WORLD_HEIGHT
        { World = world; Engine = world |> Game.buildMaze 0 0 |> Enumerator.create }

    let nextModel (model: Model) = 
        match model.Engine with
        | None -> model
        | Some engine -> 
            let world = engine |> Enumerator.value
            { model with World = world; Engine = engine |> Enumerator.next }

    let withTick (model: Model) =
        match model.Engine with
        | None -> Time.now () |> printfn "Stop: %f"; model, []
        | Some _ -> model, [deferEvent 0.0 Tick]

    let update model event =
        match event with
        | Restart -> Time.now () |> printfn "Start: %f"; newModel ()
        | Tick -> model |> nextModel
        |> withTick

    let renderBox className (x: int) (y: int) (w: int) (h: int) =
        rect [
            klass className 
            attr "x" (string x); attr "y" (string y) 
            attr "width" (string w); attr "height" (string h)
        ] []

    let renderRoom (room: Room) = [
        let x, y = room.Position
        let x, y = x*ROOM_SIZE + (x + 1)*DOOR_SIZE, y*ROOM_SIZE + (y + 1)*DOOR_SIZE
        yield renderBox "room" x y ROOM_SIZE ROOM_SIZE
        for exit in room.Exits do
            match exit with
            | South -> yield renderBox "door" x (y + ROOM_SIZE) ROOM_SIZE DOOR_SIZE
            | East -> yield renderBox "door" (x + ROOM_SIZE) y DOOR_SIZE ROOM_SIZE
            | _ -> ()
    ]

    let renderWorld (world: World) = 
        world.Rooms 
        |> Map.toList 
        |> List.collect (fun (_, r) -> if r.Visited then renderRoom r else [])

    let button label action =
        Tags.button 
            [ itype "button"; klass "btn"; onMouseClick (fun _ -> action) ] 
            [ text label ]

    let view model =
        let w, h = model.World.Size

        let mazeWidth = attr "width" (w*ROOM_SIZE + (w + 1)*DOOR_SIZE |> string) 
        let mazeHeight = attr "height" (h*ROOM_SIZE + (h + 1)*DOOR_SIZE |> string)
        let maze = svg [klass "maze"; mazeWidth; mazeHeight] 

        div [klass "container"] [
            div [klass "row content"] [
                button "Restart" Restart
            ]
            div [klass "row content"] [
                model.World |> renderWorld |> maze 
            ]
        ]

    let main () =
        printfn "version 1"

        createApp (newModel ()) view update 
        |> withStartNodeSelector "#main" 
        |> start renderer
