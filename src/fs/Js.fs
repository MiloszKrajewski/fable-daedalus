namespace Playground

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.Browser
open System

module Js =
    let inline empty () = createEmpty ()
    let inline newmap fields = createObj fields
    let inline newobj o args = createNew o args

    module Option = 
        let def v o = defaultArg o v
        let alt a o = match o with | None -> a | _ -> o

    module Time = 
        let now () = DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds
        let defer seconds (action: unit -> unit) = 
            let disposable = window.setTimeout (action, seconds / 1000.0)
            { new IDisposable with member x.Dispose () = window.clearTimeout(disposable) }
        let interval seconds (action: unit -> unit) =
            let disposable = window.setInterval (action, seconds / 1000.0)
            { new IDisposable with member x.Dispose () = window.clearInterval(disposable) }
