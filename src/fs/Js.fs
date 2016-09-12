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

    module Random =
        [<Emit("Math.random()")>] 
        let random () = failwith "JS only"

        let randomInt min max =
            Math.Floor(random () * (double max - double min + 1.0)) + double min |> int

    module Option = 
        let def v o = defaultArg o v
        let alt a o = match o with | None -> a | _ -> o
        let filter f o = match o with | Some v when f v -> o | _ -> None

    module Array = 
        let shuffleInPlace (array: 'a[]) =
            let max = array.Length - 1 
            for i' = 1 to max do
                let i = max - i' + 1 // workaround 'downto' bug
                let j = Random.randomInt 0 i
                let t = array.[i]
                array.[i] <- array.[j]
                array.[j] <- t
        
        let shuffle array = 
            let result = array |> Array.copy
            shuffleInPlace result
            result

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
            fun () -> window.clearTimeout(disposable)
        let interval seconds (action: unit -> unit) =
            let disposable = window.setInterval (action, seconds / 1000.0)
            fun () -> window.clearInterval(disposable)
