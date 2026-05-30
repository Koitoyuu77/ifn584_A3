Design Process:
- Requirements analysis and commonality identifications: shared elements: players, grids layout, turn-taking, undo/redo vs. specific elements: gravity placement, misere wincheck, etc. 
- Domain modelling and abstraction, e.g. single-board & multi-board
- Decoupling and refactoring with design patterns:
    + Separate placement strategies -> Standard vs Gravity
    + WinCheck is handled separately from the Game class
    + Undo and Redo are moved into the Command pattern, instead of the game loop.
- Integration and verification: plug in AI and saveloadmanager
---
Tasks allocation:
- Mem 1: Handling the core elements (board, cell, piece, move), interface template and abstraction: Istrategy, command, and games
- Mem 2: Handling detailed win strategies, and placement strategies
- Mem 3: Implement each specific games by overriding methods from the games class, gamefactory and players
- Mem 4: Handle the UI, In-game commands handling, and GameController
- Mem 5: Handle the SaveLoadManager and integrate all components into the game enginer (Program.cs) 
---
Group 7:
- Tri Tin Vo             - 12501034
- Ruiming Zhu            - 12338621
- Weiyun Hsu             - 11881313
- Keerati Kongsricharoen - 11899557
- Shenqi Zhang           - 9628771

---
Features List:

- [ ] Game initialisation
    + [ ] Game selection
    + [ ] Edge case: Invalid selection
    + [ ] Mode selection
    + [ ] Board configuration (Both default and manual setup)
        - [ ] Fixed board: Notakto
    + [ ] Game rendering
        - [ ] Board, Separator, Col-Row Index
        - [ ] Extra Message
        - [ ] Win Condition Helper
        - [ ] Move Hint
        - [ ] Player's turn

- [ ] Robust game handling
    + [ ] TicTacToe
        + [ ] Board configuration: Yes, default = 3x3
        + [ ] Symbols: X, O
        + [ ] WinStrategy: Line(n)
        + [ ] Placement: Standard. (row, col)
        + [ ] Edge case: Out of bound
        + [ ] Board is FULL = DRAW
    + [ ] Numerical TicTacToe
        + [ ] Board configuration: Yes, default = 3x3
        + [ ] Symbols: Odd, Even numbers
        + [ ] WinStrategy: SumLine(n(n^2 + 1)/2)
        + [ ] Placement: Standard. (row, col, number)
        + [ ] Edge case: Invalid Piece
    + [ ] Notakto
        + [ ] Board configuration: No, 3 boards 3x3 
        + [ ] Symbols: X only
        + [ ] WinStrategy: Misere()
        + [ ] Placement: Standard. (board, row, col)
    + [ ] Gomoku
        + [ ] Board configuration: Yes, default = 5
        + [ ] Symbols: X, O
        + [ ] WinStrategy: Line(5)
        + [ ] Placement: Standard. (row, col)
    + [ ] Connect Four
        + [ ] Board configuration: No (6x7)
        + [ ] Symbols: X, O
        + [ ] WinStrategy: Line(4)
        + [ ] Placement: Gravity. (col)
        + [ ] Edge case: Column is full

- [ ] In-game commands
    + [ ] Save
    + [ ] Undo
    + [ ] Redo
    + [ ] Help
    + [ ] Quit

- [ ] Save Load Manager
    + [ ] Save Game screen
        + [ ] as .json
        + [ ] as .txt
        + [ ] Cancel command handling
    + [ ] Load Game screen

- [ ] Edge case handling
    + [ ] Invalid selection input

- [ ] Responsive Console UI, smooth UX
    + [ ] Smooth board scaling
    + [ ] Handle bottom-up, left-to-right board grid
    + [ ] Safely clear screen
    + [ ] End game screen