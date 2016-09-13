namespace Daedalus

open System
open System.Collections.Generic

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

open Daedalus.Js
open Daedalus.Game

module Main =
    let [<Literal>] WORLD_WIDTH = 20
    let [<Literal>] WORLD_HEIGHT = 20
    let [<Literal>] ROOM_SIZE = 20
    let [<Literal>] DOOR_SIZE = 5

    let jq = importDefault<obj> "jquery"

    // let renderBox className (x: int) (y: int) (w: int) (h: int) =
    //     sprintf "M%d,%d h%dv%dh%dz" x y w h -w

    // let renderDoor (room: Room) = 
    //     let x, y = room.Position
    //     let x, y = x*ROOM_SIZE + (x + 1)*DOOR_SIZE, y*ROOM_SIZE + (y + 1)*DOOR_SIZE
    //     [|
    //         for exit in room.Exits do
    //             match exit with
    //             | South -> yield renderBox "door" x (y + ROOM_SIZE) ROOM_SIZE DOOR_SIZE
    //             | East -> yield renderBox "door" (x + ROOM_SIZE) y DOOR_SIZE ROOM_SIZE
    //             | _ -> ()
    //     |] |> Array.join " " 

    // let renderRoom (room: Room) = 
    //     let x, y = room.Position
    //     let x, y = x*ROOM_SIZE + (x + 1)*DOOR_SIZE, y*ROOM_SIZE + (y + 1)*DOOR_SIZE
    //     renderBox "room" x y ROOM_SIZE ROOM_SIZE
    
    // let mergePaths className renderObject (rooms: Room[]) =
    //     rooms 
    //     |> Array.map renderObject
    //     |> Array.join " "
    //     |> fun p -> Svg.svgElem "path" [klass className; attr "d" p] []
 
    // let renderWorld (world: World) = [
    //     let rooms = world.Rooms |> Array.collect id |> Array.filter (fun r -> r.Visited)
    //     yield rooms |> mergePaths "room" renderRoom
    //     yield rooms |> mergePaths "door" renderDoor
    // ]

    // let button label action =
    //     Tags.button 
    //         [ itype "button"; klass "btn"; onMouseClick (fun _ -> action) ] 
    //         [ text label ]

    let startAnimation canvas =
        let mutable cancel = (fun () -> ())
        let mutable state = newWorld WORLD_WIDTH WORLD_HEIGHT |> Game.buildMaze 0 0 |> Enumerator.create

        canvas ? width("1111") |> ignore

        cancel <- Time.interval (1.0 / 60.0) (fun _ -> 
            printfn "cancelling"
            cancel ()
            canvas ? clearCanvas () |> ignore
            canvas ? drawRect(
                newmap ["fillstyle" ==> "#fff"; "x" ==> 10; "y" ==> 10; "width" ==> 100; "height" ==> 100]
            ) |> ignore
        )

        printfn "returning"
        cancel

    let main () =
        printfn "version 1"

        (importDefault<obj> "jcanvas") $ (jq, Browser.window) |> ignore

        let w, h = WORLD_WIDTH, WORLD_WIDTH
        let mazeWidth = w*ROOM_SIZE + (w + 1)*DOOR_SIZE 
        let mazeHeight = h*ROOM_SIZE + (h + 1)*DOOR_SIZE

        let mutable cancel = (fun () -> ()) // empty function
        let canvas = jq $ ("#canvas")
        let button = jq $ ("#restart")  

        canvas ? width(mazeWidth) ? height(mazeHeight) |> ignore
        canvas ? clearCanvas () |> ignore

        button ? click(fun _ ->
            cancel ()
            cancel <- startAnimation canvas
        ) |> ignore
