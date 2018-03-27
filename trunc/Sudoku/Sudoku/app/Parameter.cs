using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku {
    public class Parameter {

        private uint[,] inputValues;
        private uint[,] outputValues;
        private SudokuOption option;
        private SudokuError errorCode;

        public Parameter(uint[,] inputValue, SudokuOption option) {
            this.inputValues = inputValue;
            this.option = option;
        }

        public SudokuOption Option { get { return option; } }
        public SudokuError ErrorCode { get { return errorCode; } set { errorCode = value; } }
        public uint[,] InputValues { get { return inputValues; } }
        public uint[,] OutputValues { get { return outputValues; } set { outputValues = value; } }
        public bool IsCalculated { get; set; }
    }
}
