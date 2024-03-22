using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public class Block : IEnumerable<IEnumerable<int?>>
    {
        int?[][] block;

        public int XBlock { get; }
        public int YBlock { get; }

        public int XOffset { get; }
        public int YOffset { get; }

        public int?[] this[int x]
        {
            get { return block[x]; }
            set { block[x] = value; }
        }

        public Block(int?[][] block, int xBlock, int yBlock)
        {
            this.block = block;
            XBlock = xBlock;
            YBlock = yBlock;
            XOffset = xBlock * 3;
            YOffset = yBlock * 3;
        }

        public bool Contains(int n)
        {
            return block.SelectMany(x => x).Any(nB => nB == n);
        }

        public IEnumerator<IEnumerable<int?>> GetEnumerator()
        {
            return (block as IEnumerable<IEnumerable<int?>>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return block.GetEnumerator();
        }
    }
}
