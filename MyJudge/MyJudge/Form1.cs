using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Judge;

namespace MyJudge
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string cmd = "cl.exe";
            string arg = "source.cpp";
            Compiler compiler = new Compiler(cmd, arg, ".");
            compiler.Run();

            MessageBox.Show("compile ok !!!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Runnr runner = new Runnr("./file/in.txt", "./file/out1.txt", "./source.exe", 1000, 65536);
            runner.Run();

            MessageBox.Show("run ok !!!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Compare compare = new Compare("./file/out1.txt", "./file/out.txt", 0);
            compare.Run();
            if (compare.Status == 0)
            {
                MessageBox.Show("AC");
            }
            else
            {
                MessageBox.Show("WA");
            }
        }
    }
}
