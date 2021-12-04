module MessageTypes

open Fable.Core

[<StringEnum>]
type Command =
    | Alert
    | Refactor

/// Interface of messages from webview
[<Erase>]
type IMessage =
    abstract command: Command with get, set
    abstract text: string with get, set

let createMessage (command: Command) (text: string) = {| command = command; text = text |}

let (|Message|_|) (cmd: Command) (i: obj) =
    match i with
    | :? IMessage as msg when msg.command = cmd -> Some msg
    | _ -> None
