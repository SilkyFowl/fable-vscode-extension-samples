module Extension

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.VSCode
open Fable.Import.VSCode.Vscode

[<AutoOpen>]
module Helpler =
    let inline tap ([<InlineIfLambda>] sideEffect) x =
        sideEffect x
        x

    let html = sprintf
    let js = sprintf

    type Promise.PromiseBuilder with
        member _.Bind(t: Thenable<'T>, f: 'T -> JS.Promise<'R>) : JS.Promise<'R> = promise.Bind(!!t, f)

    type Control.AsyncBuilder with
        member _.Bind(t: Thenable<'T>, f: 'T -> Async<'R>) : Async<'R> = async.Bind(Async.AwaitPromise !!t, f)

module Panel =

    let viewType = "webview-lit-sample"
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


    let getWebviewContent (webview: Webview) extensionUri =
        let getRsourceUri pathSegments =

            vscode.Uri.joinPath (extensionUri, pathSegments)
            |> webview.asWebviewUri
            |> string

        let scriptUri = getRsourceUri [| "dist"; "maincsp.js" |]

        let toolkitUri = getRsourceUri [| "dist"; "toolkit.js" |]

        let nonce = getNonce ()

        /// https://stackoverflow.com/a/43702240/16630205
        let esModuleExports = js """var exports = {"__esModule": true};"""

        html
            $"""
        <!DOCTYPE html>
        <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta property="csp-nonce" content={nonce}>
                <meta http-equiv="Content-Security-Policy"
                      content="default-src 'none';
                               img-src https:;
                               style-src 'nonce-{nonce}';
                               script-src 'nonce-{nonce}';">
                <meta name="viewport" content="width=device-width,initial-scale=1,shrink-to-fit=no">
                <script nonce="{nonce}">{esModuleExports}</script>
                <title>{viewType}</title>
            </head>
            <my-element></my-element>
            <script nonce="{nonce}" type="module" src="{toolkitUri}"></script>
            <script nonce="{nonce}" type="module" src="{scriptUri}"></script>
        </html>
        """

    let initWebviewPanel extensionUri (panel: WebviewPanel) =
        panel.webview.html <- getWebviewContent panel.webview extensionUri

        panel.onDidDispose.Invoke (fun _ ->
            window.showInformationMessage "Lit Cat Coding closed."
            |> ignore

            dispose ()

            currentPanel <- None

            None)
        |> disposables.Push

    let start extensionUri _ =

        match currentPanel with
        | Some panel -> panel.reveal (ViewColumn.Beside, true)
        | None ->
            currentPanel <-
                window.createWebviewPanel (
                    viewType,
                    "Lit Cat Coding",
                    !^ViewColumn.Active,
                    !!{| enableScripts = true
                         retainContextWhenHidden = true
                         localResourceRoots = [| vscode.Uri.joinPath (extensionUri, "dist") |] |}
                )
                |> tap (initWebviewPanel extensionUri)
                |> Some

        None

    let serializer extensionUri : WebviewPanelSerializer =
        !!{| deserializeWebviewPanel =
            fun (panel: WebviewPanel) (state: obj) ->
                async {
                    currentPanel <-
                        panel
                        |> tap (initWebviewPanel extensionUri)
                        |> Some

                    panel.reveal (ViewColumn.Active, false)

                    window.showInformationMessage $"Lit Cat Coding deserialized. state: {state}"
                    |> ignore
                }
                |> Async.StartAsPromise |}


let activate (context: ExtensionContext) =
    let addDisposable (d: Disposable) = context.subscriptions.Add !!d

    commands.registerCommand ("webview-lit-sample.satrt", Panel.start context.extensionUri)
    |> addDisposable

    window.registerWebviewPanelSerializer (Panel.viewType, Panel.serializer context.extensionUri)
