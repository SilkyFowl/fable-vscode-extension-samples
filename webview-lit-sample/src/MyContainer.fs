module MyContainer

open Lit
open Fable.Core

[<AllowNullLiteral>]
type VSCode =
    abstract postMessage: message: 't -> unit
    abstract getState: unit -> 's option
    abstract setState: state: 's -> unit

[<Emit("acquireVsCodeApi()")>]
let acquireVsCodeApi () : VSCode = jsNative

let vscode = acquireVsCodeApi ()

type Cat = { value: string; name: string }

let cats =
    [ { value = "JIX9t2j0ZTN9S"
        name = "Coding Cat" }
      { value = "mlvseq9yvZhba"
        name = "Compiling Cat" }
      { value = "3oriO0OEd9QIDdllqo"
        name = "Testing Cat" } ]

let radioCat cat =
    html $"""<vscode-radio name="cats" value="{cat.value}" >{cat.name}</vscode-radio>"""

let getGiphy src =
    $"https://media.giphy.com/media/{src}/giphy.gif"

[<LitElement("my-container")>]
let MyContainer () =

    // This call is obligatory to initialize the web component
    let _, props =
        LitElement.init (fun init ->
            init.props <-
                {| initial = Prop.Of(vscode.getState () |> Option.defaultValue 0)
                   giphy = Prop.Of(getGiphy "5Zesu5VPNGJlm") |})

    let counter, setCounter = Hook.useState props.initial.Value
    let setCounter = setCounter >> (fun _ -> vscode.setState counter)

    let giphy, setGiphy = Hook.useState props.giphy.Value

    html
        $"""
        <h2>Counter</h2>
        <h3><vscode-tag>{counter}</vscode-tag></h3>
        <h3>
            <vscode-button @click={fun _ -> setCounter (counter + 1)}>+</vscode-button>
            <vscode-button @click={fun _ -> setCounter (counter - 1)}>-</vscode-button>
        </h3>
        <h3>
            <vscode-button @click={fun _ -> setCounter 0}>Reset</vscode-button>
        </h3>
        <vscode-divider></vscode-divider>
        <h2>Radio Group</h2>
        <vscode-radio-group @change={Ev(fun e -> getGiphy e.target.Value |> setGiphy)}>
            <label slot="label">Select Cats</label>
            {cats |> Lit.mapUnique (fun x -> x.value) radioCat}
        </vscode-radio-group>
        <img .src={giphy} width="300" />
        """
