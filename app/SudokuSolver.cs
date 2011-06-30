// =========================================================================
// Copyright (C) HaboDotNet 2008.  All Rights Reserved. Confidential
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

    public sealed class SudokuSolver {
        // #########################################################################
        // constants
        // #########################################################################

        public const uint Max = 9;

        private const uint Min = 1;
        private const uint CMax = 3; //(uint)Math.Sqrt(Max);

        private const uint MinInputValues = 15;
        private const string m_try = "**********Try*********";
        private const string m_tryNotOk = "*** Try not OK ***";
        private const string m_tryPossible = "*** Try is possible ***";
        private const string m_tryIn = "*** Try in [Row, Column] ";

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
            public uint Col; //tIndex
        }

        // #########################################################################
        // variables
        // #########################################################################

        private bool fileOpen;
        private bool fileOpenForAll;
        private StreamWriter streamWriter;
        private bool[,] pass2Checked = new bool[Max, Max]; //array[tIndex,tIndex];
        private int m_inputNo;
        private uint[] m_smallField = new uint[Max];
        private bool m_abort;

        // #########################################################################
        // class
        // #########################################################################

        private class SolveHelper {
            // #########################################################################
            // constants
            // #########################################################################

            //private const string m_possibleResult = "moegliche Ergebnisse [Zeile, Spalte]";
            //private const string m_evaluate = "Bewerten";
            //private const string m_rowTest = "Zeilentest";
            //private const string m_colTest = "Spaltentest";
            //private const string m_carreeTest = "Carreetest";
            //private const string m_input = "Eingabe:";
            //private const string m_result = "Ergebnis:";
            //private const string m_insert = "Fuege ";
            //private const string m_to = " in ";
            //private const string m_ein = " ein";
            //private const string m_row = "Zeile ";
            //private const string m_column = "Spalte ";
            //private const string m_carree = "Carree ";
            //private const string m_ready = " fertig";
            private const string m_possibleResult = "Possible results [Row, Column]";
            private const string m_evaluate = "Evaluate";
            private const string m_rowTest = "Testing rows";
            private const string m_colTest = "Testing columns";
            private const string m_carreeTest = "Testing carrees";
            private const string m_input = "Inputs:";
            private const string m_result = "Result:";
            private const string m_insert = "Insert ";
            private const string m_to = " to ";
            private const string m_ein = "";
            private const string m_row = "row ";
            private const string m_column = "column ";
            private const string m_carree = "carree ";
            private const string m_ready = " finished";

            // #########################################################################
            // variables
            // #########################################################################

            public uint[,] field = new uint[Max, Max]; // Playing field 
            //tField = array[tIndex,tIndex] of tValue;
            public ValueType[,] fieldEstimation = new ValueType[Max, Max]; //array[tIndex,tIndex] of ValueType;
            public TryValueType tryValue = new TryValueType();
            private bool m_reCalc;

            private uint[,] sudokuField = new uint[Max, Max]; //sudokuField : tField;
            private ValueType[] cols = new ValueType[Max]; //array[tIndex];
            private ValueType[] rows = new ValueType[Max]; //array[tIndex];
            private ValueType[] carrees = new ValueType[Max]; //array[tIndex];

            private SudokuSolver sudoku;
            //private bool m_startValuesSet;
            private int m_validationCounter;
            private int m_newValCalculated;

            // #########################################################################
            // functions
            // #########################################################################

            public SolveHelper(SudokuSolver sudoku)
                : base() {
                this.sudoku = sudoku;
            }

            public bool ReCalc {
                set { m_reCalc = value; }
            }

            public bool SetStartValues(uint[,] matrix) {
                uint row, col;
                uint value;
                bool result;
                result = true;
                for (row = 0; row < Max; row++) {
                    for (col = 0; col < Max; col++) {
                        value = matrix[row, col];
                        if ((value < 0) || (value > Max)) {
                            result = false;
                        }
                        sudokuField[row, col] = value;
                    }
                }
                //m_startValuesSet = result;
                return result;
            }

            public uint[,] GetResultValues() {
                return field;
            }

            private static uint CarreePos(uint aRow, uint aCol) {
                return ((aCol / CMax) + (CMax * (aRow / CMax)));
            }

            private void InsertValCol(uint aCol, uint value) {
                if ((value < Min) || (value > Max)) {
                    return;
                }
                BitSet.Include(ref cols[aCol].Occupied, (int)value);
                cols[aCol].Number++;
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

            private void InsertValue(uint aRow, uint aCol, uint value) {
                field[aRow, aCol] = value;
                InsertValCol(aCol, value);
                InsertValRow(aRow, value);
                InsertValCarree(CarreePos(aRow, aCol), value);
                m_newValCalculated++;
            }

            private void FieldOutput(uint[,] field) {
                uint row, col;
                if (sudoku.fileOpen) {
                    for (row = 0; row < Max; row++) {
                        for (col = 0; col < Max; col++) {
                            sudoku.streamWriter.Write(String.Format("{0,4}", field[row, col].ToString()));
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

            private ValueType Evaluate(uint aRow, uint aCol) {
                uint i;
                ValueType result = new ValueType();
                m_validationCounter++;
                result.Number = 0;
                result.Occupied = 0;
                if (field[aRow, aCol] == 0) {
                    result.Occupied = rows[aRow].Occupied | cols[aCol].Occupied |
                                  carrees[CarreePos(aRow, aCol)].Occupied;
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

            public bool FieldCheck() {
                uint row, col;
                ValueType estimate;
                uint i;
                bool result = true;
                tryValue.Number = 0;
                if (sudoku.fileOpenForAll) {
                    sudoku.streamWriter.WriteLine(m_possibleResult);
                }
                for (row = 0; row < Max; row++) {
                    for (col = 0; col < Max; col++) {
                        estimate = fieldEstimation[row, col];
                        if ((estimate.Number > tryValue.Number) && !(sudoku.pass2Checked[row, col])) {
                            tryValue.Number = estimate.Number;
                            tryValue.Occupied = estimate.Occupied;
                            tryValue.Row = row;
                            tryValue.Col = col;
                        }
                        if (sudoku.fileOpenForAll) {
                            StringBuilder possibleResults = new StringBuilder(32);
                            for (i = Min; i <= Max; i++) {
                                if (BitSet.IsInBitset(fieldEstimation[row, col].Occupied, (int)i)) {
                                    possibleResults.Append(i.ToString());
                                    possibleResults.Append(", ");
                                }
                            }
                            if (possibleResults.Length > 0) {
                                possibleResults.Length -= 2;
                                sudoku.streamWriter.WriteLine("[" + (row + 1).ToString() + ", " + (col + 1).ToString() + "] : "
                                   + possibleResults);
                            }
                        }
                        if ((estimate.Number != 0) || (field[row, col] == 0)) {
                            result = false;
                        }
                    }
                }
                return result;
            }

            private bool FieldEvaluation() {
                uint row, col, value;
                bool result;
                if (sudoku.fileOpenForAll) {
                    sudoku.streamWriter.WriteLine(m_evaluate);
                }
                do {
                    result = true;
                    for (row = 0; row < Max; row++) {
                        for (col = 0; col < Max; col++) {
                            if (!m_reCalc) {
                                fieldEstimation[row, col] = Evaluate(row, col);
                            }
                            if (fieldEstimation[row, col].Number == 1) {
                                result = false;
                                for (value = Min; value <= Max; value++) {
                                    if (BitSet.IsInBitset(fieldEstimation[row, col].Occupied, (int)value)) {
                                        if (sudoku.fileOpenForAll) {
                                            sudoku.streamWriter.WriteLine(m_insert + value.ToString() + m_to + "["
                                                + (row + 1).ToString() + "," + (col + 1).ToString() + "]" + m_ein);
                                        }
                                        InsertValue(row, col, value);
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

            private bool FieldEvaluationRows() {
                uint row, col;
                uint i;
                uint value;
                uint testSet;
                bool result;
                if (sudoku.fileOpenForAll) {
                    sudoku.streamWriter.WriteLine(m_rowTest);
                }
                result = true;
                for (row = 0; row < Max; row++) {
                    if (rows[row].Number < Max) {
                        for (col = 0; col < Max; col++) {
                            testSet = 0;
                            for (i = 0; i < Max; i++) {
                                if (i != col) {
                                    testSet = testSet | fieldEstimation[row, i].Occupied;
                                }
                            }
                            testSet = fieldEstimation[row, col].Occupied & (~(testSet));
                            value = SingleValueFromSet(testSet);
                            if (value != 0) {
                                if (sudoku.fileOpenForAll) {
                                    sudoku.streamWriter.WriteLine(m_insert + value.ToString() + m_to + "[" + (row + 1).ToString()
                                        + "," + (col + 1).ToString() + "]" + m_ein);
                                }
                                InsertValue(row, col, value);
                                result = false;
                            }
                        }
                    }    // if (rows[row].Number < Max)
                    else {
                        if (sudoku.fileOpenForAll) {
                            sudoku.streamWriter.WriteLine(m_row + (row + 1).ToString() + m_ready);
                        }
                    }
                }   // for row
                FieldEvaluation();
                return result;
            }

            private bool FieldEvaluationCols() {
                uint row, col;
                uint i;
                uint value;
                uint testSet;
                bool result;
                if (sudoku.fileOpenForAll) {
                    sudoku.streamWriter.WriteLine(m_colTest);
                }
                result = true;
                for (col = 0; col < Max; col++) {
                    if (cols[col].Number < Max) {
                        for (row = 0; row < Max; row++) {
                            testSet = 0;
                            for (i = 0; i < Max; i++) {
                                if (i != row) {
                                    testSet = testSet | fieldEstimation[i, col].Occupied;
                                }
                            }
                            testSet = fieldEstimation[row, col].Occupied & (~(testSet));
                            value = SingleValueFromSet(testSet);
                            if (value != 0) {
                                if (sudoku.fileOpenForAll) {
                                    sudoku.streamWriter.WriteLine(m_insert + value.ToString() + m_to + "[" + (row + 1).ToString()
                                        + "," + (col + 1).ToString() + "]" + m_ein);
                                }
                                InsertValue(row, col, value);
                                result = false;
                            }
                        }
                    } else {
                        if (sudoku.fileOpenForAll) {
                            sudoku.streamWriter.WriteLine(m_column + (col + 1).ToString() + m_ready);
                        }
                    }
                }
                FieldEvaluation();
                return result;
            }

            private bool FieldEvaluationCarrees() {
                uint row, col, carree;
                uint i, j, hRow, hCol;
                uint value;
                uint testSet = 0;
                bool result;
                if (sudoku.fileOpenForAll) {
                    sudoku.streamWriter.WriteLine(m_carreeTest);
                }
                result = true;
                for (carree = 0; carree < Max; carree++) {
                    row = (carree / CMax) * CMax;
                    col = (carree * CMax) % Max;
                    if (carrees[carree].Number < Max) {
                        for (hRow = 0; hRow < CMax; hRow++) {
                            for (hCol = 0; hCol < CMax; hCol++) {
                                testSet = 0;
                                for (i = 0; i < CMax; i++) {
                                    for (j = 0; j < CMax; j++) {
                                        if (!((i == hRow) && (j == hCol)))
                                            testSet = testSet | fieldEstimation[row + i, col + j].Occupied;
                                    }
                                }
                                testSet = fieldEstimation[row + hRow, col + hCol].Occupied & ~testSet;
                                value = SingleValueFromSet(testSet);
                                if (value != 0) {
                                    if (sudoku.fileOpenForAll) {
                                        sudoku.streamWriter.WriteLine(m_insert + value.ToString() + m_to + "["
                                            + (row + hRow + 1).ToString() + "," + (col + hCol + 1).ToString() + "]" + m_ein);
                                    }
                                    InsertValue(row + hRow, col + hCol, value);
                                    result = false;
                                }
                            }
                        }
                    } else {
                        if (sudoku.fileOpenForAll) {
                            sudoku.streamWriter.WriteLine(m_carree + (carree + 1).ToString() + m_ready);
                        }
                    }
                }
                FieldEvaluation();
                return result;
            }

            public bool Evaluation() {
                bool ready;

                do {
                    m_newValCalculated = 0;
                    do { }
                    while (!FieldEvaluationRows());
                    do { }
                    while (!FieldEvaluationCols());
                    do { }
                    while (!FieldEvaluationCarrees());
                    ready = FieldCheck();
                }
                while (!(ready || (m_newValCalculated == 0)));
                return ready;
            }

            public bool Calculate() {
                uint row, col;
                bool ready;

                rows.Initialize();
                cols.Initialize();
                carrees.Initialize();
                field.Initialize();
                fieldEstimation.Initialize();
                if (sudoku.fileOpen) {
                    sudoku.streamWriter.WriteLine(m_input);
                    FieldOutput(sudokuField);
                }
                for (row = 0; row < Max; row++) {
                    for (col = 0; col < Max; col++) {
                        InsertValue(row, col, sudokuField[row, col]);
                    }
                }
                FieldEvaluation();
                m_validationCounter = 0;
                ready = Evaluation();
                if (sudoku.fileOpen) {
                    sudoku.streamWriter.WriteLine(m_result);
                    FieldOutput(field);
                }
                return ready;
            }
        }

        // #########################################################################
        // functions in class SudokuSolver
        // #########################################################################


        private bool CompareFields(SolveHelper mainSudoku, SolveHelper helperSudoku) {
            uint row, col;
            uint mainValue, helperValue; //tValue;
            bool result = true;
            for (row = 0; row < Max; row++) {
                for (col = 0; col < Max; col++) {
                    helperValue = helperSudoku.field[row, col];
                    if (helperValue != 0) {
                        mainValue = mainSudoku.field[row, col];
                        if (mainValue != 0) {
                            if (mainValue != helperValue) {
                                result = false;
                            }
                        } else {
                            if (!(BitSet.IsInBitset(mainSudoku.fieldEstimation[row, col].Occupied, (int)helperValue))) {
                                result = false;
                            }
                        }
                    } else {
                        if (helperSudoku.fieldEstimation[row, col].Number == 0) {
                            result = false;
                        }
                        if (!(pass2Checked[row, col])) {
                            if (!(mainSudoku.fieldEstimation[row, col].Occupied >= helperSudoku.fieldEstimation[row, col].Occupied)) {
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

        private bool CheckCol(uint aCol, uint[,] inVal) {
            uint i;

            for (i = 0; i < Max; i++) {
                m_smallField[i] = inVal[i, aCol];
            }
            return CheckSmallField();
        }

        private bool CheckCarree(uint carree, uint[,] inVal) {
            uint row, col;
            uint i, hRow, hCol;
            row = (carree / CMax) * CMax;
            col = (carree * CMax) % Max;
            i = 0;
            for (hRow = 0; hRow < CMax; hRow++) {
                for (hCol = 0; hCol < CMax; hCol++) {
                    m_smallField[i] = inVal[row + hRow, col + hCol];
                    i++;
                }
            }
            return CheckSmallField();
        }

        private bool CheckSmallField() {
            uint col, i;
            uint value;

            bool result = true;
            for (col = 0; col < Max; col++) {
                value = m_smallField[col];
                if ((col < Max) && (value > 0)) {
                    for (i = col + 1; i < Max; i++) {
                        if (value == m_smallField[i]) {
                            result = false;
                        }
                    }
                }
            }
            return result;
        }

        private SudokuError InputCheck(uint[,] inVal) {
            uint row, col, i;
            uint value;

            SudokuError result = SudokuError.None;
            m_inputNo = 0;
            for (row = 0; row < Max; row++) {
                for (col = 0; col < Max; col++) {
                    value = inVal[row, col];
                    if ((value < 0) || (value > Max)) {
                        result = SudokuError.WrongInput;
                    } else {
                        if (value > 0) {
                            m_inputNo++;
                        }
                    }
                }
            }
            if (m_inputNo < MinInputValues) {
                result = SudokuError.FewValues;
                return result;
            }
            for (i = 0; i < Max; i++) {
                if (!(CheckRow(i, inVal))) {
                    result = SudokuError.WrongInput;
                }
            }
            for (i = 0; i < Max; i++) {
                if (!(CheckCol(i, inVal))) {
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
                streamWriter.WriteLine(m_try);
                streamWriter.WriteLine();
            }
            do {
                tryField = (uint[,])mainSudoku.field.Clone();
                uint tryNumber = mainSudoku.tryValue.Number;
                uint tryRow = mainSudoku.tryValue.Row;
                uint tryCol = mainSudoku.tryValue.Col;
                for (int i = 0; i < tryNumber; i++) {
                    testNumber = GetNumber(mainSudoku.tryValue.Occupied);
                    tryField[tryRow, tryCol] = (uint)testNumber;
                    if (fileOpenForAll) {
                        streamWriter.Write(m_tryIn);
                        streamWriter.WriteLine("[" + (tryRow + 1).ToString() + ", "
                            + (tryCol + 1).ToString() + "] : " + testNumber.ToString());
                    }
                    SolveHelper helperSudoku = new SolveHelper(this);
                    helperSudoku.SetStartValues(tryField);
                    if (helperSudoku.Calculate()) {
                        outVal = helperSudoku.GetResultValues();
                        result = true;
                    } else {
                        //Change variables in mainSudoku for next run
                        if (!CompareFields(mainSudoku, helperSudoku)) {
                            BitSet.Exclude(ref mainSudoku.fieldEstimation[tryRow, tryCol].Occupied, testNumber);
                            mainSudoku.fieldEstimation[tryRow, tryCol].Number--;
                            streamString = m_tryNotOk;
                        } else {
                            field = (uint[,])helperSudoku.field.Clone();
                            fieldEstimation = (ValueType[,])helperSudoku.fieldEstimation.Clone();
                            streamString = m_tryPossible;
                        }
                        if (fileOpenForAll) {
                            streamWriter.WriteLine(streamString);
                            streamWriter.WriteLine();
                        }
                        BitSet.Exclude(ref mainSudoku.tryValue.Occupied, testNumber);
                        if (mainSudoku.fieldEstimation[tryRow, tryCol].Number == 0) {
                            abort = true;
                        }
                        if (mainSudoku.fieldEstimation[tryRow, tryCol].Number == 1) {
                            mainSudoku.field = (uint[,])field.Clone();
                            mainSudoku.fieldEstimation = (ValueType[,])fieldEstimation.Clone();
                        } else {
                            pass2Checked[tryRow, tryCol] = true;
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

        public bool Execute(uint[,] inVal, out uint[,] outVal, SudokuOption option, out SudokuError errorCode) {
            SolveHelper mainSudoku;

            bool result = false;
            pass2Checked.Initialize();
            fileOpen = false;
            fileOpenForAll = false;
            if ((option & SudokuOption.WithFile) != 0) {
                fileOpen = true;
                if ((option & SudokuOption.AddonInFile) != 0) {
                    fileOpenForAll = true;
                }
            }
            outVal = inVal;
            errorCode = InputCheck(inVal);
            if (errorCode != 0) {
                return result;
            }
            mainSudoku = new SolveHelper(this);
            try {
                if (mainSudoku.SetStartValues(inVal)) {
                    if (fileOpen) {
                        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + Path.DirectorySeparatorChar + "Sudoku";
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
