using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Sudoku
    {
        public int?[][] Grid { get; set; }

        Dictionary<SolutionStrategie, Func<int, int, int?>> strategies = new Dictionary<SolutionStrategie,Func<int, int, int?>>();

        public Sudoku()
        {
            strategies.Add(SolutionStrategie.CombinedRemainingNumber, CombinedRemainingNumberStrategy);
            strategies.Add(SolutionStrategie.SimpleBlockExclusion, BlockExclusionFactory(CanNumberBePlacedSimple));
            strategies.Add(SolutionStrategie.SimpleRowExclusion, RowExclusionFactory(CanNumberBePlacedSimple));
            strategies.Add(SolutionStrategie.SimpleColumnExclusion, ColumnExclusionFactory(CanNumberBePlacedSimple));
            strategies.Add(SolutionStrategie.BlockExclusionWithContraintsR1, BlockExclusionFactory(CanNumberBePlacedWithConstraintsFactory(1)));
            strategies.Add(SolutionStrategie.RowExclusionWithContraintsR1, RowExclusionFactory(CanNumberBePlacedWithConstraintsFactory(1)));
            strategies.Add(SolutionStrategie.ColumnExclusionWithContraintsR1, ColumnExclusionFactory(CanNumberBePlacedWithConstraintsFactory(1)));
            strategies.Add(SolutionStrategie.BlockExclusionWithContraintsR2, BlockExclusionFactory(CanNumberBePlacedWithConstraintsFactory(2)));
            strategies.Add(SolutionStrategie.RowExclusionWithContraintsR2, RowExclusionFactory(CanNumberBePlacedWithConstraintsFactory(2)));
            strategies.Add(SolutionStrategie.ColumnExclusionWithContraintsR2, ColumnExclusionFactory(CanNumberBePlacedWithConstraintsFactory(2)));

            Grid = new int?[9][];

            for (int x = 0; x < 9; x++)
            {
                Grid[x] = new int?[9];
            }
        }

        public int?[] Row(int y)
        {
            return Grid.Select(row => row[y]).ToArray();
        }
        
        public int?[] Column(int x)
        {
            return Grid[x];
        }

        public Block Block(int xBlock, int yBlock)
        {
            int?[][] result = new int?[3][];

            var xOffset = xBlock * 3;
            var yOffset = yBlock * 3;

            for (var x = 0; x < 3; x++)
            {
                result[x] = new int?[3];
                for (var y = 0; y < 3; y++)
                {
                    result[x][y] = Grid[x + xOffset][y + yOffset];
                }
            }

            return new Block(result, xBlock, yBlock);
        }

        public void Parse(string sudoku)
        {
            var lines = sudoku.Split("\n");

            if(lines.Length != 9)
            {
                throw new ArgumentException("Cannot parse sudoku: " + sudoku);
            }

            for (int y = 0; y < 9; y++)
            {
                var line = lines[y];

                var cells = line.Split(line.Contains('|') ? "|" : " ");

                if (cells.Length != 9)
                {
                    throw new ArgumentException("Cannot parse sudoku: " + sudoku);
                }

                for(int x = 0; x < 9; x++)
                {
                    if (string.IsNullOrWhiteSpace(cells[x]))
                    {
                        Grid[x][y] = null;
                    }
                    else
                    {
                        Grid[x][y] = int.Parse(cells[x]);
                    }
                }
            }
        }

        public Sudoku Copy()
        {
            var result = new Sudoku();
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    result.Grid[x][y] = Grid[x][y];
                }
            }

            return result;
        }

        public List<(int,int)> GetErrorCells()
        {
            List<(int, int)> errors = new List<(int, int)>();

            for (int i = 0; i < 9; i++)
            {
                errors.AddRange(GetErrorsInColumn(i));
                errors.AddRange(GetErrorsInRow(i));
                errors.AddRange(GetErrorsInBlock(i % 3, i / 3));
            }

            return errors;
        }

        public List<CellSolution> GetSolvableCells(List<SolutionStrategie> allowedStrategies)
        {
            List<CellSolution> result = new List<CellSolution>();

            for(int x = 0; x < 9; x++)
            {
                for(int y = 0; y < 9; y++)
                {
                    if (Grid[x][y].HasValue)
                    {
                        continue;
                    }

                    var solution = GetSolutionForCell(x, y, allowedStrategies);
                    if(solution.Successful)
                    {
                        result.Add(solution);
                    }
                }
            }

            return result;
        }

        public RecursiveSolveResult GetSolvableCellsRecursive(RecursiveSolvingOptions recursiveSolvingOptions)
        {
            if (recursiveSolvingOptions.AllowedStrategies == null)
            {
                recursiveSolvingOptions.AllowedStrategies = Enum.GetValues<SolutionStrategie>().Where(x => x != SolutionStrategie.None).ToList();
            }

            if (recursiveSolvingOptions.AllowedStrategies.Count == 0)
            {
                return new RecursiveSolveResult();
            }

            if (recursiveSolvingOptions.RecursiveStrategie == RecursiveStrategie.AllowMaxComplexity
                &&
                recursiveSolvingOptions.AllowedStrategies.Any(x => x.ToString().Contains("R5") || x.ToString().Contains("R10")))
            {
                // ahst to do here? exception or other? solving with max complexity with that many recursions takes very long
            }

            var target = Copy();

            var allowsStrategies = recursiveSolvingOptions.AllowedStrategies.OrderBy(x => x).ToArray();

            List<CellSolution> result = new List<CellSolution>();
            int iterationCount = 0;

            bool solutionFoundInIteration = true;

            while(solutionFoundInIteration)
            {
                var copy = target.Copy();
                solutionFoundInIteration = false;
                int strategyIndex = 0;

                while(strategyIndex < allowsStrategies.Length)
                {
                    iterationCount++;
                    SolutionStrategie[] targetStrategies;

                    if (recursiveSolvingOptions.RecursiveStrategie == RecursiveStrategie.PreferSimpleSolution)
                    {
                        var targetStrategy = allowsStrategies[strategyIndex];
                        targetStrategies = new SolutionStrategie[] { targetStrategy };
                    }
                    else
                    {
                        targetStrategies = allowsStrategies;
                    }


                    for (int x = 0; x < 9; x++)
                    {
                        for (int y = 0; y < 9; y++)
                        {
                            var solution = copy.GetSolutionForCell(x, y, targetStrategies);
                            if(solution.Successful && solution.WinningStrategie != SolutionStrategie.None)
                            {
                                result.Add(solution);
                                target.Grid[x][y] = solution.Value;
                                solutionFoundInIteration = true;
                            }
                        }
                    }

                    if (recursiveSolvingOptions.RecursiveStrategie == RecursiveStrategie.AllowMaxComplexity)
                    {
                        break; // all strategies have been used in the first iteration
                    }

                    if(solutionFoundInIteration && recursiveSolvingOptions.RecursiveStrategie == RecursiveStrategie.PreferSimpleSolution)
                    {
                        break; // stop iteration and start new
                    }
                    else
                    {
                        strategyIndex++;
                    }
                }
            }

            return new RecursiveSolveResult
            {
                CellSolutions = result,
                IterationCount = iterationCount
            };

        }

        private List<(int,int)> GetErrorsInColumn(int y)
        {
            var numbers = Grid[y].Where(n => n.HasValue).Select(n => n.Value).ToList();
            var errorNumbers = GetErrorNumbers(numbers);

            List<(int, int)> errors = new List<(int,int)>();

            if(errorNumbers.Count == 0)
            {
                return errors;
            }

            foreach(var err in errorNumbers)
            {
                for(int x = 0; x < 9; x++)
                {
                    if(Grid[y][x] == err)
                    {
                        errors.Add((y,x));
                    }
                }
            }

            return errors;
        }

        private List<(int, int)> GetErrorsInRow(int x)
        {
            var numbers = Grid.Select(row => row[x]).Where(n => n.HasValue).Select(n => n.Value).ToList();
            var errorNumbers = GetErrorNumbers(numbers);


            List<(int, int)> errors = new List<(int, int)>();

            if (errorNumbers.Count == 0)
            {
                return errors;
            }

            foreach (var err in errorNumbers)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (Grid[y][x] == err)
                    {
                        errors.Add((y, x));
                    }
                }
            }

            return errors;
        }

        private List<(int,int)> GetErrorsInBlock(int xBlock, int yBlock)
        {
            List<int> numbers = new List<int>();

            for (int i = 0; i < 3; i ++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var cell = Grid[xBlock * 3 + i][yBlock * 3 + j];
                    if (cell.HasValue)
                    {
                        numbers.Add(cell.Value);
                    }
                }
            }

            var errorNumbers = GetErrorNumbers(numbers);
            List<(int, int)> errors = new List<(int, int)>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var xCord = xBlock * 3 + i;
                    var yCord = yBlock * 3 + j;
                    var cell = Grid[xCord][yCord];
                    if (cell.HasValue && errorNumbers.Contains(cell.Value))
                    {
                        errors.Add((xCord, yCord));
                    }
                }
            }

            return errors;
        }

        private List<int> GetErrorNumbers(List<int> allNumbers)
        {
            allNumbers = allNumbers.ToList();
            var result = new List<int>();
            var distinctNumbers = allNumbers.Distinct().ToList();
            if (allNumbers.Count == distinctNumbers.Count)
            {
                return result;
            }

            foreach (var number in distinctNumbers)
            {
                allNumbers.Remove(number);
            }

            return allNumbers.Distinct().ToList();
        }

        private List<(int,int)> GetEmptyCellsInBlock(Block block)
        {
            List<(int,int)> result = new List<(int,int)> ();
            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    if (block[x][y] == null)
                    {
                        result.Add((x + block.XOffset, y + block.YOffset));
                    }
                }
            }

            return result;
        }

        private List<(int, int)> GetEmptyCellsInRow(int?[] row, int y)
        {
            List<(int, int)> result = new List<(int, int)>();
            for (var x = 0; x < 9; x++)
            {
                if(row[x] == null)
                {
                    result.Add((x, y));
                }
            }

            return result;
        }

        private List<(int, int)> GetEmptyCellsInColumn(int?[] column, int x)
        {
            List<(int, int)> result = new List<(int, int)>();
            for (var y = 0; y < 9; y++)
            {
                if (column[y] == null)
                {
                    result.Add((x, y));
                }
            }

            return result;
        }

        /// <summary>
        /// Checks whether a number can be placed in a cell by simple Row, column an block checks.
        /// </summary>
        /// <returns></returns>
        private bool CanNumberBePlacedSimple(int x, int y, int n)
        {
            var row = Row(y);
            var column = Column(x);
            var block = Block(x / 3, y / 3);

            var allNumbers = new List<int>();
            allNumbers.AddFilledCells(row);
            allNumbers.AddFilledCells(column);
            allNumbers.AddFilledCells(block);

            return allNumbers.All(ntest => ntest != n);
        }

        public Func<int, int, int, bool> CanNumberBePlacedWithConstraintsFactory(int maxRecursionLevel)
        {
            return (x, y, n) => CanNumberBePlacedWithConstraintsRecursive(x, y, n, 0, maxRecursionLevel);
        }

        private bool CanNumberBePlacedWithConstraintsRecursive(int x, int y, int n, int currentRecursionLevel, int maxRecursionLevel)
        {
            var canBePlacedSimple = CanNumberBePlacedSimple(x, y, n);
            if (!canBePlacedSimple || currentRecursionLevel == maxRecursionLevel) 
            {
                return canBePlacedSimple;
            }

            // first find constrainable block (block on same xBlock/yBlock that do not have "n")
            int xBlock = x / 3;
            int yBlock = y / 3;

            List<Block> constrainableBlocks = new List<Block>();

            for(int xB = 0; xB < 3; xB++)
            {
                if (xB != xBlock)
                {
                    var block = Block(xB, yBlock);
                    if (!block.Contains(n))
                    {
                        constrainableBlocks.Add(block);
                    }
                }
            }

            for (int yB = 0; yB < 3; yB++)
            {
                if (yB != yBlock)
                {
                    var block = Block(xBlock, yB);
                    if (!block.Contains(n))
                    {
                        constrainableBlocks.Add(block);
                    }
                }
            }


            foreach (var constrainableBlock in constrainableBlocks)
            {
                var emptyCell = GetEmptyCellsInBlock(constrainableBlock);
                var remainingCells = RecursiveCellExclusionForNumber(n, emptyCell, currentRecursionLevel + 1, maxRecursionLevel);

                if(remainingCells.Count == 0)
                {
                    continue;
                }

                if (constrainableBlock.XBlock == xBlock)
                {
                    // if the number n ist placed in the contrainable block on the same x level, n cannot be placed in this block
                    if(remainingCells.All(c => c.Item1 == x))
                    {
                        return false;
                    }
                }
                else // constrainableBlock.YBlock == yBlock
                {
                    // if the number n ist placed in the contrainable block on the same y level, n cannot be placed in this block
                    if (remainingCells.All(c => c.Item2 == y))
                    {
                        return false;
                    }
                }
            }


            return true;
        }

        public CellSolution GetSolutionForCell(int x, int y, IEnumerable<SolutionStrategie> solutionStrategies)
        {
            if(solutionStrategies == null)
            {
                solutionStrategies = Enum.GetValues<SolutionStrategie>().Where(x => x != SolutionStrategie.None);
            }

            if(solutionStrategies.Count() == 0)
            {
                return new CellSolution(x, y);
            }

            if (Grid[x][y].HasValue)
            {
                return new CellSolution(x, y, SolutionStrategie.None, Grid[x][y].Value);
            }

            foreach (var strategy in solutionStrategies)
            {
                var result = strategies[strategy](x, y);
                if (result.HasValue)
                {
                    return new CellSolution(x, y, strategy, result.Value);
                }
            }

            return new CellSolution(x, y);
        }

        private int? CombinedRemainingNumberStrategy(int x, int y)
        {
            var row = Row(y);
            var column = Column(x);
            var block = Block(x / 3, y / 3);

            var allNumbers = new List<int>();
            allNumbers.AddFilledCells(row);
            allNumbers.AddFilledCells(column);
            allNumbers.AddFilledCells(block);

            allNumbers = allNumbers.Distinct().ToList();

            if(allNumbers.Count != 8)
            {
                return null;
            }

            for (int i = 1; i < 10; i++)
            {
                if (!allNumbers.Contains(i))
                {
                    return i;
                }
            }

            return null;
        }

        #region ExclusionFactories

        private Func<int, int, int?> BlockExclusionFactory(Func<int, int, int, bool> placementCheck)
        {
            return (x, y) =>
            {
                var xBlockOffset = x - (x % 3);
                var yBlockOffset = y - (y % 3);

                var block = Block(x / 3, y / 3);

                var emptyCells = GetEmptyCellsInBlock(block);

                emptyCells.Remove((x, y));

                return FindNumberByExclusion(x, y, emptyCells, placementCheck);
            };
        }

        private Func<int, int, int?> RowExclusionFactory(Func<int, int, int, bool> placementCheck)
        {
            return (x, y) =>
            {
                var row = Row(y);

                var emptyCells = GetEmptyCellsInRow(row, y);

                return FindNumberByExclusion(x, y, emptyCells, placementCheck);
            };
        }

        private Func<int, int, int?> ColumnExclusionFactory(Func<int, int, int, bool> placementCheck)
        {
            return (x, y) =>
            {
                var column = Column(x);

                var emptyCells = GetEmptyCellsInColumn(column, x);

                return FindNumberByExclusion(x, y, emptyCells, placementCheck);
            };
        }

        private int? FindNumberByExclusion(int x, int y, List<(int,int)> emptyCellsToCheck, Func<int, int, int, bool> placementCheck)
        {
            emptyCellsToCheck.Remove((x, y));

            for (int n = 1; n < 10; n++)
            {
                if (!placementCheck(x, y, n))
                {
                    continue;
                }

                var isOtherCellAvailable = false;
                foreach (var (xEmpty, yEmpty) in emptyCellsToCheck)
                {
                    if (placementCheck(xEmpty, yEmpty, n))
                    {
                        isOtherCellAvailable = true;
                        break;
                    }
                }

                if (!isOtherCellAvailable)
                {
                    return n;
                }
            }

            return null;
        }

        /// <summary>
        /// Return the list of cell that the number n might be placed in (based on current contraints)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="emptyCellsToCheck"></param>
        /// <param name="currentRecursionLevel"></param>
        /// <param name="maxRecursionLevel"></param>
        /// <returns></returns>
        private List<(int, int)> RecursiveCellExclusionForNumber(int n, List<(int, int)> emptyCellsToCheck, int currentRecursionLevel, int maxRecursionLevel)
        {
            emptyCellsToCheck = emptyCellsToCheck.ToList();

            foreach (var cell in emptyCellsToCheck.ToArray())
            {
                if (!CanNumberBePlacedWithConstraintsRecursive(cell.Item1, cell.Item2, n, currentRecursionLevel, maxRecursionLevel))
                {
                    emptyCellsToCheck.Remove(cell);
                }
            }

            return emptyCellsToCheck;
        }

        #endregion
    }
}
