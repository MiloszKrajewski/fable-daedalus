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
    let [<Literal>] ROOM_SIZE = 20
    let [<Literal>] DOOR_SIZE = 5

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
        sprintf "M%d,%d h%dv%dh%dz" x y w h -w

    let renderDoor (room: Room) = 
        let x, y = room.Position
        let x, y = x*ROOM_SIZE + (x + 1)*DOOR_SIZE, y*ROOM_SIZE + (y + 1)*DOOR_SIZE
        [|
            for exit in room.Exits do
                match exit with
                | South -> yield renderBox "door" x (y + ROOM_SIZE) ROOM_SIZE DOOR_SIZE
                | East -> yield renderBox "door" (x + ROOM_SIZE) y DOOR_SIZE ROOM_SIZE
                | _ -> ()
        |] |> Array.join " " 

    let renderRoom (room: Room) = 
        let x, y = room.Position
        let x, y = x*ROOM_SIZE + (x + 1)*DOOR_SIZE, y*ROOM_SIZE + (y + 1)*DOOR_SIZE
        renderBox "room" x y ROOM_SIZE ROOM_SIZE
    
    let mergePaths className renderObject (rooms: Room[]) =
        rooms 
        |> Array.map renderObject
        |> Array.join " "
        |> fun p -> Svg.svgElem "path" [klass className; attr "d" p] []
 
    let renderWorld (world: World) = [
        let rooms = world.Rooms |> Array.collect id |> Array.filter (fun r -> r.Visited)
        yield rooms |> mergePaths "room" renderRoom
        yield rooms |> mergePaths "door" renderDoor
    ]

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
