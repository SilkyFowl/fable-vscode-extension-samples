module CatCoding.TypedMessage

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.VSCode
open Fable.Import.VSCode.Vscode

open MessageTypes

let mutable currentPanel: WebviewPanel option = None

let getWebviewContent extensionUri =

    /// Get path to resource on disk
    let scriptPathOnDisk =
        vscode.Uri.joinPath (extensionUri, "dist", "panelcontentmain.js")

    /// Get the special URI to use with the webview
    let scriptUri =
        scriptPathOnDisk.``with`` !!{| scheme = "vscode-resource" |}
        |> string

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
            <h1 id="lines-of-code-counter">0</h1>
            <!-- https://stackoverflow.com/a/43702240/16630205 -->
            <script>var exports = {"__esModule": true};</script>
            <script src="%s"></script>
        </body>
    </html>
    """
        scriptUri

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
                     // Only allow the webview to access resources in our extension's dist directory
                     localResourceRoots = [| vscode.Uri.joinPath (extensionUri, "dist") |] |}
            )

        let cts = new System.Threading.CancellationTokenSource()


        panel.webview.html <- getWebviewContent extensionUri

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
