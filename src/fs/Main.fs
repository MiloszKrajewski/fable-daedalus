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
        // let optionToString option = match option with | None -> "None" | Some value -> value |> sprintf "Some %A"
        // Some 7 |> Option.filter (fun _ -> false) |> optionToString |> printfn "%s"
        // Some 7 |> Option.filter (fun _ -> true) |> optionToString |> printfn "%s"

        printfn "Main.main()"
        // createApp model view update |> withStartNodeSelector "#main" |> start renderer
        
        Game.dfsWithVisitedSet (fun i -> if i < 10000 then [i + 1] else []) 0 
        |> Seq.toList 
        |> printfn "%A"
