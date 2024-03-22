using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public enum SolutionStrategie
    {
        None = 0,
        /// <summary>
        /// Checks whether a number has to be placed becaus all other numbers are already
        /// present in the Block, Column and Row
        /// </summary>
        CombinedRemainingNumber = 1,
        /// <summary>
        /// Checks whether a number has to be placed somewhere because all other cells in a block cannot accept that number
        /// </summary>
        SimpleBlockExclusion = 2,
        /// <summary>
        /// Checks whether a number has to be placed somewhere because all other cells in a row cannot accept that number
        /// </summary>
        SimpleRowExclusion = 3,
        /// <summary>
        /// Checks whether a number has to be placed somewhere because all other cells in a column cannot accept that number
        /// </summary>
        SimpleColumnExclusion = 4,

        /// <summary>
        /// Like simple block exclusion, but additional contraints will be checked, recursing the contriants check once
        /// </summary>
        BlockExclusionWithContraintsR1 = 8,
        /// <summary>
        /// Like simple row exclusion, but additional contraints will be checked, recursing the contriants check once
        /// </summary>
        RowExclusionWithContraintsR1 = 9,
        /// <summary>
        /// Like simple column exclusion, but additional contraints will be checked, recursing the contriants check once
        /// </summary>
        ColumnExclusionWithContraintsR1 = 10,

        /// <summary>
        /// Like simple block exclusion, but additional contraints will be checked, recursing the contriants check twice
        /// </summary>
        BlockExclusionWithContraintsR2 = 11,
        /// <summary>
        /// Like simple row exclusion, but additional contraints will be checked, recursing the contriants check twice
        /// </summary>
        RowExclusionWithContraintsR2 = 12,
        /// <summary>
        /// Like simple column exclusion, but additional contraints will be checked, recursing the contriants check twice
        /// </summary>
        ColumnExclusionWithContraintsR2 = 13
    }
}
