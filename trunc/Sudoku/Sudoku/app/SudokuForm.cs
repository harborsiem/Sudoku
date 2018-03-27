using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku {
    public partial class SudokuForm : Form {

        private const int MaxRowAndColumn = 9;
        private const int SudokuBoardLength = MaxRowAndColumn * MaxRowAndColumn;

        uint[] sudoku1 =  //Example at program start //only for testing
        {
            0,0,0, 0,0,0, 0,0,9,
            0,2,5, 0,0,8, 0,0,0,
            0,0,0, 4,9,0, 0,6,0,

            5,0,0, 0,0,0, 1,4,0,
            0,0,7, 0,0,0, 8,9,0,
            1,0,3, 0,0,0, 0,7,5,

            6,0,0, 0,2,0, 0,0,4,
            0,3,0, 0,0,4, 0,0,0,
            7,0,0, 0,6,0, 0,3,0
        };

        private List<TextBox> tbList;
        private uint[,] inputs = new uint[SudokuSolver.Max, SudokuSolver.Max];
        private SudokuSolver sudoku = new SudokuSolver();
        private volatile bool threadRunning;
        private bool outputsInTextboxes;

        public SudokuForm() {
            if (!this.DesignMode) {
                this.Font = SystemFonts.MessageBoxFont;
            }
            InitializeComponent();
            InitClickEvents();
            InitSudokuBoardList();
            InitSudokuBoard();
            ClearSudokuBoard();
            ClearInputsMatrix();
            //ToMatrix();  //only for testing
            ValuesToSudokuBoard(inputs);
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void InitClickEvents() {
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItem_Click);
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            this.solveToolStripMenuItem.Click += new System.EventHandler(this.SolveToolStripMenuItem_Click);
            this.solveFileToolStripMenuItem.Click += new System.EventHandler(this.SolveFileToolStripMenuItem_Click);
            this.helpToolStripMenuItem.Click += (delegate {
                MessageBox.Show(this, "No help available", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
            this.aboutToolStripMenuItem.Click += (delegate {
                new AboutDialog().ShowDialog(this);
            });
        }

        private void InitSudokuBoardList() {
            tbList = new List<TextBox>(SudokuBoardLength);
            tbList.Add(textBox1);
            tbList.Add(textBox2);
            tbList.Add(textBox3);
            tbList.Add(textBox4);
            tbList.Add(textBox5);
            tbList.Add(textBox6);
            tbList.Add(textBox7);
            tbList.Add(textBox8);
            tbList.Add(textBox9);
            tbList.Add(textBox10);
            tbList.Add(textBox11);
            tbList.Add(textBox12);
            tbList.Add(textBox13);
            tbList.Add(textBox14);
            tbList.Add(textBox15);
            tbList.Add(textBox16);
            tbList.Add(textBox17);
            tbList.Add(textBox18);
            tbList.Add(textBox19);
            tbList.Add(textBox20);
            tbList.Add(textBox21);
            tbList.Add(textBox22);
            tbList.Add(textBox23);
            tbList.Add(textBox24);
            tbList.Add(textBox25);
            tbList.Add(textBox26);
            tbList.Add(textBox27);
            tbList.Add(textBox28);
            tbList.Add(textBox29);
            tbList.Add(textBox30);
            tbList.Add(textBox31);
            tbList.Add(textBox32);
            tbList.Add(textBox33);
            tbList.Add(textBox34);
            tbList.Add(textBox35);
            tbList.Add(textBox36);
            tbList.Add(textBox37);
            tbList.Add(textBox38);
            tbList.Add(textBox39);
            tbList.Add(textBox40);
            tbList.Add(textBox41);
            tbList.Add(textBox42);
            tbList.Add(textBox43);
            tbList.Add(textBox44);
            tbList.Add(textBox45);
            tbList.Add(textBox46);
            tbList.Add(textBox47);
            tbList.Add(textBox48);
            tbList.Add(textBox49);
            tbList.Add(textBox50);
            tbList.Add(textBox51);
            tbList.Add(textBox52);
            tbList.Add(textBox53);
            tbList.Add(textBox54);
            tbList.Add(textBox55);
            tbList.Add(textBox56);
            tbList.Add(textBox57);
            tbList.Add(textBox58);
            tbList.Add(textBox59);
            tbList.Add(textBox60);
            tbList.Add(textBox61);
            tbList.Add(textBox62);
            tbList.Add(textBox63);
            tbList.Add(textBox64);
            tbList.Add(textBox65);
            tbList.Add(textBox66);
            tbList.Add(textBox67);
            tbList.Add(textBox68);
            tbList.Add(textBox69);
            tbList.Add(textBox70);
            tbList.Add(textBox71);
            tbList.Add(textBox72);
            tbList.Add(textBox73);
            tbList.Add(textBox74);
            tbList.Add(textBox75);
            tbList.Add(textBox76);
            tbList.Add(textBox77);
            tbList.Add(textBox78);
            tbList.Add(textBox79);
            tbList.Add(textBox80);
            tbList.Add(textBox81);
        }

        private void InitSudokuBoard() {
            TextBox tb;
            IEnumerator<TextBox> it = this.tbList.GetEnumerator();
            while (it.MoveNext()) {
                tb = (TextBox)it.Current;
                //tb.KeyPress += Tb_KeyPress;
                tb.KeyUp += Tb_KeyUp;
                tb.MaxLength = 1;
                //tb.Mask = "0";
                tb.TextAlign = HorizontalAlignment.Center;
                tb.Font = new Font(this.Font.Name, 12.0f, FontStyle.Regular);
            }
        }

        private void ClearSudokuBoard() {
            TextBox tb;
            IEnumerator<TextBox> it = this.tbList.GetEnumerator();
            while (it.MoveNext()) {
                tb = it.Current;
                tb.Text = string.Empty;
            }
        }

        private void ClearInputsMatrix() {
            Array.Clear(inputs, 0, inputs.Length);
        }

        private void ToMatrix() {  //only for testing
            uint row, column;
            uint i = 0;
            for (row = 0; row < SudokuSolver.Max; row++) {
                for (column = 0; column < SudokuSolver.Max; column++) {
                    inputs[row, column] = sudoku1[i];
                    i++;
                }
            }
            outputsInTextboxes = false;
        }

        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "0#")]
        public void ValuesToSudokuBoard(uint[,] matrix) {
            if (matrix.GetLength(0) != MaxRowAndColumn && matrix.GetLength(1) != MaxRowAndColumn) {
                throw new ArgumentException("Array length invalid", "matrix");
            }
            uint row = 0;
            uint column = 0;
            TextBox tb;
            IEnumerator<TextBox> it = this.tbList.GetEnumerator();
            while (it.MoveNext()) {
                tb = (TextBox)it.Current;
                uint value = matrix[row, column];
                if (value != 0) {
                    tb.Text = value.ToString(CultureInfo.InvariantCulture);
                }
                column++;
                if (column == SudokuSolver.Max) {
                    column = 0;
                    row++;
                }
            }
        }

        private void SudokuBoardToInputsMatrix() {
            uint row = 0;
            uint column = 0;
            char ch;
            TextBox tb;
            IEnumerator<TextBox> it = this.tbList.GetEnumerator();
            while (it.MoveNext()) {
                tb = (TextBox)it.Current;
                if (tb.Text.Length > 0) {
                    ch = Char.Parse(tb.Text);
                    if ((ch >= '1') && (ch <= '9')) {
                        inputs[row, column] = UInt32.Parse(tb.Text, CultureInfo.InvariantCulture);
                    } else {
                        ;// MessageBox.Show("Not a number in [" + row.ToString() + ", " + col.ToString() + "]");
                    }
                }
                column++;
                if (column == SudokuSolver.Max) {
                    column = 0;
                    row++;
                }
            }
        }

        private void AbortThreadCalculation() {
            if (threadRunning) {
                sudoku.Abort = true;
            }
            while (threadRunning) {
                Thread.Sleep(50);
            }
            sudoku.Abort = false;
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e) {
            AbortThreadCalculation();
            ClearSudokuBoard();
            ClearInputsMatrix();
            outputsInTextboxes = false;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e) {
            AbortThreadCalculation();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = "xml";
            dialog.FileName = "*.xml";
            dialog.Filter = "(*.xml)|*.xml";
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            if (dialog.ShowDialog() == DialogResult.OK) {
                ClearSudokuBoard();
                ClearInputsMatrix();
                try {
                    SudokuXmlReader.OpenSudokuFile(inputs, dialog.FileName);
                }
                catch (SudokuException ex) {
                    MessageBox.Show(ex.Message);
                    ClearSudokuBoard();
                    ClearInputsMatrix();
                }
                ValuesToSudokuBoard(inputs);
                ClearInputsMatrix();
                outputsInTextboxes = false;
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "xml";
            dialog.FileName = "*.xml";
            dialog.Filter = "(*.xml)|*.xml";
            if (dialog.ShowDialog() == DialogResult.OK) {
                if (!outputsInTextboxes) {
                    SudokuBoardToInputsMatrix();
                }
                SudokuXmlWriter.SaveInputs(inputs, dialog.FileName);
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) {
            AbortThreadCalculation();
            Application.Exit();
        }

        private void SolveToolStripMenuItem_Click(object sender, EventArgs e) {
            if (threadRunning) {
                return;
            }
            solveFileToolStripMenuItem.Enabled = false;
            SudokuBoardToInputsMatrix();
            this.Cursor = Cursors.WaitCursor;
            Parameter parameter = new Parameter(inputs, GetOption());
            Action<Parameter> executeAction = sudoku.Execute;
            threadRunning = true;
            executeAction.BeginInvoke(parameter,
                delegate (IAsyncResult ar) {
                    executeAction.EndInvoke(ar);
                    uint[,] outputs = parameter.OutputValues;
                    SudokuError errCode = parameter.ErrorCode;
                    this.Invoke((MethodInvoker)delegate {
                        if (errCode != SudokuError.None) {
                            MessageBox.Show(this, errCode.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        ValuesToSudokuBoard(outputs);
                        outputsInTextboxes = true;
                        solveFileToolStripMenuItem.Enabled = true;
                        this.Cursor = Cursors.Default;
                    });
                    threadRunning = false;
                }
                , null);
        }

        private SudokuOption GetOption() {
            if (!detailInfosToolStripMenuItem.Enabled) {
                return SudokuOption.None;
            }
            if (solveFileToolStripMenuItem.Checked) {
                if (detailInfosToolStripMenuItem.Checked) {
                    return SudokuOption.WithFileAddon;
                } else {
                    return SudokuOption.WithFile;
                }
            }
            return SudokuOption.None;
        }

        private void SolveFileToolStripMenuItem_Click(object sender, EventArgs e) {
            if ((sender as ToolStripMenuItem).Checked) {
                detailInfosToolStripMenuItem.Enabled = true;
            } else {
                detailInfosToolStripMenuItem.Enabled = false;
            }
        }

        private void Tb_KeyUp(object sender, KeyEventArgs e) {
            TextBox tb = sender as TextBox;
            if (e.KeyValue >= 0x31 && e.KeyValue <= 0x39) {
                tb.Select(0, 0);
                return;
            } else if (e.KeyValue < 0x30) {
                string tbNr = tb.Name.Substring("textBox".Length);
                int listIndex = Int32.Parse(tbNr, CultureInfo.InvariantCulture) - 1;
                int nextIndex;
                switch (e.KeyCode) {
                    case Keys.Right:
                        nextIndex = listIndex + 1;
                        this.ActiveControl = tbList[AdjustIndex(nextIndex)];
                        break;
                    case Keys.Left:
                        nextIndex = listIndex - 1;
                        this.ActiveControl = tbList[AdjustIndex(nextIndex)];
                        break;
                    case Keys.Up:
                        nextIndex = listIndex - 9;
                        this.ActiveControl = tbList[AdjustIndex(nextIndex)];
                        break;
                    case Keys.Down:
                        nextIndex = listIndex + 9;
                        this.ActiveControl = tbList[AdjustIndex(nextIndex)];
                        break;
                    default:
                        tb.Select(0, 0);
                        break;
                }
            } else {
                tb.Text = string.Empty;
            }
        }

        private int AdjustIndex(int idx) {
            if (idx >= 0 && idx < SudokuBoardLength) {
                return idx;
            } else if (idx < 0) {
                return idx + tbList.Count;
            } else {
                return idx - tbList.Count;
            }
        }

        private void Tb_KeyPress(object sender, KeyPressEventArgs e) {
            TextBox tb = sender as TextBox;
            if (e.KeyChar >= 0x31 && e.KeyChar <= 0x39) {
                return;
            } else {
                tb.Text = string.Empty;
            }
        }
    }
}