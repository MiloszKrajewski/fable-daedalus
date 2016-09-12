namespace Daedalus

open Fable.Helpers.Virtualdom
open Fable.Helpers.Virtualdom.App
open Fable.Helpers.Virtualdom.Html

open Daedalus.Js

module VDOM = 
    let elem = Tags.elem
    let attr = Attributes.attribute
    let button = Tags.button
    let klass = Attributes.Class
    let itype value = attr "type" value 
    let svg = Svg.svg
    let style value = attr "style" value

    let deferEvent seconds event push = 
        Time.defer seconds (fun () -> push event) |> ignore