using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Sudoku {
    class Presenter {

        private IView view;
        private uint[,] inputs = new uint[SudokuSolver.Max, SudokuSolver.Max];
        private uint[,] outputs = new uint[SudokuSolver.Max, SudokuSolver.Max];
        private SudokuError errCode;
        private SudokuOption option;
        private SudokuSolver sudoku;
        private bool threadRunning;
        private bool isAbortPossible;
        private bool textboxesWithOutputs;

        public Presenter(IView view) {
            this.view = view;
            sudoku = new SudokuSolver();
        }

        public SudokuError ErrCode {
            get { return errCode; }
            set { errCode = value; }
        }

        public SudokuOption Option {
            get { return option; }
            set { option = value; }
        }

        public void ClearInputsMatrix() {
            Array.Clear(inputs, 0, inputs.Length);
        }

        public void SetInput(int row, int col, uint value) {
            inputs[row, col] = value;
        }

        public bool Calculate() {
            return sudoku.Execute(inputs, out outputs, option, out errCode);
        }

        public void CalculateAbort() {
            sudoku.Abort = true;
        }
    }
}
