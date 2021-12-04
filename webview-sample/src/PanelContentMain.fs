module PanelContentMain

open Fable.Core
open Fable.Core.JS

open MessageTypes

[<AllowNullLiteral>]
type VSCode =
    abstract postMessage: message: 't -> unit
    abstract getState: unit -> 't option
    abstract setState: state: 't -> unit

[<Emit("acquireVsCodeApi()")>]
let acquireVsCodeApi () : VSCode = jsNative

let vscode = acquireVsCodeApi ()

let window = Browser.Dom.window
let counter = window.document.getElementById "lines-of-code-counter"

// Check if we have an old state to restore from
let mutable count = vscode.getState () |> Option.defaultValue 0

let inline update _ =
    count <- count + 1
    counter.textContent <- string count
    // Update saved state
    vscode.setState count

    if Math.random () < float count * 0.001 then
        createMessage Alert $"🐛 on line {count}..."
        |> vscode.postMessage

setInterval update 100 |> ignore
