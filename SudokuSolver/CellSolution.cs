using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class CellSolution
    {
        public CellSolution(int x, int y, SolutionStrategie winningStrategie, int value)
        {
            X = x;
            Y = y;
            WinningStrategie = winningStrategie;
            Value = value;
            Successful = true;
        }

        public CellSolution(int x, int y)
        {
            Successful = false;
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        /// <summary>
        /// Solution Strategie that
        /// </summary>
        public SolutionStrategie WinningStrategie { get;  }
        public int Value { get; }
        public bool Successful { get; }

        public override string ToString()
        {
            return $"Successful: {Successful}, Value: {Value}, Winning Strategie: {WinningStrategie}";
        }
    }
}
