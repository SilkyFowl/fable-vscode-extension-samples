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

[<LitElement("my-container")>]
let Counter () =
    // This call is obligatory to initialize the web component
    let _, props =
        let v = vscode.getState () |> Option.defaultValue 0
        LitElement.init (fun init -> init.props <- {| initial = Prop.Of(defaultValue = v) |})

    let counter, setCounter = Hook.useState props.initial.Value
    let setCounter = setCounter >> (fun _ -> vscode.setState counter)

    html
        $"""
        <img src="https://media.giphy.com/media/JIX9t2j0ZTN9S/giphy.gif" width="300" />
        <article>
            <p>{counter}</p>
            <button @click={fun _ -> setCounter (counter + 1)}>+</button>
            <button @click={fun _ -> setCounter (counter - 1)}>-</button>
        </article>
        """
