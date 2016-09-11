namespace Daedalus

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.Browser
open System
open System.Collections.Generic

module Js =
    let inline empty () = createEmpty ()
    let inline newmap fields = createObj fields
    let inline newobj o args = createNew o args

    module Option = 
        let def v o = defaultArg o v
        let alt a o = match o with | None -> a | _ -> o
        let filter f o = match o with | Some v when f v -> o | _ -> None
    
    module Map = 
        let update key func map =
            map |> Map.add key (map |> Map.find key |> func)

    [<AutoOpen>]
    module Fx =
        let apply f v = f v; v

    type Enumerator<'item>(enumerator: IEnumerator<'item>) =
        member this.Next () = match enumerator.MoveNext () with | false -> None | _ -> Some this
        member this.Value with get () = enumerator.Current

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Enumerator =
        let next (enumerator: Enumerator<_>) = enumerator.Next()
        let create (enumerable: IEnumerable<_>) = Enumerator(enumerable.GetEnumerator()) |> next
        let value (enumerator: Enumerator<_>) = enumerator.Value

    module Time = 
        let now () = DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds
        let defer seconds (action: unit -> unit) = 
            let disposable = window.setTimeout (action, seconds / 1000.0)
            { new IDisposable with member x.Dispose () = window.clearTimeout(disposable) }
        let interval seconds (action: unit -> unit) =
            let disposable = window.setInterval (action, seconds / 1000.0)
            { new IDisposable with member x.Dispose () = window.clearInterval(disposable) }
