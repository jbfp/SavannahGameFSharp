open System
open System.Threading

type Gender =
| Male = 0
| Female = 1

type Animal =
| Lion of Gender : Gender * Weight : double
| Rabbit of Gender : Gender * Weight : double

type Grass = { IsAlive : bool }
type Field = { grass : Grass; animal : Animal option}
type World = { Savannah : Grass [,]; Animals : Animal option [,] }
type State = { World : World; Seed : int }

let getGrass (random : unit -> double) =
    let k = random()
    let isAlive = k < 0.25
    { Grass.IsAlive = isAlive }

let getAnimal (random : int -> int -> int) =
    let gender : Gender = enum (random 0 2)
    let k = random 0 101    
    if k < 5 then Some(Lion(Gender.Male, 10.0))
    elif k < 15 then Some(Rabbit(Gender.Female, 2.5))
    else None

let build seed =
    let rnd = new Random(seed)
    let savannah = Array2D.init 20 20 (fun x y -> getGrass rnd.NextDouble)
    let animals = Array2D.init 20 20 (fun x y-> getAnimal (fun min max -> rnd.Next(min, max)))
    let world = { World.Savannah = savannah; World.Animals = animals }
    { State.World = world; State.Seed = seed }

let drawGrass x y (g : Grass) =
    Console.SetCursorPosition(x, y)
    if g.IsAlive then
        Console.Write('#')
    else
        Console.Write('.')

let drawAnimal x y (animal : Animal option) =
    Console.SetCursorPosition(x, y)
    match animal with
    | Some a -> match a with
                | Lion(g, w) -> Console.Write('L')
                | Rabbit(g, w) -> Console.Write('R')
    | _ -> Console.Write("")

let tick state =
    let rnd = new Random(state.Seed)
    state

let rec run initial (callback : State -> unit) =
    let state = tick initial
    callback state
    run state callback

[<EntryPoint>]
let main argv = 
    let intial = build Environment.TickCount
    Console.CursorVisible <- false
    run intial (fun state ->
        state.World.Savannah |> Array2D.iteri (fun x y current -> drawGrass x y current)
        state.World.Animals |> Array2D.iteri (fun x y current -> drawAnimal x y current)
        Thread.Sleep(100))
    0    