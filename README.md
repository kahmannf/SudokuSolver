# Sudoku Solver

This simple project started because the i stumbled across [this video](https://www.youtube.com/watch?v=pezlnN4X52g) about the Phistomefel Ring.

The application can be used to explain how a sudoku can be solved  as easily as possible or with the least steeps possible.

The search for a solution for a single cell is done by executing `SolutionStrategie`s. The enum `SolutionStrategie` has a value for each strategy that exists (additionally, it has a value for `None` if a solution is requested and the cell already contains a value). The value of the strategy in the enum is sorted by complexity of the strategy (judged by my personal opinion and experience in solving Sudokus).


The recursive solving Methode allows two strategies for solving:
- `RecursiveStrategie.PreferSimpleSolution`
- `RecursiveStrategie.AllowMaxComplexity`

When searching for solutions with `PreferSimpleSolution`, the solver will retry solving all cells with a simpler `SolutionStrategy` if a solution for any cell is found. When searching with `AllowMaxComplexity`, all `SolutionStrategy`s will be used to find Solutions before before a new iteration of all cells is started.



![Solving Stragety with most simple solutions](./docs/ScreenshotSolveRecursiveSimple.png)

Example Sudoku solutions with recursive stragtegy `PreferSimpleSolution`

![Solving Stragety with least steps to solutions](./docs/ScreenshotSolveRecursiveComplex.png)

Example Sudoku solutions with recursive stragtegy `AllowMaxComplexity`


## SolutionStrategies

There are currently 10 `SolutionStrategies` (excluding `None`):

- CombinedRemainingNumber
- SimpleBlockExclusion
- SimpleRowExclusion
- SimpleColumnExclusion
- BlockExclusionWithContraintsR1
- RowExclusionWithContraintsR1
- ColumnExclusionWithContraintsR1
- BlockExclusionWithContraintsR2
- RowExclusionWithContraintsR2
- ColumnExclusionWithContraintsR2


These can be divided into three groups of checks, that will be performed.

- remaining numbers check
- simple exclusion check
- recursive exclusion check

Each of these is explained below.

### Remaining Numbers Check
Checks for a single cell whether only one number is missing of the combined sets of row, column and box. See example below

```
  1 2 3 4 5 6 7 8 9
--------------------
a|    5 8 9 4 3
b|  3
c|  1
d|4
e|7
f|6
g|
h|
i|

When checking with "remaining numbers check" for cell a1:

Row a:        3,4,5,8,9
Column 1:     4,6,7
Box top left: 1,3,5

Combined:     1,3,4,5,6,7,8,9

If the combined set contains 8 numbers, the missing number can be set into the cell.
in this case "2" is missing
```
