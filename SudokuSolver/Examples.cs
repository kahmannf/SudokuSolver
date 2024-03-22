using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public static class Examples
    {
        public const string Empty =
            " | | | | | | | | " + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | | | | | ";

        public const string Example1 =
              " | | |5| |2| | | \n"
            + " | |5|4| |7|1| | \n"
            + " |9| | |1| | |2| \n"
            + "4|3| | |5| | |7|1\n"
            + " | |6|3| |1|4| | \n"
            + "8|1| | |4| | |5|6\n"
            + " |5| | |6| | |9| \n"
            + " | |1|2| |5|8| | \n"
            + " | | |9| |4| | | ";


        public const string SimpleSolve =
            "1|3|5|2| |6|7|9|8" + "\n" +
            " | | | | | | | |2" + "\n" +
            " | | | | | | | |3" + "\n" +
            " | | |6| |2| | |4" + "\n" +
            " | | |3|9|1| | |5" + "\n" +
            " | | |4|8|7| | |6" + "\n" +
            " | | | | | | | |9" + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | | | | |7";
        
        public const string SimpleBlockExclusion =
            "1| | | | | | | | " + "\n" +
            " | | | | | | | | " + "\n" +
            " | | |1| | | | | " + "\n" +
            " | | | | | | | |1" + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | | | |1| " + "\n" +
            " | | | | | | | | ";

        public const string SimpleRowExclusion =
            " | |4| | |3|8| |9" + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | | | | |1" + "\n" +
            "1| | | | | | | | " + "\n" +
            " | | | |1| | | | " + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | | | | | " + "\n" +
            " |1| | | | | | | " + "\n" +
            " | | | | | | | | ";

        public const string SimpleColumnExclusion =
            " | | |1| | | | | " + "\n" +
            " | | | |2| | | | " + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | | | | | " + "\n" +
            " | | | | |2| | | " + "\n" +
            " | | | | | |1| | " + "\n" +
            " | | | | |4| | | " + "\n" +
            " | | | | |5| | | " + "\n" +
            " | | | | |7| | | ";
    }
}
