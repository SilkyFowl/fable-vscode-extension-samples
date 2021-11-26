module CatCoding.WithViewState

open Fable.Core.JsInterop
open Fable.Import.VSCode.Vscode

let update (panel: WebviewPanel) =
    let title, cat =
        match panel.viewColumn with
        | Some ViewColumn.One -> "Coding Cat", "https://media.giphy.com/media/JIX9t2j0ZTN9S/giphy.gif"
        | Some ViewColumn.Two -> "Compiling Cat", "https://media.giphy.com/media/mlvseq9yvZhba/giphy.gif"
        | Some ViewColumn.Three -> "Testing  Cat", "https://media.giphy.com/media/3oriO0OEd9QIDdllqo/giphy.gif"
        | _ -> "Laptop Cat", "https://media.giphy.com/media/3oKIPnAiaMCws8nOsE/giphy.gif"

    panel.title <- title
    panel.webview.html <- getWebviewContent cat

let start addDisposable _ =
    /// new webViewPanel
    let panel =
        window.createWebviewPanel ("catCoding", "Cat Coding", !!ViewColumn.Active, None)

    update panel

    panel.onDidChangeViewState.Invoke (fun e ->
        update e.webviewPanel
        None)
    |> addDisposable

    panel.onDidDispose.Invoke (fun _ ->
        window.showInformationMessage "Cat Coding closed."
        |> ignore

        None)
    |> addDisposable

    None
