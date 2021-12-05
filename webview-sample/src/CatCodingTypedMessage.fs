module CatCoding.TypedMessage

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.VSCode
open Fable.Import.VSCode.Vscode

open MessageTypes

let viewType = "fable.CatCoding.TypedMessage"

module private Panel =
    let mutable currentPanel: WebviewPanel option = None
    let disposables = Collections.Generic.Stack<Disposable>()

    let dispose () =
        while disposables.Count > 0 do
            let d = disposables.Pop()
            d.dispose () |> ignore

    let getNonce () =
        let possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"

        let random = Random()

        Seq.init 32 (fun _ -> possible[possible.Length - 1 |> random.Next])
        |> String.Concat

    let getWebviewContent cspSource extensionUri =

        /// Get path to resource on disk
        let scriptPathOnDisk =
            vscode.Uri.joinPath (extensionUri, "dist", "panelcontentmain.js")

        /// Get the special URI to use with the webview
        let scriptUri =
            scriptPathOnDisk.``with`` !!{| scheme = "vscode-resource" |}
            |> string

        let nonce = getNonce ()

        /// https://stackoverflow.com/a/43702240/16630205
        let esModuleExports = js """var exports = {"__esModule": true};"""

        html
            $"""
        <!DOCTYPE html>
        <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta http-equiv="Content-Security-Policy"
                    content="default-src 'none';
                            style-src {cspSource};
                            img-src {cspSource} https:;
                            script-src 'nonce-{nonce}';">
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

open Panel

let start extensionUri _ =
    match currentPanel with
    | Some panel -> panel.reveal (ViewColumn.Beside, true)
    | None ->
        /// new webViewPanel
        let panel =
            window.createWebviewPanel (
                viewType,
                "Cat Coding",
                !^ViewColumn.Active,
                !!{| enableScripts = true
                     // https://code.visualstudio.com/api/extension-guides/webview#retaincontextwhenhidden
                     retainContextWhenHidden = true
                     localResourceRoots = [| vscode.Uri.joinPath (extensionUri, "dist") |] |}
            )


        panel.webview.html <- getWebviewContent panel.webview.cspSource extensionUri

        // Handle messages from the webview
        panel.webview.onDidReceiveMessage.Invoke (fun e ->
            let msg: IMessage = !!e

            match msg.command with
            | Alert -> window.showErrorMessage (msg.text) |> ignore
            | Refactor -> ()

            None)
        |> disposables.Push

        panel.onDidDispose.Invoke (fun _ ->
            window.showInformationMessage "Cat Coding closed."
            |> ignore

            dispose ()

            currentPanel <- None

            None)
        |> disposables.Push

        currentPanel <- Some panel

    None

let doRefactor _ =
    currentPanel
    |> Option.iter (fun panel ->
        createMessage Refactor "✨"
        |> panel.webview.postMessage
        |> ignore)

    None

let serializer extensionUri : WebviewPanelSerializer =
    !!{| deserializeWebviewPanel =
        fun (panel: WebviewPanel) (state: State) ->
            async {
                window.showInformationMessage $"Cat Coding deserialized. count: {state.count}"
                |> ignore

                panel.webview.html <- getWebviewContent panel.webview.cspSource extensionUri
            }
            |> Async.StartAsPromise |}
