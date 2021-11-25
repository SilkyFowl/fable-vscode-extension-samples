module Main

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.VSCode.Vscode

module CatCoding =

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

            do! Async.Sleep 1000

            return! updateWebView panel (iter + 1)
        }

    let start _ =
        /// new webViewPanel
        let panel =
            window.createWebviewPanel ("catCoding", "Cat Coding", !!ViewColumn.One, None)

        // Start update loop
        updateWebView panel 0 |> Async.Start

        None

let activate (context: ExtensionContext) =
    !! commands.registerCommand("fable.catCoding.start", CatCoding.start)
    |> context.subscriptions.Add
