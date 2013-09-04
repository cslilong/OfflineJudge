using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MyJudge2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //this.scintilla1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintilla1.LineWrapping.VisualFlags = ScintillaNET.LineWrappingVisualFlags.End;
            //this.scintilla1.Location = new System.Drawing.Point(0, 0);
            this.scintilla1.ConfigurationManager.Language = "cpp";
            this.scintilla1.ConfigurationManager.Configure();
            this.scintilla1.Margins.Margin0.AutoToggleMarkerNumber = 0;
            this.scintilla1.Margins.Margin0.Width = 20;
            this.scintilla1.Margins.Margin1.AutoToggleMarkerNumber = 0;
            this.scintilla1.Margins.Margin1.IsClickable = true;
            this.scintilla1.Margins.Margin2.Width = 16;

            //this.scintilla2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintilla2.LineWrapping.VisualFlags = ScintillaNET.LineWrappingVisualFlags.End;
            //this.scintilla2.Location = new System.Drawing.Point(0, 0);
            this.scintilla2.ConfigurationManager.Language = "cpp";
            this.scintilla2.ConfigurationManager.Configure();
            this.scintilla2.Margins.Margin0.AutoToggleMarkerNumber = 0;
            this.scintilla2.Margins.Margin0.Width = 20;
            this.scintilla2.Margins.Margin1.AutoToggleMarkerNumber = 0;
            this.scintilla2.Margins.Margin1.IsClickable = true;
            this.scintilla2.Margins.Margin2.Width = 16;

            //this.scintilla3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintilla3.LineWrapping.VisualFlags = ScintillaNET.LineWrappingVisualFlags.End;
            //this.scintilla3.Location = new System.Drawing.Point(0, 0);
            this.scintilla3.ConfigurationManager.Language = "cpp";
            this.scintilla3.ConfigurationManager.Configure();
            this.scintilla3.Margins.Margin0.AutoToggleMarkerNumber = 0;
            this.scintilla3.Margins.Margin0.Width = 20;
            this.scintilla3.Margins.Margin1.AutoToggleMarkerNumber = 0;
            this.scintilla3.Margins.Margin1.IsClickable = true;
            this.scintilla3.Margins.Margin2.Width = 16;

            //this.scintilla4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintilla4.LineWrapping.VisualFlags = ScintillaNET.LineWrappingVisualFlags.End;
            //this.scintilla4.Location = new System.Drawing.Point(0, 0);
            this.scintilla4.ConfigurationManager.Language = "cpp";
            this.scintilla4.ConfigurationManager.Configure();
            this.scintilla4.Margins.Margin0.AutoToggleMarkerNumber = 0;
            this.scintilla4.Margins.Margin0.Width = 20;
            this.scintilla4.Margins.Margin1.AutoToggleMarkerNumber = 0;
            this.scintilla4.Margins.Margin1.IsClickable = true;
            this.scintilla4.Margins.Margin2.Width = 16;

            comboBox1.Items.Add("1秒");
            comboBox1.Items.Add("2秒");
            comboBox1.Items.Add("3秒");
            comboBox1.Items.Add("4秒");
            comboBox1.Items.Add("5秒");
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter("source.cpp");
            string src = scintilla1.Text;
            sw.WriteLine(src);
            sw.Flush();
            sw.Close();

            Compiler compiler = new Compiler("cl.exe", "source.cpp", ".");
            string compileResult = compiler.Compile();
            if (compiler.exitCode == -1)
            {
                MessageBox.Show("compile error！！！");
            }

            textBox1.Text = compileResult;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter("in.txt");
            string src = scintilla2.Text;
            sw.WriteLine(src);
            sw.Flush();
            sw.Close();

            Runner runner = new Runner("in.txt", "out1.txt", "source.exe", 1000, 65536);
            runner.Run();
            // status 0 ok; 1 tle; 2 mle; 3 re;
            if (runner.Status == -1)
            {
                MessageBox.Show("run error！！！");
                return;
            }

            StreamReader sr = File.OpenText("out1.txt");
            this.scintilla3.Text = "";
            string str;
            while ((str = sr.ReadLine()) != null)
            {
                //将读出的字符串在richTextBox1中显示;
                this.scintilla3.Text += str + "\r\n";
            }
            sr.Close();

            tabControl1.SelectedIndex = 1;

            this.textBox2.Text = runner.time + "";

            if (checkBox1.Checked)
            {
                if (this.scintilla3.Text == this.scintilla4.Text)
                {
                    MessageBox.Show("AC");
                }
                else
                {
                    MessageBox.Show("WA");
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //File  openFileDialog();
            OpenFileDialog openFiledDialog = new OpenFileDialog();
            openFiledDialog.InitialDirectory = ".";
            openFiledDialog.Filter = "文本文件|*.*|C#文件|*.cs|所有文件|*.*";
            openFiledDialog.FilterIndex = 1;
            if (openFiledDialog.ShowDialog() == DialogResult.OK)
            {
                //打开文件对话框中选择的文件名
                string fname = openFiledDialog.FileName;
                //创建从字符串进行读取的StringReader对象
                StreamReader sr = File.OpenText(fname);
                this.scintilla2.Text = "";
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    //将读出的字符串在richTextBox1中显示;
                    this.scintilla2.Text += str + "\r\n";
                }
                sr.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFiledDialog = new OpenFileDialog();
            openFiledDialog.InitialDirectory = ".";
            openFiledDialog.Filter = "文本文件|*.*|C#文件|*.cs|所有文件|*.*";
            openFiledDialog.FilterIndex = 1;
            if (openFiledDialog.ShowDialog() == DialogResult.OK)
            {
                //打开文件对话框中选择的文件名
                string fname = openFiledDialog.FileName;
                //创建从字符串进行读取的StringReader对象
                StreamReader sr = File.OpenText(fname);
                this.scintilla4.Text = "";
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    //将读出的字符串在richTextBox1中显示;
                    this.scintilla4.Text += str + "\r\n";
                }
                sr.Close();
            }
        }
    }
}
