module MyContainerCSP

open Lit

[<LitElement("my-element")>]
let MyElement() =

    let _ = LitElement.init ignore

    let counter, setCounter =
        MyContainer.vscode.getState ()
        |> Option.defaultValue 0
        |> Hook.useState

    let setCounter = setCounter >> (fun _ -> MyContainer.vscode.setState counter)

    let imgSrc, setImgSrc = MyContainer.getGiphy "5Zesu5VPNGJlm" |> Hook.useState

    html
        $"""
        <h2>Counter</h2>
        <h3><vscode-tag>{counter}</vscode-tag></h3>
        <h3>
            <vscode-button @click={fun _ -> setCounter (counter + 1)}>+</vscode-button>
            <vscode-button @click={fun _ -> setCounter (counter - 1)}>-</vscode-button>
        </h3>
        <h3>
            <vscode-button @click={fun _ -> setCounter 0}>Reset!!</vscode-button>
        </h3>
        <vscode-divider></vscode-divider>
        <h2>Radio Group</h2>
        <vscode-radio-group @change={Ev(fun e -> MyContainer.getGiphy e.target.Value |> setImgSrc)}>
            <label slot="label">Select Cats</label>
            {MyContainer.cats |> Lit.mapUnique (fun x -> x.value) MyContainer.radioCat}
        </vscode-radio-group>
        <img src={imgSrc} width="300" />
        """