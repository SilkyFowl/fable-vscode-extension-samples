module Main

open Fable.Core.JsInterop
open Fable.Import.VSCode.Vscode

let activate (context: ExtensionContext) =
    let addDisposable (d: Disposable) = context.subscriptions.Add !!d

    commands.registerCommand ("fable.catCoding.start", CatCoding.start addDisposable)
    |> addDisposable

    commands.registerCommand ("fable.catCodingMBP.start", CatCoding.MBP.start addDisposable)
    |> addDisposable

    commands.registerCommand ("fable.CatCodingWithViewState.start", CatCoding.WithViewState.start addDisposable)
    |> addDisposable

    commands.registerCommand ("fable.CatCodingMessagePassing.start", CatCoding.MessagePassin.start addDisposable)
    |> addDisposable

    commands.registerCommand ("fable.CatCodingMessagePassing.doRefactor", CatCoding.MessagePassin.doRefactor)
    |> addDisposable
    
    commands.registerCommand ("fable.CatCodingTypedMessage.start", CatCoding.TypedMessage.start context.extensionUri addDisposable)
    |> addDisposable