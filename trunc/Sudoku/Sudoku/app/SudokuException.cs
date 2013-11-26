using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Sudoku {
    [Serializable]
    public class SudokuException : Exception {

        public SudokuException()
            : base() {
        }

        public SudokuException(string message, Exception cause)
            : base(message, cause) {
        }

        public SudokuException(string message)
            : base(message) {
        }

        protected SudokuException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
    }
}