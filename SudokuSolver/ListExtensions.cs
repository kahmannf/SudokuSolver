using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public static class ListExtensions
    {
        public static void AddFilledCells(this List<int> target, IEnumerable<int?> cells)
        {
            target.AddRange(cells.Where(x => x.HasValue).Select(x => x.Value));
        }
        
        public static void AddFilledCells(this List<int> target, IEnumerable<IEnumerable<int?>> cells)
        {
            target.AddRange(cells.SelectMany(x => x).Where(x => x.HasValue).Select(x => x.Value));
        }
    }
}
