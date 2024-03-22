using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class RecursiveSolveResult
    {
        public bool FoundSolutions => CellSolutions.Count > 0;

        /// <summary>
        /// Solutions Generated
        /// </summary>
        public List<CellSolution> CellSolutions { get; set; } = new List<CellSolution>();

        /// <summary>
        /// Amount of iterations used
        /// </summary>
        public int IterationCount { get; set; }
    }
}
