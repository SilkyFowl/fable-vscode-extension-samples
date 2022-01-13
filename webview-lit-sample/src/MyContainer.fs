module MyContainer

open Elmish

open Lit
open Lit.Elmish
open Fable.Core
open Browser

module Util =
    [<AllowNullLiteral>]
    type VSCode =
        abstract postMessage: message: 't -> unit
        abstract getState: unit -> 's option
        abstract setState: state: 's -> unit

    [<Emit("acquireVsCodeApi()")>]
    let acquireVsCodeApi () : VSCode = jsNative

module CodingCats =
    type Cat = { value: string; name: string }

    let cats =
        [ { value = "JIX9t2j0ZTN9S"
            name = "Coding Cat" }
          { value = "mlvseq9yvZhba"
            name = "Compiling Cat" }
          { value = "3oriO0OEd9QIDdllqo"
            name = "Testing Cat" } ]

    let getGiphy src =
        $"https://media.giphy.com/media/{src}/giphy.gif"

module ElmishLit =
    type State = { counter: int; imgSrc: string }

    type Msg =
        | SetCounter of int
        | SetImgSrc of string

open CodingCats
open ElmishLit

let vscode = Util.acquireVsCodeApi ()

let init _ =
    { counter = vscode.getState () |> Option.defaultValue 0
      imgSrc = "5Zesu5VPNGJlm" },
    Cmd.none

let update msg state =
    match msg with
    | SetCounter n ->
        vscode.setState n
        { state with counter = n }, Cmd.none
    | SetImgSrc src -> { state with imgSrc = src }, Cmd.none


let view state dispatch =
    let radioCat cat =
        html $"""<vscode-radio name="cats" value="{cat.value}" >{cat.name}</vscode-radio>"""

    html
        $"""
        <h2>Counter</h2>
        <h3><vscode-tag>{state.counter}</vscode-tag></h3>
        <h3>
            <vscode-button @click={fun _ -> SetCounter(state.counter + 1) |> dispatch}>+</vscode-button>
            <vscode-button @click={fun _ -> SetCounter(state.counter - 1) |> dispatch}>-</vscode-button>
        </h3>
        <h3>
            <vscode-button @click={fun _ -> SetCounter 0 |> dispatch}>Reset!!</vscode-button>
        </h3>
        <vscode-divider></vscode-divider>
        <h2>Radio Group</h2>
        <vscode-radio-group @change={Ev(fun e -> SetImgSrc e.target.Value |> dispatch)}>
            <label slot="label">Select Cats.</label>
            {cats |> Lit.mapUnique (fun x -> x.value) radioCat}
        </vscode-radio-group>
        <img src={getGiphy state.imgSrc} width="300" />
        """

[<HookComponent>]
let MyContainer () =
    let state, dispatch = Hook.useElmish (init, update)
    view state dispatch

MyContainer()
|> Lit.render document.body
