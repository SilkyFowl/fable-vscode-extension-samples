module CatCoding.MessagePassin

open Fable.Core.JsInterop
open Fable.Import.VSCode.Vscode

let mutable currentPanel: WebviewPanel option = None

let getWebviewContent _ =
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

            <script>
                const counter = document.getElementById('lines-of-code-counter');

                let count = 0;
                setInterval(() => {
                    counter.textContent = count++;
                }, 100);

                // Handle the message inside the webview
                window.addEventListener('message', event => {

                    const message = event.data; // The JSON data our extension sent

                    switch (message.command) {
                        case 'refactor':
                            count = Math.ceil(count * 0.5);
                            counter.textContent = count;
                            break;
                    }
                });
            </script>
        </body>
    </html>
    """

let start addDisposable _ =
    currentPanel <-
        currentPanel
        |> Option.orElseWith (fun _ ->
            /// new webViewPanel
            let panel =
                window.createWebviewPanel (
                    "catCoding",
                    "Cat Coding",
                    !!ViewColumn.One,
                    createObj [ "enableScripts" ==> true ]
                )

            let cts = new System.Threading.CancellationTokenSource()


            panel.webview.html <- getWebviewContent ()

            panel.onDidDispose.Invoke (fun _ ->
                // When the panel is closed, cancel updateWebView loop.
                cts.Cancel()

                window.showInformationMessage "Cat Coding closed."
                |> ignore

                None)
            |> addDisposable

            Some panel)

    None

let doRefactor _ =
    currentPanel
    |> Option.iter (fun panel ->
        panel.webview.postMessage (createObj [ "command" ==> "refactor" ] |> Some)
        |> ignore)
    None
