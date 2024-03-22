using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public enum RecursiveStrategie
    {
        /// <summary>
        /// If a simple solution is found within a iteration, the algorithm retries all simpler solutions first bevor advancing to complexer algorithms
        /// </summary>
        PreferSimpleSolution = 0,
        /// <summary>
        /// Each algorithm will be used in each iteration, allowing a solution in less iteration buth with higher complexity
        /// </summary>
        AllowMaxComplexity = 1
    }

    public class RecursiveSolvingOptions
    {
        /// <summary>
        /// Strategies that should be used to solve. If null, all strategies will be used
        /// </summary>
        public List<SolutionStrategie> AllowedStrategies { get; set; }

        public RecursiveStrategie RecursiveStrategie { get; set; }
    }
}
