module CatCoding.TypedMessage

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.VSCode
open Fable.Import.VSCode.Vscode

open MessageTypes

let mutable currentPanel: WebviewPanel option = None

let getNonce () =
    let possible =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
            .ToCharArray()

    let gen = System.Random()

    Seq.init 32 (fun _ ->
        possible
        |> (possible.Length - 1 |> gen.Next |> Array.item))
    |> String.Concat

let getWebviewContent (webview: Webview) extensionUri =

    /// Get path to resource on disk
    let scriptPathOnDisk =
        vscode.Uri.joinPath (extensionUri, "dist", "panelcontentmain.js")

    /// Get the special URI to use with the webview
    let scriptUri =
        scriptPathOnDisk.``with`` !!{| scheme = "vscode-resource" |}
        |> string

    let nonce = getNonce ()

    let cspContent =
        $"default-src 'none'; style-src {webview.cspSource}; img-src {webview.cspSource} https:; script-src 'nonce-{nonce}';"

    /// https://stackoverflow.com/a/43702240/16630205
    let esModuleExports = @"var exports = {""__esModule"": true};"

    html
        $"""
    <!DOCTYPE html>
    <html lang="en">
        <head>
            <meta charset="UTF-8">
            <meta http-equiv="Content-Security-Policy"
                  content="{cspContent}">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Cat Coding</title>
        </head>
        <body>
            <img src="https://media.giphy.com/media/JIX9t2j0ZTN9S/giphy.gif" width="300" />
            <h1 id="lines-of-code-counter">0</h1>
            <script nonce="{nonce}">{esModuleExports}</script>
            <script nonce="{nonce}" src="{scriptUri}"></script>
        </body>
    </html>
    """

let start extensionUri addDisposable _ =
    match currentPanel with
    | Some panel -> panel.reveal (ViewColumn.Beside, true)
    | None ->
        /// new webViewPanel
        let panel =
            window.createWebviewPanel (
                "catCoding",
                "Cat Coding",
                !^ViewColumn.Active,
                !!{| enableScripts = true
                     localResourceRoots = [| vscode.Uri.joinPath (extensionUri, "dist") |] |}
            )

        let cts = new System.Threading.CancellationTokenSource()


        panel.webview.html <- getWebviewContent panel.webview extensionUri

        // Handle messages from the webview
        panel.webview.onDidReceiveMessage.Invoke (fun e ->
            let msg: IMessage = !!e

            match msg.command with
            | Alert -> window.showErrorMessage (msg.text) |> ignore

            None)
        |> addDisposable

        panel.onDidDispose.Invoke (fun _ ->
            // When the panel is closed, cancel updateWebView loop.
            cts.Cancel()

            window.showInformationMessage "Cat Coding closed."
            |> ignore

            currentPanel <- None

            None)
        |> addDisposable

        currentPanel <- Some panel

    None
