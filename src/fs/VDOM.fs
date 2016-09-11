namespace Daedalus

open Fable.Helpers.Virtualdom
open Fable.Helpers.Virtualdom.App
open Fable.Helpers.Virtualdom.Html

open Daedalus.Js

module VDOM = 
    let elem = Tags.elem
    let attr = Attributes.attribute
    let button = Tags.button

    let deferEvent seconds event push = 
        Time.defer seconds (fun () -> push event) |> ignore