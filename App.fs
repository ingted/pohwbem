module App

open Terminal.Gui.Elmish

open Start
open NewGame
open ContinueGame


type Pages =
    | Start
    | NewGame
    | ContinueGame


type State = {
    CurrentPage : Pages
    Start : Start.State
    NewGame : NewGame.State
    ContinueGame : ContinueGame.State
    StatusMessage : string
}


type Msg =
    | StartMsg of Start.Msg
    | NewGameMsg of NewGame.Msg
    | ContinueGameMsg of ContinueGame.Msg
    | NavigateTo of Pages
    | ExitApp
    | UpdateStatus of string


let init () =
    let initStart, cmdStart = Start.init ()
    let initNewGame, cmdNewGame = NewGame.init ()
    let initContinueGame, cmdContinueGame = ContinueGame.init ()
    let initState = {
        CurrentPage = Start
        Start = initStart
        NewGame = initNewGame
        ContinueGame = initContinueGame
        StatusMessage = ""
    }

    let initCmd = Cmd.batch [
        Cmd.map StartMsg cmdStart
        Cmd.map NewGameMsg cmdNewGame
        Cmd.map ContinueGameMsg cmdContinueGame
    ]

    initState, initCmd


let update msg prevState =
    match msg with
    | StartMsg startMsg ->
        let nextStartState, nextStartCmd = Start.update startMsg prevState.Start
        let nextState = { prevState with Start = nextStartState }
        nextState, Cmd.map StartMsg nextStartCmd

    | NewGameMsg startMsg ->
        let nextNewGameState, nextNewGameCmd = NewGame.update startMsg prevState.NewGame
        let nextState = { prevState with NewGame = nextNewGameState }
        nextState, Cmd.map NewGameMsg nextNewGameCmd

    | ContinueGameMsg startMsg ->
        let nextContinueGameState, nextContinueGameCmd = ContinueGame.update startMsg prevState.ContinueGame
        let nextState = { prevState with ContinueGame = nextContinueGameState }
        nextState, Cmd.map ContinueGameMsg nextContinueGameCmd

    | NavigateTo page ->
        let nextState = { prevState with CurrentPage = page }
        nextState, Cmd.none

    | ExitApp ->
        Program.quit ()
        prevState, Cmd.none

    | UpdateStatus m ->
        { prevState with StatusMessage = m }, Cmd.none


let view (state : State) (dispatch : Msg -> unit) =
    page [
        statusBar [
            statusItem state.StatusMessage Terminal.Gui.Key.Unknown (fun () -> ())
        ]
        menuBar [
            menuBarItem "_File" [

                menuItem "_New Game..." "Start a new game" (fun () -> 
                    dispatch (UpdateStatus "New Game")
                    dispatch (NavigateTo NewGame)
                )

                menuItem "_Open Game..." "Continue a game" (fun () -> 
                    dispatch (UpdateStatus "Open Game")
                    dispatch (NavigateTo ContinueGame)
                )

                menuItem "_Close Game" "Close the game" (fun () ->
                    dispatch (UpdateStatus "Close Game")
                    dispatch (NavigateTo Start)
                )

                menuItem "_Save Game..." "Save the current game" (fun () -> 
                    dispatch (UpdateStatus "Save Game")
                )

                menuItem "E_xit" "Exit POHWBEM" (fun () -> Program.quit ())
            ]
            menuBarItem "_Edit" [
                menuItem "Undo _Z" "" (fun () -> dispatch (UpdateStatus "Undo"))
                menuItem "Redo" "" (fun () -> dispatch (UpdateStatus "Redo"))
                menuItem "Cut _X" "" (fun () -> dispatch (UpdateStatus "Cut"))
                menuItem "Copy _C" "" (fun () -> dispatch (UpdateStatus "Copy"))
                menuItem "Paste _V" "" (fun () -> dispatch (UpdateStatus "Paste"))
            ]
        ]
        window [
                Styles [
                    Pos (AbsPos 1, AbsPos 1)
                    Dim (Fill, FillMargin 1)
                ]
                Title "Play One-Hour Wargames By EMail"
        ][
            match state.CurrentPage with
            | Start ->
                yield! Start.view

            | NewGame ->
                yield! NewGame.view state.NewGame (NewGameMsg >> dispatch)

            | ContinueGame ->
                yield! ContinueGame.view state.ContinueGame (ContinueGameMsg >> dispatch)
        ]
    ]
