using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int CELL_SIZE = 30;

        private Sudoku sudoku;

        Button[][] cells = new Button[9][];

        Dictionary<string, string> loadableSudokus = new Dictionary<string, string>()
        {
            { "Empty", Examples.Empty },
            { "Example", Examples.Example1 },
            { "SimpleRow", Examples.SimpleSolve },
            { "Simple Block Exclusion", Examples.SimpleBlockExclusion },
            { "Simple Row Exclusion", Examples.SimpleRowExclusion },
            { "Simple Column Exclusion", Examples.SimpleColumnExclusion }
        };

        Dictionary<SolutionStrategie, Color> solutionColors = new Dictionary<SolutionStrategie, Color>()
        {
            { SolutionStrategie.None, Colors.Transparent },
            { SolutionStrategie.CombinedRemainingNumber, Colors.Yellow },
            { SolutionStrategie.SimpleBlockExclusion, Colors.Lime },
            { SolutionStrategie.SimpleRowExclusion, Colors.DarkGreen },
            { SolutionStrategie.SimpleColumnExclusion, Colors.DarkKhaki }
        };

        (int, int)? selectedCell;

        List<CellSolution> currentSolutions;

        public MainWindow()
        {
            InitializeComponent();

            Grid mainGrid = new Grid();
            mainGrid.Width = CELL_SIZE * 9;
            mainGrid.Height = CELL_SIZE * 9;
            MainBorder.Child = mainGrid;

            double blockWidth = CELL_SIZE * 3;

            for (int z = 0; z < 9; z++)
            {
                cells[z] = new Button[9];
            }

            for (int x = 0; x < 3; x++)
            {
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(blockWidth) });
                for(int y = 0; y < 3; y ++)
                {
                    if (x == 0)
                    {
                        mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(blockWidth) });
                    }

                    Border block = new Border();
                    block.BorderBrush = new SolidColorBrush(Colors.Black);
                    block.BorderThickness = new Thickness(1);
                    Grid.SetColumn(block, x);
                    Grid.SetRow(block, y);
                    mainGrid.Children.Add(block);

                    Grid blockGrid = new Grid();
                    blockGrid.Width = CELL_SIZE * 3;
                    blockGrid.Height = CELL_SIZE * 3;
                    block.Child = blockGrid;

                    for (int i = 0; i < 3; i++)
                    {
                        blockGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(CELL_SIZE) });
                        for (int j = 0; j < 3; j++)
                        {
                            if (i == 0)
                            {
                                blockGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(CELL_SIZE) });
                            }

                            Border cell = new Border();
                            cell.BorderBrush = new SolidColorBrush(Colors.Black);
                            cell.BorderThickness = new Thickness(.5);
                            Grid.SetColumn(cell, i);
                            Grid.SetRow(cell, j);
                            blockGrid.Children.Add(cell);

                            var button = new Button
                            {
                                Content = "",
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Width = CELL_SIZE - 2,
                                Height = CELL_SIZE - 2,
                                Background = new SolidColorBrush(Colors.Transparent),
                                BorderThickness = new Thickness(0)
                            };
                            cells[(x * 3) + i][(y * 3) + j] = button;
                            cell.Child = button;

                            var xTemp = (x * 3) + i;
                            var yTemp = (y * 3) + j;

                            button.Click += (sender, e) => Cell_Click(xTemp, yTemp);
                        }
                    }
                }
            }

            foreach(var key in loadableSudokus.Keys)
            {
                MenuItem menuItem = new MenuItem
                {
                    Header = key,
                };
                menuItem.Click += (sender, e) => MenuItemLoadExample_Click(loadableSudokus[key]);
                MenuItemLoad.Items.Add(menuItem);
            }


            sudoku = new Sudoku();
            UpdateCells(sudoku);
        }

        private void UpdateCells(Sudoku sudoku)
        {
            for (int x = 0; x < 9; x++)
            {
                for(int y = 0; y < 9; y++)
                {
                    cells[x][y].Content = sudoku.Grid[x][y]?.ToString();

                    if (selectedCell == (x, y))
                    {
                        cells[x][y].Background = new SolidColorBrush(Colors.Green);
                    }
                }
            }
        }

        private void Cell_Click(int xTarget, int yTarget)
        {
            selectedCell = (xTarget, yTarget);
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (x == xTarget && y == yTarget)
                    {
                        cells[x][y].Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        cells[x][y].Background = new SolidColorBrush(Colors.Transparent);
                    }
                }
            }
        }

        private void MenuItemTriggerErrorCheck_Click(object sender, RoutedEventArgs e)
        {
            ErrorCheck(false);
        }

        private void ClearBackground()
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    cells[x][y].Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        private void ErrorCheck(bool silentSuccess)
        {
            bool hasErrors = false;
            var errors = sudoku.GetErrorCells();

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (errors.Any(e => e.Item1 == x && e.Item2 == y))
                    {
                        hasErrors = true;
                        cells[x][y].Background = new SolidColorBrush(Colors.Red);
                    }
                }
            }

            if (hasErrors)
            {
                MessageBox.Show("Errors detected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if(!silentSuccess)
            {
                MessageBox.Show("All Cells valid", "Valid", MessageBoxButton.OK, MessageBoxImage.None);
            }
        }

        private void SetCell(int x, int y, int? value)
        {
            sudoku.Grid[x][y] = value;
            UpdateCells(sudoku);
            ErrorCheck(true);
        }

        private void MenuItemSolveSelectedCell_Click(object sender, RoutedEventArgs e)
        {
            ClearBackground();
            if (selectedCell == null)
            {
                return;
            }

            var solution = sudoku.GetSolutionForCell(selectedCell.Value.Item1, selectedCell.Value.Item2, null);

            if (solution.Successful)
            {
                SetCell(selectedCell.Value.Item1, selectedCell.Value.Item2, solution.Value);
            }
            else
            {
                MessageBox.Show("Solution not yet possible");
            }
        }

        private void MenuItemLoadExample_Click(string example)
        {
            ClearBackground();
            sudoku = new Sudoku();
            sudoku.Parse(example);
            UpdateCells(sudoku);
        }

        private void MenuItemClear_Click(object sender, RoutedEventArgs e)
        {
            ClearBackground();
            sudoku = new Sudoku();
            UpdateCells(sudoku);
        }

        private void ButtonFindSolvableCells_Click(object sender, RoutedEventArgs e)
        {
            ClearBackground();
            currentSolutions = sudoku.GetSolvableCells(null);

            HashSet<SolutionStrategie> usedStrageties = new HashSet<SolutionStrategie>();

            foreach (var solution in currentSolutions)
            {
                cells[solution.X][solution.Y].Background = new SolidColorBrush(solutionColors[solution.WinningStrategie]);
                usedStrageties.Add(solution.WinningStrategie);
            }

            (DataContext as MainViewModel).SetSolutionLegend(usedStrageties, solutionColors, 1);

            if (currentSolutions.Count == 0)
            {
                MessageBox.Show("No possible solutions available");
                return;
            }
        }

        private void ButtonApplySolvableCells_Click(object sender, RoutedEventArgs e)
        {
            bool updateLegend = false;
            if(currentSolutions == null)
            {
                updateLegend = true;
                currentSolutions = sudoku.GetSolvableCells(null);
            }

            if(currentSolutions.Count == 0)
            {
                MessageBox.Show("No possible solutions available");
                return;
            }
            HashSet<SolutionStrategie> usedStrageties = new HashSet<SolutionStrategie>();

            foreach (var solution in currentSolutions)
            {
                SetCell(solution.X, solution.Y, solution.Value);
                cells[solution.X][solution.Y].Background = new SolidColorBrush(solutionColors[solution.WinningStrategie]);
                usedStrageties.Add(solution.WinningStrategie);
            }
            if(updateLegend)
            {
                (DataContext as MainViewModel).SetSolutionLegend(usedStrageties, solutionColors, 1);
            }
            else
            {
                ClearBackground();
            }

            currentSolutions = null;
        }

        private void ButtonFindSolvableCellsRecursiveSimple_Click(object sender, RoutedEventArgs e)
        {
            MarkSolvableRecursive(RecursiveStrategie.PreferSimpleSolution);
        }

        private void ButtonFindSolvableCellsRecursiveComplex_Click(object sender, RoutedEventArgs e)
        {
            MarkSolvableRecursive(RecursiveStrategie.AllowMaxComplexity);
        }

        private void MarkSolvableRecursive(RecursiveStrategie strategie)
        {
            ClearBackground();

            var options = new RecursiveSolvingOptions
            {
                RecursiveStrategie = strategie
            };

            var result = sudoku.GetSolvableCellsRecursive(options);

            currentSolutions = result.CellSolutions;

            if (!result.FoundSolutions)
            {
                MessageBox.Show("No possible solutions available");
                return;
            }

            HashSet<SolutionStrategie> usedStrageties = new HashSet<SolutionStrategie>();

            foreach (var solution in currentSolutions)
            {
                cells[solution.X][solution.Y].Background = new SolidColorBrush(solutionColors[solution.WinningStrategie]);
                usedStrageties.Add(solution.WinningStrategie);
            }
            (DataContext as MainViewModel).SetSolutionLegend(usedStrageties, solutionColors, result.IterationCount);
        }
    }
}
