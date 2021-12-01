module PanelContentMain

open Fable.Core
open Fable.Core.JS

open MessageTypes

[<AllowNullLiteral>]
type VSCode =
    abstract postMessage: message: obj -> unit
    abstract getState: unit -> obj option
    abstract setState: state: obj option -> unit

[<Emit("acquireVsCodeApi()")>]
let acquireVsCodeApi () : VSCode = jsNative

let vscode = acquireVsCodeApi ()

let window = Browser.Dom.window
let counter = window.document.getElementById "lines-of-code-counter"

let mutable count = 0

let inline update _ =
    count <- count + 1
    counter.textContent <- string count

    if Math.random () < float count * 0.001 then
        createMessage Alert $"🐛 on line {count}..."
        |> vscode.postMessage

setInterval update 100 |> ignore
