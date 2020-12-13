module App

open Terminal.Gui.Elmish

type Model = {
    Counter : int
    StatusMessage : string
}

type Msg =
    | Increment
    | Decrement
    | Reset
    | ExitApp
    | UpdateStatus of string

let init () : Model * Cmd<Msg> =
    { Counter = 0; StatusMessage = "" }, Cmd.none

let update (msg : Msg) (model : Model) =
    match msg with
    | Increment -> { model with Counter = model.Counter + 1; StatusMessage = "Incremented" }, Cmd.none 
    | Decrement -> { model with Counter = model.Counter - 1; StatusMessage = "Decremented" }, Cmd.none
    | Reset -> { model with Counter = 0; StatusMessage = "Reset Counter" }, Cmd.none
    | ExitApp ->
        Program.quit ()
        model, Cmd.none
    | UpdateStatus m ->
        { model with StatusMessage = m }, Cmd.none

let view (model : Model) (dispatch : Msg -> unit) =
    let undoItem = menuItem "Undo" "" (fun () -> dispatch (UpdateStatus "Undo"))
    undoItem.HotKey <- System.Rune ('z')
    page [
        statusBar [
            statusItem model.StatusMessage Terminal.Gui.Key.Unknown (fun () -> ())
        ]
        menuBar [
            menuBarItem "File" [
                menuItem "_New Game..." "Start a new game" (fun () -> ())
                menuItem "_Open Game..." "Continue a game" (fun () -> ())
                menuItem "E_xit" "Exit POHWBEM" (fun () -> Program.quit ())
            ]
            menuBarItem "Edit" [
                undoItem
                menuItem "Redo" "" (fun () -> ())
                menuItem "Cut" "" (fun () -> ())
                menuItem "Copy" "" (fun () -> ())
                menuItem "Paste" "" (fun () -> ())
            ]
        ]
        
    ]
 