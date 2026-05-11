# ULTIMATE BOARD GAMES

Apply OOP in designing an ultimate board game engine that contain 5 games as below

## Games:

1. Tic-Tac-Toe
2. Numerical Tic-Tac-Toe
3. Notakto
4. Gomoku
5. Connect Four

## CRC Details
- [Class Responsibility Collaborator Cards](https://docs.google.com/document/d/1Ausfhwz-O5GsTJDHaYaAhCb8NcTQniXfhMJwxKNsHM0/edit?usp=sharing)

## Program Structure:

```text
BoardGames
|--- Program.cs
|--- Core/
|    |--- Board.cs
|    |--- Cell.cs
|    |--- GameMode.cs
|    |--- GameType.cs
|    |--- Move.cs
|    |--- Piece.cs
|--- Players/
|    |--- Player.cs
|    |--- HumanPlayer.cs
|    |--- ComputerPlayer.cs
|--- Games/
|    |--- Game.cs
|    |--- TicTacToeGame.cs
|    |--- NumericalTicTacToeGame.cs
|    |--- NotaktoGame.cs
|    |--- GomokuGame.cs
|    |--- ConnectFour.cs
|--- PlacementStrategies/
|    |--- IPlacementStrategy.cs
|    |--- StandardPlacement.cs
|    |--- GravityPlacement.cs
|    |--- PlacementResult.cs
|    |--- IAIStrategy.cs
|    |--- SimpleAI.cs
|    |--- BoardDirections.cs
|--- WinStrategies/
|    |--- IWinStrategy.cs
|    |--- LineWinStrategy.cs
|    |--- SumLineWinStrategy.cs
|    |--- MisereLineWinStrategy.cs
|    |--- WinResult.cs
|--- UI
|    |--- Command.cs
|    |--- CommandType.cs
|    |--- ConsoleUI.cs
|    |--- GameController.cs
|    |--- InputHandler.cs
|--- Commands/
|    |--- IMoveCommand.cs
|    |--- MoveHistory.cs
|    |--- PlaceMoveHistory.cs
|--- Factory/
|    |--- GameFactory.cs
|--- SaveLoadManager/
|    |--- GameSaveState.cs
|    |--- ISaveFormat.cs
|    |--- JsonSaveFormat.cs
|    |--- TextSaveFormat.cs
|    |--- SaveLoadManager.cs
|--- _FileSave/
|    |--- <file_name>.json
|    |--- <file_name>.txt
```