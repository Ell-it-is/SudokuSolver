using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] sudoku = new int[,]
            {
                //Ukazkove
                {0,2,0,0,8,0,0,3,0},
                {3,0,0,0,7,0,0,0,0},
                {0,0,0,0,0,0,1,9,0},
                {0,0,6,2,4,0,0,0,0},
                {0,0,0,0,0,0,0,0,1},
                {2,0,8,0,3,6,0,0,0},
                {4,0,0,0,6,0,0,0,0},
                {5,0,2,4,0,0,0,0,0},
                {0,6,0,0,9,7,8,0,4}
            };


            SudokuSolver solver = new SudokuSolver(sudoku);
            solver.Solve();
            solver.VykresliSudoku();
            Console.WriteLine();
            solver.VytiskniKandidaty();
            Console.ReadLine();
        }
    }
}
