# OmegaSudoku

OmegaSudoku is a C# project for solving Sudoku puzzles. It supports solving 9x9 boards. 
The project is modular, with separate Core logic, an application interface, and automated tests.

## Features
- Solve standard 9x9 Sudoku boards quickly and efficiently (under 1 second).
- Designed to be easily extended to larger boards (e.g., 16x16, 25x25) with minimal changes.
- Using multiple advanced efficient solving algorithms including:
  - Bitmask-based representation for fast calculations of possible numbers.
  - Minimum Remaining Values (MRV) heuristic to choose the next cell intelligently.
  - Forward-checking/backtracking algorithm with stack-based state restoration.
- Automated xUnit tests to verify solver correctness.
- Timed solver to measure performance (in milliseconds).

## Performance
OmegaSudoku is optimized for speed and can solve standard 9x9 Sudoku boards very efficiently. Typical timings (on modern hardware) are:

| Board Type      | Time (ms) |
|-----------------|-----------|
| Easy            | < 1       |
| Medium          | 1–10      |
| Hard            | 10–150    |
| Very Complex    | < 1000    |

- All automated tests include timing checks to ensure the solver meets the under-1-second requirement for 9x9 boards.

> Note: Actual performance may vary depending on CPU, build configuration (Debug vs Release), and background load. For guaranteed timing measurements, run in Release mode.

## How it Solves
OmegaSudoku's Solver uses a combination of **backtracking**, **forward-checking**, and **heuristics** to efficiently solve Sudoku boards:

1. **Bitmask Representation**  
   Each row, column, and box tracks which numbers are already used using bitmasks. 
   This allows for extremely fast checks of valid numbers for each cell.

2. **Possibilities Table**  
   For each empty cell, the solver calculates a bitmask of all valid numbers based on the current board state.

3. **Heuristic: Minimum Remaining Values (MRV)**  
   The next cell to fill is chosen based on the fewest possible numbers. This greatly reduces the search space.

4. **Tie-breakers**  
   If multiple cells have the same number of possibilities:
    - Prefer the cell with the most empty neighboring cells.
    - If still tied, prefer the cell with the highest **connectivity** (a precomputed measure of constraint influence).

5. **Forward Checking / Propagation**  
   When a number is placed in a cell, all affected neighbors (row, column, and box) are updated to remove that number from their possibilities.  
   If a neighbor is reduced to a single option, it is automatically placed (propagated) recursively.

6. **Backtracking Stack**  
   Changes to the board and possibilities are tracked in a stack. If a dead-end is reached, the solver backtracks efficiently by restoring the previous state.

7. **Timed Solve**  
   The `TimedSolve` method measures the execution time for solving a board, allowing performance monitoring.

This combination of **MRV heuristic**, **forward-checking**, and **efficient bit operations** ensures that even hard Sudoku puzzles can be solved quickly, in under 1 second for 9x9 boards.

## Project Structure
- **Core** – contains `Board.cs` and `Solver.cs` and more.
- **App** – simple interface to run and solve Sudoku boards.
- **Tests** – automated tests ensuring the Solver works correctly and efficiently, including reading boards from files.
- 
## Example Usage
```csharp
var (solved, ms) = Solver.TimedSolve(board);
```
- solved - boolean for solved or not
- ms - the time it took to solve the puzzle (or to detect if its unsolvable)

## Installation

1. Clone the repository:
```bash
git clone https://github.com/RoniOuzan/OmegaSudoku.git
```
2. Open the solution OmegaSudoku.sln in Rider, Visual Studio, or any .NET IDE.
3. Restore packages (xUnit for tests is included).
4. Build the solution.
5. Run OmegaSudoku.App to solve boards interactively 
   or run OmegaSudoku.Tests to execute automated tests.

## Requirements
- .NET 8.0
- xUnit (for running tests)


### Author - Roni Ouzan