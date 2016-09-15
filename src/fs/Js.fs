namespace Daedalus

open Fable.Core
open Fable.Core.JsInterop
open System
open System.Collections.Generic

module Js =
    let inline empty () = createEmpty ()
    let inline newmap (fields: seq<string * obj>) = createObj fields
    let inline newobj o args = createNew o args

    [<AutoOpen>]
    module Fx =
        let apply f v = f v; v

    [<Emit("console.log($0)")>]
    let debug object = failwith "JS only"

    module Random =
        [<Emit("Math.random()")>] 
        let random () = failwith "JS only"

        let randomInt min max =
            Math.Floor(random () * (double max - double min + 1.0)) + double min |> int

    module Option = 
        let def v o = defaultArg o v
        let alt a o = match o with | None -> a | _ -> o

    module Array = 
        let inline swapInPlace i j (array: 'a[]) = 
            let t = array.[i]
            array.[i] <- array.[j]
            array.[j] <- t

        let shuffleInPlace (array: 'a[]) =
            for i = array.Length - 1 downto 1 do
                let j = Random.randomInt 0 i
                array |> swapInPlace i j
        
        let shuffle array = array |> Array.copy |> apply shuffleInPlace
        
        [<Emit("$1.join($0)")>]
        let join (separator: string) (array: string[]): string = failwith "JS only"

    module Map = 
        let update key func map =
            map |> Map.add key (map |> Map.find key |> func)

    type Enumerator<'item>(enumerator: IEnumerator<'item>) =
        member this.Next () = match enumerator.MoveNext () with | false -> None | _ -> Some this
        member this.Value with get () = enumerator.Current

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Enumerator =
        let next (enumerator: Enumerator<_>) = enumerator.Next()
        let create (enumerable: IEnumerable<_>) = Enumerator(enumerable.GetEnumerator()) |> next
        let value (enumerator: Enumerator<_>) = enumerator.Value

    module Time = 
        open Fable.Import.Browser

        let now () = DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds
        let defer seconds (action: unit -> unit) = 
            let disposable = window.setTimeout (action, seconds / 1000.0)
            fun () -> window.clearTimeout(disposable)
        let interval seconds (action: unit -> unit) =
            let disposable = window.setInterval (action, seconds / 1000.0)
            fun () -> window.clearInterval(disposable)
