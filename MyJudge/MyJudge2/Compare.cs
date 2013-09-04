using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MyJudge2
{
    public class Compare
    {
        string inputfilePath;
        string ansfilePath;
        string indata;
        string ansdata;
        public int Status = 0;
        int Mode = 0;

        public Compare(string inPath, string ansPath, int M)
        {
            this.inputfilePath = inPath;
            this.ansfilePath = ansPath;
            this.Mode = M;
            this.indata = null;
            this.ansdata = null;
            this.Status = 0;
        }

        public void Run()
        {
            if (!File.Exists(inputfilePath))
            {
                //Console.Write("No such inputfile!");
                return;
            }
            if (!File.Exists(ansfilePath))
            {
                //Console.Write("No such ansfile!");
                return;
            }

            FileInfo infinfo = new FileInfo(inputfilePath);
            FileInfo ansfinfo = new FileInfo(ansfilePath);
            if (infinfo.Length >= 2 * ansfinfo.Length)
            {
                Status = 3;
                infinfo.Delete();
                return;
            }

            StreamReader ir = new StreamReader(inputfilePath);
            StreamReader ar = new StreamReader(ansfilePath);
            indata = ir.ReadLine();
            ansdata = ar.ReadLine();
            while (indata != null && ansdata != null)
            {
                if (Mode == 0)
                {
                    if (indata != ansdata) Status = 1;
                    if (Status == 1)
                    {
                        indata = indata.Replace(" ", "");
                        ansdata = ansdata.Replace(" ", "");
                        if (indata == ansdata)
                        {
                            Status = 4;
                        }
                    }
                }
                /*
                else if (Mode == 1)
                {
                    if (indata.Trim() != ansdata.Trim()) Status = 1;
                }
                else if (Mode == 2)
                {
                    while (indata == "") indata = ir.ReadLine();
                    while (ansdata == "") ansdata = ar.ReadLine();
                    if (indata != ansdata) Status = 1;
                }
                else if (Mode == 3)
                {
                    indata = indata.Trim();
                    ansdata = ansdata.Trim();
                    while (indata == "")
                    {
                        indata = ir.ReadLine();
                        indata = indata.Trim();
                    }
                    while (ansdata == "")
                    {
                        ansdata = ar.ReadLine();
                        ansdata = ansdata.Trim();
                    }
                    if (indata != ansdata) this.Status = 1;
                }
                 */
                indata = ir.ReadLine();
                ansdata = ar.ReadLine();
                if (Status != 0) break;
            }

            if (Status == 0)
            {
                while (indata != null && Status == 0)
                {
                    if (indata.Replace(" ", "") != "") Status = 1;
                    indata = ir.ReadLine();
                }
                while (ansdata != null && Status == 0)
                {
                    if (ansdata.Replace(" ", "") != "") Status = 1;
                    ansdata = ar.ReadLine();
                }
            }
            ir.Close();
            ar.Close();
        }

    }
}
