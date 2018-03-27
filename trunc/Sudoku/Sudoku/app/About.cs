using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection;

namespace Sudoku {
    public partial class AboutDialog : Form {
        private const String HeaderLine0 = "Sudoku-Solver, Version ";
        private const String DescriptionLine0 = "Insert values in the text-boxes and press the \"Solve\" menu-button. The program calculates the board if possible.";
        private const String DescriptionLine1 = "If you set the Option \"SolveFile\" the calculation was shown at the Desktop in Directory Sudoku.";

        public AboutDialog() {
            if (!this.DesignMode) {
                this.Font = SystemFonts.MessageBoxFont;
            }
            InitializeComponent();
            string[] lines = new string[5];
            lines[0] = HeaderLine0 + AssemblyFileVersion;
            lines[1] = AssemblyCopyright;
            lines[2] = String.Empty;
            lines[3] = DescriptionLine0;
            lines[4] = DescriptionLine1;
            aboutText.Lines = lines;
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            this.Close();
        }

        public string DisplayAssemblyVersion {
            get {
                return String.Format(CultureInfo.CurrentCulture, HeaderLine0 + AssemblyVersion);
            }
        }

        private string AssemblyVersion {
            get {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        private string AssemblyFileVersion {
            get {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
                if (attributes.Length == 0) {
                    return string.Empty;
                }
                return ((AssemblyFileVersionAttribute)attributes[0]).Version;
            }
        }

        private string AssemblyCopyright {
            get {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0) {
                    return string.Empty;
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
    }
}
