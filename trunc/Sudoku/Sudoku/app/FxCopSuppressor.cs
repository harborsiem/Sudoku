﻿using System.Diagnostics.CodeAnalysis;


[module: SuppressMessage("Microsoft.Design", "CA1014:MarkAssembliesWithClsCompliant")]

[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuSolver+SolveHelper.#fieldEstimation", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuSolver+SolveHelper.#GetResultValues()", MessageId = "Return")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuSolver+SolveHelper.#field", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuSolver+SolveHelper.#SetStartValues(System.UInt32[,])", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuSolver+SolveHelper.#sudokuField", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuSolver+SolveHelper.#FieldOutput(System.UInt32[,])", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuForm.#inputs", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuForm.#outputs", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuForm.#MatrixToTextBox(System.UInt32[,])", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuXmlWriter.#SaveInputs(System.UInt32[,],System.String)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuXmlWriter.#SetInputs(System.UInt32[,],System.Xml.XmlDocument)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuSolver.#CheckCol(System.UInt32,System.UInt32[,])", MessageId = "1#")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuSolver.#CheckRow(System.UInt32,System.UInt32[,])", MessageId = "1#")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuSolver.#Execute(System.UInt32[,],System.UInt32[,]&,Sudoku.SudokuOption,Sudoku.SudokuError&)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuSolver.#CheckCarree(System.UInt32,System.UInt32[,])", MessageId = "1#")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuSolver.#pass2Checked", MessageId = "Member")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuSolver.#TryExecute(Sudoku.SudokuSolver+SolveHelper,System.UInt32[,]&)", MessageId = "Body")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuSolver.#InputCheck(System.UInt32[,])", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuXmlReader.#LoadInputs(System.Xml.XmlNode,System.UInt32[,])", MessageId = "1#")]
[module: SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", Scope = "member", Target = "Sudoku.SudokuXmlReader.#OpenSudokuFile(System.UInt32[,],System.String)", MessageId = "0#")]

[module: SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Scope = "member", Target = "Sudoku.SudokuForm.#Dispose(System.Boolean)", MessageId = "sudoku")]

[module: SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Scope = "member", Target = "Sudoku.SudokuForm.#openToolStripMenuItem_Click(System.Object,System.EventArgs)")]
[module: SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Scope = "member", Target = "Sudoku.SudokuForm.#InitDelegates()")]
[module: SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Scope = "member", Target = "Sudoku.SudokuForm.#helpToolStripMenuItem_Click(System.Object,System.EventArgs)")]

[module: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Scope = "member", Target = "Sudoku.BitSet.#Include(System.UInt16&,System.Int32)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Scope = "member", Target = "Sudoku.BitSet.#Include(System.UInt64&,System.Int32)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Scope = "member", Target = "Sudoku.BitSet.#Exclude(System.UInt16&,System.Int32)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Scope = "member", Target = "Sudoku.BitSet.#Exclude(System.UInt64&,System.Int32)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Scope = "member", Target = "Sudoku.BitSet.#Exclude(System.UInt32&,System.Int32)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Scope = "member", Target = "Sudoku.BitSet.#Exclude(System.Byte&,System.Int32)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Scope = "member", Target = "Sudoku.BitSet.#Include(System.UInt32&,System.Int32)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Scope = "member", Target = "Sudoku.BitSet.#Include(System.Byte&,System.Int32)", MessageId = "0#")]
[module: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "Sudoku.SudokuSolver.#Execute(System.UInt32[,],System.UInt32[,]&,Sudoku.SudokuOption,Sudoku.SudokuError&)", MessageId = "3#")]
[module: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "Sudoku.SudokuSolver.#Execute(System.UInt32[,],System.UInt32[,]&,Sudoku.SudokuOption,Sudoku.SudokuError&)", MessageId = "1#")]

[module: SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", Scope = "member", Target = "Sudoku.SudokuXmlWriter.#SaveDocument(System.Xml.XmlDocument,System.IO.FileInfo)", MessageId = "System.Xml.XmlNode")]
[module: SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Scope = "member", Target = "Sudoku.SudokuXmlWriter.#SaveDocument(System.Xml.XmlDocument,System.IO.FileInfo)")]
[module: SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", Scope = "member", Target = "Sudoku.SudokuXmlWriter.#CreateDocument()", MessageId = "System.Xml.XmlNode")]
