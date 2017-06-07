using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku {
    interface IView {
        void ValuesToSudokuField(uint[,] matrix);

        void ShowError();
    }
}
