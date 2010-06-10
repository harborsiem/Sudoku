using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku {

    public sealed class SudokuXml {

        public const string RootTag = "SudokuValues";
        public const string ItemTag = "Item";
        public const string RowAttribute = "row";
        public const string ColumnAttribute = "column";
        public const string ValueAttribute = "value";

        private SudokuXml() { }
    }
}
