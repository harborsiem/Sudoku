﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sudoku {
    public partial class AboutDialog : Form {
        public AboutDialog() {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
