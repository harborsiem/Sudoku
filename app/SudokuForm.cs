using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Sudoku {
    public partial class SudokuForm : Form {

        uint[] sudoku1 =  //Example at program start
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

        private List<TextBox> tbList = new List<TextBox>(81);
        private uint[,] matrix = new uint[SudokuSolver.Max, SudokuSolver.Max];
        private uint[,] outmatrix = new uint[SudokuSolver.Max, SudokuSolver.Max];
        private SudokuError errCode;
        private SudokuOption option;
        private SudokuSolver sudoku = new SudokuSolver();
        private bool threadRunning;
        private bool matrixCleared = true;
        private Thread solveThread;
        public delegate void RunCallback();
        private object m_lock = new object();
        private object t_lock = new object();
        private RunCallback runDelegate1;
        private RunCallback runDelegate2;

        public SudokuForm() {
            InitializeComponent();
            FillTextBoxList();
            SetTextBoxMask();
            ClearTextBoxes();
            ClearMatrix();
            //ToMatrix();
            MatrixToTextBox(matrix);
            InitDelegates();
        }

        private void FillTextBoxList() {
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

        private int AdjustIndex(int idx) {
            if (idx >= 0 && idx < 81) {
                return idx;
            } else if (idx < 0) {
                return idx + tbList.Count;
            } else {
                return idx - tbList.Count;
            }
        }

        private void tb_KeyUp(object sender, KeyEventArgs e) {
            TextBox tb = sender as TextBox;
            if (e.KeyValue >= 0x31 && e.KeyValue <= 0x39) {
                tb.Select(0, 0);
                return;
            } else if (e.KeyValue < 0x30) {
                string tbNr = tb.Name.Substring("textBox".Length);
                int listIndex = Int32.Parse(tbNr) - 1;
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
                tb.Text = "";
            }
        }

        private void tb_KeyPress(object sender, KeyPressEventArgs e) {
            TextBox tb = sender as TextBox;
            if (e.KeyChar >= 0x31 && e.KeyChar <= 0x39) {
                return;
            } else {
                tb.Text = "";
            }
        }

        private void SetTextBoxMask() {
            TextBox tb;
            IEnumerator<TextBox> it = this.tbList.GetEnumerator();
            while (it.MoveNext()) {
                tb = (TextBox)it.Current;
                //tb.KeyPress += tb_KeyPress;
                tb.KeyUp += tb_KeyUp;
                tb.MaxLength = 1;
                //tb.Mask = "0";
                tb.TextAlign = HorizontalAlignment.Center;
            }
        }

        private void ToMatrix() {
            uint row, col;
            uint i = 0;
            for (row = 0; row < SudokuSolver.Max; row++) {
                for (col = 0; col < SudokuSolver.Max; col++) {
                    matrix[row, col] = sudoku1[i];
                    i++;
                }
            }
            matrixCleared = false;
        }

        private void MatrixToTextBox(uint[,] matrix) {
            uint row = 0;
            uint col = 0;
            TextBox tb;
            IEnumerator<TextBox> it = this.tbList.GetEnumerator();
            while (it.MoveNext()) {
                tb = (TextBox)it.Current;
                uint value = matrix[row, col];
                if (value != 0) {
                    tb.Text = value.ToString();
                }
                col++;
                if (col == SudokuSolver.Max) {
                    col = 0;
                    row++;
                }
            }
        }

        private void TextBoxToMatrix() {
            uint row = 0;
            uint col = 0;
            char ch;
            TextBox tb;
            IEnumerator<TextBox> it = this.tbList.GetEnumerator();
            while (it.MoveNext()) {
                tb = (TextBox)it.Current;
                if (tb.Text.Length > 0) {
                    ch = Char.Parse(tb.Text);
                    if ((ch >= '1') && (ch <= '9')) {
                        matrix[row, col] = UInt32.Parse(tb.Text);
                    } else {
                        ;// MessageBox.Show("Not a number in [" + row.ToString() + ", " + col.ToString() + "]");
                    }
                }
                col++;
                if (col == SudokuSolver.Max) {
                    col = 0;
                    row++;
                }
            }
            matrixCleared = false;
        }

        private void ClearTextBoxes() {
            TextBox tb;
            IEnumerator<TextBox> it = this.tbList.GetEnumerator();
            while (it.MoveNext()) {
                tb = (TextBox)it.Current;
                tb.Text = "";
            }
        }

        private void ClearMatrix() {
            Array.Clear(matrix, 0, matrix.Length);
            matrixCleared = true;
        }

        private void newtoolStripMenuItem_Click(object sender, EventArgs e) {
            lock (m_lock) {
                if (threadRunning && solveThread != null) {
                    sudoku.Abort = true;
                }
            }
            ClearTextBoxes();
            ClearMatrix();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = "xml";
            dialog.FileName = "*.xml";
            dialog.Filter = "(*.xml)|*.xml";
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            if (dialog.ShowDialog() == DialogResult.OK) {
                ClearTextBoxes();
                ClearMatrix();
                try {
                    SudokuXmlReader.OpenSudokuFile(matrix, dialog.FileName);
                }
                catch (SudokuException ex) {
                    MessageBox.Show(ex.Message);
                    ClearTextBoxes();
                    ClearMatrix();
                }
                MatrixToTextBox(matrix);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "xml";
            dialog.FileName = "*.xml";
            dialog.Filter = "(*.xml)|*.xml";
            if (dialog.ShowDialog() == DialogResult.OK) {
                if (matrixCleared) {
                    TextBoxToMatrix();
                }
                SudokuXmlWriter.SaveInputs(matrix, dialog.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            lock (m_lock) {
                if (threadRunning && solveThread != null) {
                    sudoku.Abort = true;
                }
            }
            Application.Exit();
        }

        private void InitDelegates() {
            runDelegate1 = (delegate {
                threadRunning = true;
                this.Cursor = Cursors.WaitCursor;
                solveFileToolStripMenuItem.Enabled = false;
                TextBoxToMatrix();
            });
            runDelegate2 = (delegate {
                if (errCode != SudokuError.None) {
                    //CodeProject.Dialog.MsgBox.Show(this, errCode.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MessageBox.Show(this, errCode.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                MatrixToTextBox(outmatrix);
                solveFileToolStripMenuItem.Enabled = true;
                this.Cursor = Cursors.Default;
                threadRunning = false;
            });
        }

        private void solveToolStripMenuItem_Click(object sender, EventArgs e) {
            if (threadRunning) {
                return;
            }
            ThreadPool.QueueUserWorkItem(delegate(object obj) {
                this.Invoke(runDelegate1);
                lock (t_lock) {
                    solveThread = Thread.CurrentThread;
                    sudoku.Execute(matrix, out outmatrix, option, out errCode);
                    solveThread = null;
                }
                this.Invoke(runDelegate2);
            });
        }

        private void solveFileToolStripMenuItem_Click(object sender, EventArgs e) {
            if ((sender as ToolStripMenuItem).Checked) {
                detailInfosToolStripMenuItem.Enabled = true;
                if (detailInfosToolStripMenuItem.Checked) {
                    option = SudokuOption.WithFileAddon;
                } else {
                    option = SudokuOption.WithFile;
                }
            } else {
                detailInfosToolStripMenuItem.Enabled = false;
                option = SudokuOption.None;
            }
        }

        private void detailInfosToolStripMenuItem_Click(object sender, EventArgs e) {
            if ((sender as ToolStripMenuItem).Checked) {
                option = SudokuOption.WithFileAddon;
            } else {
                option = SudokuOption.WithFile;
            }
        }
    }
}