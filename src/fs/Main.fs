namespace Daedalus

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser

open Fable.Helpers.Virtualdom
open Fable.Helpers.Virtualdom.App
open Fable.Helpers.Virtualdom.Html

open Daedalus.Js
open Daedalus.Game

module Main =
    type Model = { 
        Value: int
     }

    type Event = | Reset | Up | Down

    let elem = Tags.elem
    let attr = Attributes.attribute

    let model = { Value = -5 }

    let update model event = 
        match event with
        | Reset -> { Value = 0 }
        | Up -> { model with Value = model.Value + 1 }
        | Down -> { model with Value = model.Value - 1 }
        |> (fun m -> m, [])

    let button label action =
        Tags.button 
            [ attr "type" "button"; attr "class" "btn"; onMouseClick (fun _ -> action) ] 
            [ text label ]

    let view model = 
        div [] [ 
            text (model |> sprintf "%A")
            br []
            button "Reset" Reset
            button "Increment" Up
            button "Decrement" Down
        ]

    let main () = 
        let w = newWorld 100 100
        printfn "Main.main()"
        createApp model view update
        |> withStartNodeSelector "#main"
        |> start renderer
