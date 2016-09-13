namespace Daedalus

open System
open System.Collections.Generic

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

open Daedalus.Js
open Daedalus.Game

module Main =
    let [<Literal>] WORLD_WIDTH = 5
    let [<Literal>] WORLD_HEIGHT = 5
    let [<Literal>] ROOM_SIZE = 50
    let [<Literal>] DOOR_SIZE = 5

    let [<Literal>] ROOM_COLOR = "#fff"
    let [<Literal>] DOOR_COLOR = "#eee"

    let jq = importDefault<obj> "jquery"

    let startAnimation canvas =
        let world = createWorld WORLD_WIDTH WORLD_HEIGHT 
        let mutable action = world |> Game.buildMaze 0 0 |> Enumerator.create

        let inline pixelxy (x, y) = x*ROOM_SIZE + (x + 1)*DOOR_SIZE, y*ROOM_SIZE + (y + 1)*DOOR_SIZE 

        let clearWorld () = canvas ? clearCanvas () |> ignore

        let drawBox (color: string) (x: int) (y: int) (w: int) (h: int) =
            let rect = 
                newmap [ 
                    "fillStyle" ==> color; 
                    "x" ==> x; "y" ==> y; "width" ==> w; "height" ==> h; 
                    "fromCenter" ==> false 
                ]
            canvas ? drawRect(rect) |> ignore

        let drawRoom location =
            let x, y = pixelxy location   
            drawBox ROOM_COLOR x y ROOM_SIZE ROOM_SIZE
            
        let drawDoor location direction =
            let x, y = pixelxy location
            match direction with
            | North -> drawBox DOOR_COLOR x (y - DOOR_SIZE) ROOM_SIZE DOOR_SIZE
            | South -> drawBox DOOR_COLOR x (y + ROOM_SIZE) ROOM_SIZE DOOR_SIZE
            | East -> drawBox DOOR_COLOR (x + ROOM_SIZE) y DOOR_SIZE ROOM_SIZE
            | West -> drawBox DOOR_COLOR (x - DOOR_SIZE) y DOOR_SIZE ROOM_SIZE

        clearWorld ()

        let mutable cancel = id
        cancel <- Time.interval (1.0 / 60.0) (fun _ ->
            match action |> Option.map Enumerator.value with
            | None -> cancel ()
            | Some (InitAt location) -> 
                drawRoom location
            | Some (MoveTo (_, direction, location)) -> 
                drawRoom location
                drawDoor location (opposite direction)
            action <- action |> Option.bind Enumerator.next 
        )

        cancel

    let main () =
        printfn "version 3"

        (importDefault<obj> "jcanvas") $ (jq, Browser.window) |> ignore

        let w, h = WORLD_WIDTH, WORLD_HEIGHT
        let mazeWidth = w*ROOM_SIZE + (w + 1)*DOOR_SIZE 
        let mazeHeight = h*ROOM_SIZE + (h + 1)*DOOR_SIZE

        let mutable cancel = (fun () -> ()) // empty function
        let canvas = jq $ ("#canvas")
        let button = jq $ ("#restart")  

        canvas 
            ? attr("width", mazeWidth) ? attr("height", mazeHeight) 
            ? attr("viewbox", sprintf "0 0 %d %d" mazeWidth mazeHeight) 
            ? attr("viewport", sprintf "0 0 %d %d" mazeWidth mazeHeight) 
            |> ignore

        button ? click(fun _ ->
            cancel ()
            canvas ? clearCanvas () |> ignore
            cancel <- startAnimation canvas
        ) |> ignore
