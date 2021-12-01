module MessageTypes

open Fable.Core

[<StringEnum>]
type Command = | Alert

/// Interface of messages from webview
[<Erase>]
type IMessage =
    abstract command: Command with get, set
    abstract text: string with get, set

let createMessage (command: Command) (text: string) = {| command = command; text = text |}
