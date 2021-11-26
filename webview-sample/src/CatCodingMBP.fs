module CatCoding.MBP

open System
open Fable.Core.JsInterop
open Fable.Import.VSCode.Vscode

type Msg =
    | Start of addDisposable: (Disposable -> unit)
    | Cancel

type State =
    | Closed
    | Opened of panel: WebviewPanel * cts: Threading.CancellationTokenSource

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
