// =========================================================================
// Copyright (C) Harbor 2008.  All Rights Reserved. Confidential
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// File:           SudokuSolver.cs
// Project:
// Author:         harbor
// Creation date:  09.11.2007
// based on
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// D e s c r i p t i o n :
//
// Sudoku-Solver
//
// E n d  D e s c r i p t i o n
// =========================================================================

// #########################################################################
// using
// #########################################################################

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

// #########################################################################
// basic data types
// #########################################################################
//bool
//char  //16bit unicode
//float
//double
//decimal
//string
//object

//typedef byte       Bitf8;
//typedef ushort     Bitf16;
//typedef uint       Bitf32;
//typedef sbyte      Int8;
//typedef short      Int16;
//typedef int        Int32;
//typedef long       Int64;
//typedef byte       Uint8;
//typedef ushort     Uint16;
//typedef uint       Uint32;
//typedef ulong      Uint64;

// #########################################################################
// namespace
// #########################################################################

namespace Sudoku {
    // #########################################################################
    // class
    // #########################################################################

    public sealed class BitSet {
        private BitSet() { }

        public static void Include(ref ulong bitValue, int value) {
            bitValue |= (ulong)(1L << (value & 63));
        }
        public static void Include(ref uint bitValue, int value) {
            bitValue |= (uint)(1 << (value & 31));
        }
        public static void Include(ref ushort bitValue, int value) {
            bitValue |= (ushort)(1 << (value & 15));
        }
        public static void Include(ref byte bitValue, int value) {
            bitValue |= (byte)(1 << (value & 7));
        }
        public static void Exclude(ref ulong bitValue, int value) {
            bitValue &= (ulong)~(1L << (value & 63));
        }
        public static void Exclude(ref uint bitValue, int value) {
            bitValue &= (uint)~(1 << (value & 31));
        }
        public static void Exclude(ref ushort bitValue, int value) {
            bitValue &= (ushort)~(1 << (value & 15));
        }
        public static void Exclude(ref byte bitValue, int value) {
            bitValue &= (byte)~(1 << (value & 7));
        }
        public static bool IsInBitset(ulong bitValue, int value) {
            if ((bitValue & (ulong)(1L << (value & 63))) != 0) {
                return true;
            } else {
                return false;
            }
        }
        public static bool IsInBitset(uint bitValue, int value) {
            if ((bitValue & (1 << (value & 31))) != 0) {
                return true;
            } else {
                return false;
            }
        }
        public static bool IsInBitset(ushort bitValue, int value) {
            if ((bitValue & (1 << (value & 15))) != 0) {
                return true;
            } else {
                return false;
            }
        }
        public static bool IsInBitset(byte bitValue, int value) {
            if ((bitValue & (1 << (value & 7))) != 0) {
                return true;
            } else {
                return false;
            }
        }
    }

    [Flags]
    public enum SudokuOption {
        None,
        WithFile = 1,
        AddonInFile = 2,
        WithFileAddon = WithFile | AddonInFile
    };

    public enum SudokuError {
        None,
        WrongInput,
        FewValues,
        Aborted
    };

    // #########################################################################
    // class
    // #########################################################################

    public sealed class SudokuSolver : IDisposable {
        // #########################################################################
        // constants
        // #########################################################################

        public const uint Max = 9;

        private const uint Min = 1;
        private const uint CarreeMax = 3; //(uint)Math.Sqrt(Max);

        private const uint MinInputValues = 15;
        private const string Try = "**********Try*********";
        private const string TryNotOk = "*** Try not OK ***";
        private const string TryIsPossible = "*** Try is possible ***";
        private const string TryInRowColumn = "*** Try in [Row, Column] ";

        //public enum tValue { 0 .. Max };
        //public enum tIndex { 0 .. (Max-1) };

        // #########################################################################
        // data types
        // #########################################################################

        private struct ValueType {
            public uint Occupied; //Bitset
            public uint Number; //tValue
            //public override bool Equals(object obj) {
            //    return base.Equals(obj);
            //}
            //public override int GetHashCode() {
            //    return base.GetHashCode();
            //}
        }

        private class TryValueType {
            public uint Occupied;
            public uint Number; //tValue
            public uint Row; //tIndex
            public uint Column; //tIndex
        }

        // #########################################################################
        // variables
        // #########################################################################

        private bool fileOpen;
        private bool fileOpenForAll;
        private StreamWriter streamWriter;
        private bool[,] pass2Checked = new bool[Max, Max]; //array[tIndex,tIndex];
        private uint[] m_smallField = new uint[Max];
        private bool m_abort;

        // #########################################################################
        // class
        // #########################################################################

        private class SolveHelper {
            // #########################################################################
            // constants
            // #########################################################################

            //private const string PossibleResults = "moegliche Ergebnisse [Zeile, Spalte]";
            //private const string EvaluateText = "Bewerten";
            //private const string RowTest = "Zeilentest";
            //private const string ColumnTest = "Spaltentest";
            //private const string CarreeTest = "Carreetest";
            //private const string Inputs = "Eingabe:";
            //private const string Result = "Ergebnis:";
            //private const string Insert = "Fuege ";
            //private const string To = " in ";
            //private const string Ein = " ein";
            //private const string Row = "Zeile ";
            //private const string Column = "Spalte ";
            //private const string Carree = "Carree ";
            //private const string Finished = " fertig";
            private const string PossibleResults = "Possible results [Row, Column]";
            private const string EvaluateText = "Evaluate";
            private const string RowTest = "Testing rows";
            private const string ColumnTest = "Testing columns";
            private const string CarreeTest = "Testing carrees";
            private const string Inputs = "Inputs:";
            private const string Result = "Result:";
            private const string Insert = "Insert ";
            private const string To = " to ";
            private const string Ein = "";
            private const string Row = "row ";
            private const string Column = "column ";
            private const string Carree = "carree ";
            private const string Finished = " finished";

            // #########################################################################
            // variables
            // #########################################################################

            public uint[,] board = new uint[Max, Max]; // Playing board 
            //tBoard = array[tIndex,tIndex] of tValue;
            public ValueType[,] boardEstimation = new ValueType[Max, Max]; //array[tIndex,tIndex] of ValueType;
            public TryValueType tryValue = new TryValueType();
            private bool m_reCalc;

            private uint[,] sudokuBoard = new uint[Max, Max]; //sudokuBoard : tBoard;
            private ValueType[] columns = new ValueType[Max]; //array[tIndex];
            private ValueType[] rows = new ValueType[Max]; //array[tIndex];
            private ValueType[] carrees = new ValueType[Max]; //array[tIndex];

            private SudokuSolver sudoku;
            //private bool m_startValuesSet;
            private int m_validationCounter;
            private int m_newValCalculated;

            // #########################################################################
            // functions
            // #########################################################################

            public SolveHelper(SudokuSolver sudoku) {
                this.sudoku = sudoku;
            }

            public bool ReCalc {
                set { m_reCalc = value; }
            }

            public bool SetStartValues(uint[,] matrix) {
                uint row, column;
                uint value;
                bool result;
                result = true;
                for (row = 0; row < Max; row++) {
                    for (column = 0; column < Max; column++) {
                        value = matrix[row, column];
                        if ((value < 0) || (value > Max)) {
                            result = false;
                        }
                        sudokuBoard[row, column] = value;
                    }
                }
                //m_startValuesSet = result;
                return result;
            }

            public uint[,] GetResultValues() {
                return board;
            }

            private static uint CarreePos(uint aRow, uint aColumn) {
                return ((aColumn / CarreeMax) + (CarreeMax * (aRow / CarreeMax)));
            }

            private void InsertValColumn(uint aColumn, uint value) {
                if ((value < Min) || (value > Max)) {
                    return;
                }
                BitSet.Include(ref columns[aColumn].Occupied, (int)value);
                columns[aColumn].Number++;
            }

            private void InsertValRow(uint aRow, uint value) {
                if ((value < Min) || (value > Max)) {
                    return;
                }
                BitSet.Include(ref rows[aRow].Occupied, (int)value);
                rows[aRow].Number++;
            }

            private void InsertValCarree(uint aCarree, uint value) {
                if ((value < Min) || (value > Max)) {
                    return;
                }
                BitSet.Include(ref carrees[aCarree].Occupied, (int)value);
                carrees[aCarree].Number++;
            }

            private void InsertValue(uint aRow, uint aColumn, uint value) {
                board[aRow, aColumn] = value;
                InsertValColumn(aColumn, value);
                InsertValRow(aRow, value);
                InsertValCarree(CarreePos(aRow, aColumn), value);
                m_newValCalculated++;
            }

            private void BoardOutput(uint[,] board) {
                uint row, column;
                if (sudoku.fileOpen) {
                    for (row = 0; row < Max; row++) {
                        for (column = 0; column < Max; column++) {
                            sudoku.streamWriter.Write(String.Format(CultureInfo.InvariantCulture, "{0,4}", board[row, column].ToString(CultureInfo.InvariantCulture)));
                        }
                        sudoku.streamWriter.WriteLine();
                    }
                    sudoku.streamWriter.WriteLine();
                }
            }

            //returns a single value of the set, otherwise 0
            private static uint SingleValueFromSet(uint aSet) {
                uint i;
                uint count;
                uint result = 0;
                if (aSet == 0) {
                    return result;
                }
                count = 0;
                for (i = Min; i <= Max; i++) {
                    if (BitSet.IsInBitset(aSet, (int)i)) {
                        count++;
                        result = i;
                    }
                }
                if (count > 1) {
                    result = 0;
                }
                return result;
            }

            private ValueType Evaluate(uint aRow, uint aColumn) {
                uint i;
                ValueType result = new ValueType();
                m_validationCounter++;
                result.Number = 0;
                result.Occupied = 0;
                if (board[aRow, aColumn] == 0) {
                    result.Occupied = rows[aRow].Occupied | columns[aColumn].Occupied |
                                  carrees[CarreePos(aRow, aColumn)].Occupied;
                    for (i = Min; i <= Max; i++) {
                        if (BitSet.IsInBitset(result.Occupied, (int)i)) {
                            BitSet.Exclude(ref result.Occupied, (int)i);
                        } else {
                            result.Number++;
                            BitSet.Include(ref result.Occupied, (int)i);
                        }
                    }
                }
                return result;
            }

            public bool BoardCheck() {
                uint row, column;
                ValueType estimate;
                uint i;
                bool result = true;
                tryValue.Number = 0;
                if (sudoku.fileOpenForAll) {
                    sudoku.streamWriter.WriteLine(PossibleResults);
                }
                for (row = 0; row < Max; row++) {
                    for (column = 0; column < Max; column++) {
                        estimate = boardEstimation[row, column];
                        if ((estimate.Number > tryValue.Number) && !(sudoku.pass2Checked[row, column])) {
                            tryValue.Number = estimate.Number;
                            tryValue.Occupied = estimate.Occupied;
                            tryValue.Row = row;
                            tryValue.Column = column;
                        }
                        if (sudoku.fileOpenForAll) {
                            StringBuilder possibleResults = new StringBuilder(32);
                            for (i = Min; i <= Max; i++) {
                                if (BitSet.IsInBitset(boardEstimation[row, column].Occupied, (int)i)) {
                                    possibleResults.Append(i.ToString(CultureInfo.InvariantCulture));
                                    possibleResults.Append(", ");
                                }
                            }
                            if (possibleResults.Length > 0) {
                                possibleResults.Length -= 2;
                                sudoku.streamWriter.WriteLine("[" + (row + 1).ToString(CultureInfo.InvariantCulture) + ", " + (column + 1).ToString(CultureInfo.InvariantCulture) + "] : "
                                   + possibleResults);
                            }
                        }
                        if ((estimate.Number != 0) || (board[row, column] == 0)) {
                            result = false;
                        }
                    }
                }
                return result;
            }

            private bool BoardEvaluation() {
                uint row, column, value;
                bool result;
                if (sudoku.fileOpenForAll) {
                    sudoku.streamWriter.WriteLine(EvaluateText);
                }
                do {
                    result = true;
                    for (row = 0; row < Max; row++) {
                        for (column = 0; column < Max; column++) {
                            if (!m_reCalc) {
                                boardEstimation[row, column] = Evaluate(row, column);
                            }
                            if (boardEstimation[row, column].Number == 1) {
                                result = false;
                                for (value = Min; value <= Max; value++) {
                                    if (BitSet.IsInBitset(boardEstimation[row, column].Occupied, (int)value)) {
                                        if (sudoku.fileOpenForAll) {
                                            sudoku.streamWriter.WriteLine(Insert + value.ToString(CultureInfo.InvariantCulture) + To + "["
                                                + (row + 1).ToString(CultureInfo.InvariantCulture) + "," + (column + 1).ToString(CultureInfo.InvariantCulture) + "]" + Ein);
                                        }
                                        InsertValue(row, column, value);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                while (!result);
                return result;
            }

            private bool BoardEvaluationRows() {
                uint row, column;
                uint i;
                uint value;
                uint testSet;
                bool result;
                if (sudoku.fileOpenForAll) {
                    sudoku.streamWriter.WriteLine(RowTest);
                }
                result = true;
                for (row = 0; row < Max; row++) {
                    if (rows[row].Number < Max) {
                        for (column = 0; column < Max; column++) {
                            testSet = 0;
                            for (i = 0; i < Max; i++) {
                                if (i != column) {
                                    testSet = testSet | boardEstimation[row, i].Occupied;
                                }
                            }
                            testSet = boardEstimation[row, column].Occupied & (~(testSet));
                            value = SingleValueFromSet(testSet);
                            if (value != 0) {
                                if (sudoku.fileOpenForAll) {
                                    sudoku.streamWriter.WriteLine(Insert + value.ToString(CultureInfo.InvariantCulture) + To + "[" + (row + 1).ToString(CultureInfo.InvariantCulture)
                                        + "," + (column + 1).ToString(CultureInfo.InvariantCulture) + "]" + Ein);
                                }
                                InsertValue(row, column, value);
                                result = false;
                            }
                        }
                    }    // if (rows[row].Number < Max)
                    else {
                        if (sudoku.fileOpenForAll) {
                            sudoku.streamWriter.WriteLine(Row + (row + 1).ToString(CultureInfo.InvariantCulture) + Finished);
                        }
                    }
                }   // for row
                BoardEvaluation();
                return result;
            }

            private bool BoardEvaluationColumns() {
                uint row, column;
                uint i;
                uint value;
                uint testSet;
                bool result;
                if (sudoku.fileOpenForAll) {
                    sudoku.streamWriter.WriteLine(ColumnTest);
                }
                result = true;
                for (column = 0; column < Max; column++) {
                    if (columns[column].Number < Max) {
                        for (row = 0; row < Max; row++) {
                            testSet = 0;
                            for (i = 0; i < Max; i++) {
                                if (i != row) {
                                    testSet = testSet | boardEstimation[i, column].Occupied;
                                }
                            }
                            testSet = boardEstimation[row, column].Occupied & (~(testSet));
                            value = SingleValueFromSet(testSet);
                            if (value != 0) {
                                if (sudoku.fileOpenForAll) {
                                    sudoku.streamWriter.WriteLine(Insert + value.ToString(CultureInfo.InvariantCulture) + To + "[" + (row + 1).ToString(CultureInfo.InvariantCulture)
                                        + "," + (column + 1).ToString(CultureInfo.InvariantCulture) + "]" + Ein);
                                }
                                InsertValue(row, column, value);
                                result = false;
                            }
                        }
                    } else {
                        if (sudoku.fileOpenForAll) {
                            sudoku.streamWriter.WriteLine(Column + (column + 1).ToString(CultureInfo.InvariantCulture) + Finished);
                        }
                    }
                }
                BoardEvaluation();
                return result;
            }

            private bool BoardEvaluationCarrees() {
                uint row, column, carree;
                uint i, j, hRow, hColumn;
                uint value;
                uint testSet = 0;
                bool result;
                if (sudoku.fileOpenForAll) {
                    sudoku.streamWriter.WriteLine(CarreeTest);
                }
                result = true;
                for (carree = 0; carree < Max; carree++) {
                    row = (carree / CarreeMax) * CarreeMax;
                    column = (carree * CarreeMax) % Max;
                    if (carrees[carree].Number < Max) {
                        for (hRow = 0; hRow < CarreeMax; hRow++) {
                            for (hColumn = 0; hColumn < CarreeMax; hColumn++) {
                                testSet = 0;
                                for (i = 0; i < CarreeMax; i++) {
                                    for (j = 0; j < CarreeMax; j++) {
                                        if (!((i == hRow) && (j == hColumn)))
                                            testSet = testSet | boardEstimation[row + i, column + j].Occupied;
                                    }
                                }
                                testSet = boardEstimation[row + hRow, column + hColumn].Occupied & ~testSet;
                                value = SingleValueFromSet(testSet);
                                if (value != 0) {
                                    if (sudoku.fileOpenForAll) {
                                        sudoku.streamWriter.WriteLine(Insert + value.ToString(CultureInfo.InvariantCulture) + To + "["
                                            + (row + hRow + 1).ToString(CultureInfo.InvariantCulture) + "," + (column + hColumn + 1).ToString(CultureInfo.InvariantCulture) + "]" + Ein);
                                    }
                                    InsertValue(row + hRow, column + hColumn, value);
                                    result = false;
                                }
                            }
                        }
                    } else {
                        if (sudoku.fileOpenForAll) {
                            sudoku.streamWriter.WriteLine(Carree + (carree + 1).ToString(CultureInfo.InvariantCulture) + Finished);
                        }
                    }
                }
                BoardEvaluation();
                return result;
            }

            public bool Evaluation() {
                bool ready;

                do {
                    m_newValCalculated = 0;
                    do { }
                    while (!BoardEvaluationRows());
                    do { }
                    while (!BoardEvaluationColumns());
                    do { }
                    while (!BoardEvaluationCarrees());
                    ready = BoardCheck();
                }
                while (!(ready || (m_newValCalculated == 0)));
                return ready;
            }

            public bool Calculate() {
                uint row, column;
                bool ready;

                rows.Initialize();
                columns.Initialize();
                carrees.Initialize();
                board.Initialize();
                boardEstimation.Initialize();
                if (sudoku.fileOpen) {
                    sudoku.streamWriter.WriteLine(Inputs);
                    BoardOutput(sudokuBoard);
                }
                for (row = 0; row < Max; row++) {
                    for (column = 0; column < Max; column++) {
                        InsertValue(row, column, sudokuBoard[row, column]);
                    }
                }
                BoardEvaluation();
                m_validationCounter = 0;
                ready = Evaluation();
                if (sudoku.fileOpen) {
                    sudoku.streamWriter.WriteLine(Result);
                    BoardOutput(board);
                }
                return ready;
            }
        }

        // #########################################################################
        // functions in class SudokuSolver
        // #########################################################################

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (disposing) {
                if (fileOpen) {
                    streamWriter.Dispose();
                }
            }
        }

        private bool CompareBoardFields(SolveHelper mainSudoku, SolveHelper helperSudoku) {
            uint row, column;
            uint mainValue, helperValue; //tValue;
            bool result = true;
            for (row = 0; row < Max; row++) {
                for (column = 0; column < Max; column++) {
                    helperValue = helperSudoku.board[row, column];
                    if (helperValue != 0) {
                        mainValue = mainSudoku.board[row, column];
                        if (mainValue != 0) {
                            if (mainValue != helperValue) {
                                result = false;
                            }
                        } else {
                            if (!(BitSet.IsInBitset(mainSudoku.boardEstimation[row, column].Occupied, (int)helperValue))) {
                                result = false;
                            }
                        }
                    } else {
                        if (helperSudoku.boardEstimation[row, column].Number == 0) {
                            result = false;
                        }
                        if (!(pass2Checked[row, column])) {
                            if (!(mainSudoku.boardEstimation[row, column].Occupied >= helperSudoku.boardEstimation[row, column].Occupied)) {
                                result = false;
                            }
                        }
                    }
                }
            }
            return result;
        }

        private static int GetNumber(uint occupied) {
            int i;
            int result = 0;
            for (i = (int)Min; i <= (int)Max; i++) {
                if (BitSet.IsInBitset(occupied, i)) {
                    result = i;
                    break;
                }
            }
            return result;
        }

        private bool CheckRow(uint aRow, uint[,] inVal) {
            uint i;

            for (i = 0; i < Max; i++) {
                m_smallField[i] = inVal[aRow, i];
            }
            return CheckSmallField();
        }

        private bool CheckColumn(uint aColumn, uint[,] inVal) {
            uint i;

            for (i = 0; i < Max; i++) {
                m_smallField[i] = inVal[i, aColumn];
            }
            return CheckSmallField();
        }

        private bool CheckCarree(uint carree, uint[,] inVal) {
            uint row, column;
            uint i, hRow, hColumn;
            row = (carree / CarreeMax) * CarreeMax;
            column = (carree * CarreeMax) % Max;
            i = 0;
            for (hRow = 0; hRow < CarreeMax; hRow++) {
                for (hColumn = 0; hColumn < CarreeMax; hColumn++) {
                    m_smallField[i] = inVal[row + hRow, column + hColumn];
                    i++;
                }
            }
            return CheckSmallField();
        }

        private bool CheckSmallField() {
            uint column, i;
            uint value;

            bool result = true;
            for (column = 0; column < Max; column++) {
                value = m_smallField[column];
                if ((column < Max) && (value > 0)) {
                    for (i = column + 1; i < Max; i++) {
                        if (value == m_smallField[i]) {
                            result = false;
                        }
                    }
                }
            }
            return result;
        }

        private SudokuError InputCheck(uint[,] inVal) {
            uint row, column, i;
            uint value;

            SudokuError result = SudokuError.None;
            int inputNumber = 0;
            for (row = 0; row < Max; row++) {
                for (column = 0; column < Max; column++) {
                    value = inVal[row, column];
                    if ((value < 0) || (value > Max)) {
                        result = SudokuError.WrongInput;
                    } else {
                        if (value > 0) {
                            inputNumber++;
                        }
                    }
                }
            }
            if (inputNumber < MinInputValues) {
                result = SudokuError.FewValues;
                return result;
            }
            for (i = 0; i < Max; i++) {
                if (!(CheckRow(i, inVal))) {
                    result = SudokuError.WrongInput;
                }
            }
            for (i = 0; i < Max; i++) {
                if (!(CheckColumn(i, inVal))) {
                    result = SudokuError.WrongInput;
                }
            }
            for (i = 0; i < Max; i++) {
                if (!(CheckCarree(i, inVal))) {
                    result = SudokuError.WrongInput;
                }
            }
            return result;
        }

        private bool TryExecute(SolveHelper mainSudoku, ref uint[,] outVal) {
            uint[,] tryField = new uint[Max, Max];
            uint[,] field = new uint[Max, Max];
            ValueType[,] fieldEstimation = new ValueType[Max, Max];
            int testNumber;
            string streamString;
            bool abort = false;
            bool result = false;

            mainSudoku.ReCalc = true;
            if (fileOpenForAll) {
                streamWriter.WriteLine(Try);
                streamWriter.WriteLine();
            }
            do {
                tryField = (uint[,])mainSudoku.board.Clone();
                uint tryNumber = mainSudoku.tryValue.Number;
                uint tryRow = mainSudoku.tryValue.Row;
                uint tryColumn = mainSudoku.tryValue.Column;
                for (int i = 0; i < tryNumber; i++) {
                    testNumber = GetNumber(mainSudoku.tryValue.Occupied);
                    tryField[tryRow, tryColumn] = (uint)testNumber;
                    if (fileOpenForAll) {
                        streamWriter.Write(TryInRowColumn);
                        streamWriter.WriteLine("[" + (tryRow + 1).ToString(CultureInfo.InvariantCulture) + ", "
                            + (tryColumn + 1).ToString(CultureInfo.InvariantCulture) + "] : " + testNumber.ToString(CultureInfo.InvariantCulture));
                    }
                    SolveHelper helperSudoku = new SolveHelper(this);
                    helperSudoku.SetStartValues(tryField);
                    if (helperSudoku.Calculate()) {
                        outVal = helperSudoku.GetResultValues();
                        result = true;
                    } else {
                        //Change variables in mainSudoku for next run
                        if (!CompareBoardFields(mainSudoku, helperSudoku)) {
                            BitSet.Exclude(ref mainSudoku.boardEstimation[tryRow, tryColumn].Occupied, testNumber);
                            mainSudoku.boardEstimation[tryRow, tryColumn].Number--;
                            streamString = TryNotOk;
                        } else {
                            field = (uint[,])helperSudoku.board.Clone();
                            fieldEstimation = (ValueType[,])helperSudoku.boardEstimation.Clone();
                            streamString = TryIsPossible;
                        }
                        if (fileOpenForAll) {
                            streamWriter.WriteLine(streamString);
                            streamWriter.WriteLine();
                        }
                        BitSet.Exclude(ref mainSudoku.tryValue.Occupied, testNumber);
                        if (mainSudoku.boardEstimation[tryRow, tryColumn].Number == 0) {
                            abort = true;
                        }
                        if (mainSudoku.boardEstimation[tryRow, tryColumn].Number == 1) {
                            mainSudoku.board = (uint[,])field.Clone();
                            mainSudoku.boardEstimation = (ValueType[,])fieldEstimation.Clone();
                        } else {
                            pass2Checked[tryRow, tryColumn] = true;
                        }
                    }
                    if (result || abort)
                        break; //leave for
                }
                if (!result && !abort) {
                    result = mainSudoku.Evaluation();
                    if (result) {
                        outVal = mainSudoku.GetResultValues();
                    }
                }
            }
            while (!(result || abort || m_abort));
            return result;
        }

        public bool Abort {
            get { return m_abort; }
            set { m_abort = value; }
        }

        public void Execute(Parameter parameter) {
            bool result;
            uint[,] outVal;
            SudokuError errorCode;
            result = Execute(parameter.InputValues, out outVal, parameter.Option, out errorCode);
            parameter.OutputValues = outVal;
            parameter.ErrorCode = errorCode;
            parameter.IsCalculated = result;
        }

        public bool Execute(uint[,] inVal, out uint[,] outVal, SudokuOption option, out SudokuError errorCode) {
            SolveHelper mainSudoku;

            bool result = false;
            pass2Checked.Initialize();
            m_smallField.Initialize();
            fileOpen = false;
            fileOpenForAll = false;
            if ((option & SudokuOption.WithFile) != 0) {
                fileOpen = true;
                if ((option & SudokuOption.AddonInFile) != 0) {
                    fileOpenForAll = true;
                }
            }
            outVal = (uint[,])inVal.Clone();
            errorCode = InputCheck(inVal);
            if (errorCode != 0) {
                return result;
            }
            mainSudoku = new SolveHelper(this);
            try {
                if (mainSudoku.SetStartValues(inVal)) {
                    if (fileOpen) {
                        string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + Path.DirectorySeparatorChar + "Sudoku";
                        if (!Directory.Exists(path)) {
                            Directory.CreateDirectory(path);
                        }
                        string fileName = path + Path.DirectorySeparatorChar + "SudokuSolver.txt";
                        streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                    }
                    result = mainSudoku.Calculate();
                }
                if (result) {
                    outVal = mainSudoku.GetResultValues();
                } else {
                    result = TryExecute(mainSudoku, ref outVal);
                    if (m_abort) {
                        if (fileOpen) {
                            streamWriter.WriteLine("Aborted");
                        }
                        errorCode = SudokuError.Aborted;
                    }
                }
                if (fileOpen) {
                    streamWriter.Flush();
                }
            }
            finally {
                if (fileOpen) {
                    streamWriter.Close();
                    fileOpen = false;
                }
                m_abort = false;
            }
            return result;
        }
    }
}
