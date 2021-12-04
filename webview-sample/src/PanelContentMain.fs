module PanelContentMain

open Fable.Core
open Fable.Core.JS

open MessageTypes
open Browser
open Browser.Types

[<AllowNullLiteral>]
type VSCode =
    abstract postMessage: message: 't -> unit
    abstract getState: unit -> 't option
    abstract setState: state: 't -> unit

[<Emit("acquireVsCodeApi()")>]
let acquireVsCodeApi () : VSCode = jsNative

let vscode = acquireVsCodeApi ()

let counter = document.getElementById "lines-of-code-counter"

// Check if we have an old state to restore from
let mutable count = vscode.getState () |> Option.defaultValue 0


window.addEventListener (
    "message",
    fun e ->
        match (e :?> MessageEvent).data with
        | Message Refactor msg ->
            count <- count / 2

            createMessage Alert $"Refactor: {msg.text}"
            |> vscode.postMessage
        | _ -> ()
)

let inline update _ =
    count <- count + 1
    counter.textContent <- string count
    // Update saved state
    vscode.setState count

    if Math.random () < float count * 0.001 then
        createMessage Alert $"🐛 on line {count}..."
        |> vscode.postMessage

setInterval update 100 |> ignore
