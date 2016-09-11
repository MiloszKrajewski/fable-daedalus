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
    type Model = {
        World: World
        Engine: Enumerator<World> option
    }

    type Event = | Restart | Tick

    let newModel () =
        let world = newWorld 10 10
        { World = world; Engine = world |> Game.buildMaze 0 0 |> Enumerator.create }

    let nextModel (model: Model) = 
        match model.Engine with
        | None -> model
        | Some engine -> 
            let world = engine |> Enumerator.value
            { model with World = world; Engine = engine |> Enumerator.next }

    let withTick (model: Model) =
        match model.Engine with
        | None -> model, []
        | Some _ -> model, [deferEvent 0.1 Tick]

    let update model event =
        match event with
        | Restart -> newModel ()
        | Tick -> model |> nextModel
        |> withTick

    let renderRoom (room: Room) = 
        elem "x" [] []

    let renderWorld (world: World) = [
        printfn "renderWorld"
        let w, h = world.Size
        for y = 0 to h - 1 do
            for x = 0 to w - 1 do
                let room = world.Rooms.Item(x, y)
                if room.Visited then
                    yield renderRoom room
    ]

    // w * rw + (w+1)*dw

    let button label action =
        Tags.button 
            [ attr "type" "button"; attr "class" "btn"; onMouseClick (fun _ -> action) ] 
            [ text label ]

    let view model = 
        div [] [
            div [attr "class" "container"; attr "style" "text-align: center; margin-top: 16px"] [
                button "Restart" Restart
            ]
            div [attr "class" "container"; attr "style" "text-align: center"] [
                svg [attr "width" "400"; attr "height" "400"] [
                    Svg.rect [attr "x" "0"; attr "y" "0"; attr "width" "100"; attr "height" "100"] []
                ] 
            ]
        ]

    let main () =
        printfn "Main.main()"

        createApp (newModel ()) view update 
        |> withStartNodeSelector "#main" 
        |> start renderer
