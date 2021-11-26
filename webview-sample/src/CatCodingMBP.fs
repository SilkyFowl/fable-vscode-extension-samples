module CatCodingMBP

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.VSCode.Vscode

type Msg =
    | Start of addDisposable: (Disposable -> unit)
    | Cancel

type State =
    | Closed
    | Opened of panel: WebviewPanel * cts: Threading.CancellationTokenSource

/// https://github.com/alfonsogarciacaro/vscode-template-fsharp-highlight
let html = sprintf

/// webView
let getWebviewContent cat =
    html
        """
    <!DOCTYPE html>
    <html lang="en">
        <head>
            <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Cat Coding</title>
    </head>
    <body>
        <img src="%s" width="300" />
    </body>
    </html>
    """
        cat


let (|Coding|Compiling|) input =
    if input % 2 = 0 then
        Coding
    else
        Compiling

let rec updateWebView (panel: WebviewPanel) iter =
    async {
        let title, cat =
            match iter with
            | Coding -> "Coding Cat", "https://media.giphy.com/media/JIX9t2j0ZTN9S/giphy.gif"
            | Compiling -> "Compiling Cat", "https://media.giphy.com/media/mlvseq9yvZhba/giphy.gif"

        panel.title <- title
        panel.webview.html <- getWebviewContent cat

        do! Async.Sleep 5000

        return! updateWebView panel (iter + 1)
    }

let agent =
    MailboxProcessor.Start (fun inbox ->
        let rec loop state =
            async {
                let! msg = inbox.Receive()

                match msg, state with
                | Start _, Opened (panel, _) ->
                    panel.reveal (ViewColumn.Beside, true)
                    return! loop state
                | Start addDisposable, Closed ->
                    /// new webViewPanel
                    let panel =
                        window.createWebviewPanel ("catCoding", "Cat Coding", !!ViewColumn.One, None)

                    let cts = new Threading.CancellationTokenSource()
                    // Start update loop
                    Async.Start(updateWebView panel 0, cts.Token)

                    panel.onDidDispose.Invoke (fun _ ->
                        inbox.Post Cancel
                        None)
                    |> addDisposable

                    return! Opened(panel, cts) |> loop
                | Cancel, Opened (_, cts) ->
                    // When the panel is closed, cancel updateWebView loop.
                    cts.Cancel()

                    window.showInformationMessage "Cat Coding MailboxProcessor closed."
                    |> ignore

                    return! loop Closed
                | Cancel, Closed -> return! loop state
            }

        loop Closed)

let start addDisposable _ =
    Start(addDisposable) |> agent.Post

    None
