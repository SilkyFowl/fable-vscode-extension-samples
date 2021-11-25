module Main

open Fable.Core.JsInterop
open Fable.Import.VSCode.Vscode

module CatCoding =
    /// https://github.com/alfonsogarciacaro/vscode-template-fsharp-highlight
    let html = sprintf

    /// webView
    let getWebviewContent () =
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
            <img src="https://media.giphy.com/media/JIX9t2j0ZTN9S/giphy.gif" width="300" />
        </body>
        </html>
        """

    let start _ =
        /// new webViewPanel
        let panel =
            window.createWebviewPanel ("catCoding", "Cat Coding", !!ViewColumn.One, None)

        // Set HTML content
        panel.webview.html <- getWebviewContent ()

        None

let activate (context: ExtensionContext) =
    !! commands.registerCommand("fable.catCoding.start", CatCoding.start)
    |> context.subscriptions.Add
