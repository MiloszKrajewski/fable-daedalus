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

module Game =
    type Coords = int * int
    type Direction = | North | East | South | West

    type Room = 
        {
            Position: Coords
            Visited: bool
            Exits: Direction list
        }

    type World = 
        {
            Size: Coords
            Rooms: Room[][]
        }

    type State = {
        Current: Coords
        World: World
        Stack: Coords list 
    }

    let newRoom x y = { 
        Position = (x, y)
        Visited = false
        Exits = []
    }

    let newWorld width height = {
        Size = (width, height)
        Rooms = Array.init height (fun y -> Array.init width (fun x -> newRoom x y))
    }

    let shift direction (x, y) = 
        match direction with
        | North -> (x, y - 1)
        | East -> (x + 1, y)
        | South -> (x, y + 1)
        | West -> (x - 1, y)

    let enter (room: Room) (state: State) = 
        

    let advance state = 
        state |> enter state.Current
