# Fable Cat Coding - WebView Sample

## Reference

- [Cat Coding — A Webview API Sample](https://github.com/microsoft/vscode-extension-samples/tree/main/webview-sample)
- [Webview API](https://code.visualstudio.com/api/extension-guides/webview)
- [Fable VS Code Extension Sample](https://github.com/fable-compiler/fable-vscode-extension)
- [Ionide-VSCode: FSharp](https://github.com/ionide/ionide-vscode-fsharp)...How to use Paket

```sh
pnpm install
paket start
```

if use [Highlight HTML/SQL templates in F#](https://marketplace.visualstudio.com/items?itemName=alfonsogarciacaro.vscode-template-fsharp-highlight)...

![image](https://user-images.githubusercontent.com/16532218/143421409-256e1e71-59dd-4817-88fc-289be24ccd53.png)

## ToDo

- [ ] Serialization...`vscode.window.registerWebviewPanelSerializer` not work. How do I make it work?
- [ ] Use Flamework... [Lit can use WebView content.](https://rodydavis.com/posts/lit-vscode-extension/) It means that Fable.Lit, as well as Elmish and Feliz, could be used.
