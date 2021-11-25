module Main

open Fable.Core.JsInterop
open Fable.Import.VSCode.Vscode

let catCoding _ =
    let panel = window.createWebviewPanel ("catCoding", "Cat Coding", !!ViewColumn.One,None)

    None

let activate (context: ExtensionContext) =
    !! commands.registerCommand("fable.catCoding.start", catCoding)
    |> context.subscriptions.Add
