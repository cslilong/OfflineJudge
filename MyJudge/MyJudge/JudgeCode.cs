using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Judge
{
    public class Log
    {

        /*
         * 错误日志记录文件
         */
        public static void write(String logMessage)
        {
            StreamWriter w = File.AppendText("error_log.txt");
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
            w.Flush();
            w.Close();
        }
    }

    public class Compiler
    {
        string CompilerName;
        string Arg;
        string wDirectory;
        public int Status = 10;

        public Compiler(string CompileCMD, string parameter, string workDirectory)
        {
            CompilerName = CompileCMD;
            Arg = parameter;
            wDirectory = workDirectory;
        }

        public void Run()
        {
            Process compile = new Process();
            
            compile.StartInfo.FileName = CompilerName;
            compile.StartInfo.Arguments = Arg;
            compile.StartInfo.CreateNoWindow = true;
            compile.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //compile.StartInfo.RedirectStandardOutput = true;
            compile.StartInfo.UseShellExecute = false;
            compile.StartInfo.WorkingDirectory = wDirectory;
            try
            {
                compile.Start();
            }
            catch (Exception e)
            {
                Status = -1;
                string error = "Error from compile.Start(): " + e.Message;
                Console.WriteLine(error);
                //Log.write(error);
                return;
            }


            //StreamReader sr = compile.StandardOutput;
            //StreamWriter sw = new StreamWriter(wDirectory + "\\CompileRes.txt");

            if (!compile.WaitForExit(5000))
            {
                compile.Kill();
                compile.WaitForExit();
                //sr.Close();
                //sw.Close();
                Status = -1;
                return;
            }

            while (!compile.HasExited) compile.Refresh();
            //string result;
            //while ((result = sr.ReadLine()) != null) sw.WriteLine(result);
            //sr.Close();
            //sw.Close();
            Status = compile.ExitCode;
        }
    }

    public class Runnr
    {
        string InputFile;
        string OutputFile;
        string RunFile;

        public int Status;
        Process testProcess;
        bool isRE;
        int tle;
        long mle;
        long workset = 0;
        public long memory;
        public int time;
        public static uint inputlines = 0;
        public static uint inputedlines = 0;

        public Runnr(string inf, string outf, string runf, int tl, long ml)
        {
            InputFile = inf;
            OutputFile = outf;
            RunFile = runf;
            tle = tl;
            mle = ml * 1024;
        }

        public void Run()
        {
            testProcess = new Process();
            this.testProcess.StartInfo.FileName = RunFile;
            this.testProcess.StartInfo.UseShellExecute = false;
            this.testProcess.StartInfo.RedirectStandardInput = true;
            this.testProcess.StartInfo.RedirectStandardOutput = true;
            this.testProcess.StartInfo.RedirectStandardError = false;
            this.testProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            this.isRE = false;

            try
            {
                this.testProcess.Start();
                //TimeSpan T = new TimeSpan(0, 0, 0, 0, tle);

                this.memory = 0;
                this.Status = 0;
                DateTime pre = DateTime.Now;
                DateTime now = DateTime.Now;
                int pretime = 0;

                Thread re1 = new Thread(new ThreadStart(this.Re_dwwin));
                re1.Start();
                Thread re2 = new Thread(new ThreadStart(this.Re_vsjitdebugger));
                re2.Start();

                Thread input = new Thread(new ThreadStart(this.Input));
                input.Start();
                Thread output = new Thread(new ThreadStart(this.Output));
                output.Start();

                while ((!this.testProcess.HasExited) && (!isRE))
                {
                    //代码进入临界区
                    Thread.BeginCriticalRegion();
                    if (!this.testProcess.HasExited)
                    {
                        this.testProcess.Refresh();
                        this.time = this.testProcess.TotalProcessorTime.Milliseconds;
                        now = DateTime.Now;
                        this.time += this.testProcess.TotalProcessorTime.Seconds * 1000;
                        this.workset = this.testProcess.WorkingSet64;
                    }
                    if (this.time > this.tle)
                    {
                        this.Status = 1;// status 0 ok; 1 tle; 2 mle; 3 re;
                        if (!this.testProcess.HasExited)
                        {
                            this.testProcess.Kill();
                            this.testProcess.WaitForExit();
                        }
                    }
                    if (workset > memory) memory = workset;
                    if (memory > mle)
                    {
                        this.Status = 2;// status 0 ok; 1 tle; 2 mle; 3 re;
                        if (!this.testProcess.HasExited)
                        {
                            this.testProcess.Kill();
                            this.testProcess.WaitForExit();
                        }
                    }
                    if (this.time > pretime)
                    {
                        pretime = this.time;
                        pre = now;
                        inputedlines = inputlines;
                    }
                    else
                    {
                        if (pre.AddSeconds(2 * this.tle / 1000) < now && inputedlines == inputlines)
                        {
                            this.Status = 1;// status 0 ok; 1 tle; 2 mle; 3 re;
                        }
                        if (pre.AddSeconds(10) < now && inputedlines < inputlines)
                        {
                            this.isRE = true;
                            this.Status = 3;// status 0 ok; 1 tle; 2 mle; 3 re;
                        }
                    }
                    Thread.EndCriticalRegion();
                    //代码退出临界区
                }
                if (!this.testProcess.HasExited) this.testProcess.Kill();

                this.testProcess.WaitForExit();
                // 等待所有线程退出
                while (input.IsAlive) ;
                while (re1.IsAlive) ;
                while (re2.IsAlive) ;
                while (output.IsAlive) ;
            }
            catch (Exception e)
            {
                Console.Write("Run Error:{0}", e.Message);
                if (!this.testProcess.HasExited)
                {
                    this.testProcess.Kill();
                    this.testProcess.WaitForExit();
                }
            }
        }

        public void Input()
        {
            StreamReader sr = new StreamReader(this.InputFile);
            StreamWriter sw = this.testProcess.StandardInput;
            string data;
            try
            {
                //Thread.BeginCriticalRegion();
                while (((data = sr.ReadLine()) != null) && (!this.testProcess.HasExited))
                {
                    sw.WriteLine(data);
                    inputlines++;
                }
                //Thread.EndCriticalRegion();
            }
            catch (Exception e)
            {
                string error = "Error from JudgeCode Input(): " + e.Message;
                Console.WriteLine(error);
                Log.write(error);
            }
            sr.Close();
            sw.Close();
        }

        public void Output()
        {
            StreamReader sr = this.testProcess.StandardOutput;
            StreamWriter sw = new StreamWriter(this.OutputFile);
            string data;
            while (!this.testProcess.HasExited)
            {
                while ((data = sr.ReadLine()) != null) sw.WriteLine(data);
            }
            while (((data = sr.ReadLine()) != null) && this.Status != 4) sw.WriteLine(data);
            sr.Close();
            sw.Close();
        }

        public void Re_dwwin()
        {
            Process[] process;
            while (!this.testProcess.HasExited && !(isRE))
            {
                process = Process.GetProcessesByName("dwwin");
                if (process.Length != 0)
                {
                    foreach (Process p in process)
                    {
                        try
                        {
                            Thread.BeginCriticalRegion();
                            if (!p.HasExited)
                            {

                                p.Kill();
                                p.WaitForExit(500);
                            }
                            Thread.EndCriticalRegion();
                        }
                        catch (Exception e)
                        {
                            string error = "Error from close Dwwin.exe :" + e.Message;
                            Console.WriteLine(error);
                            Log.write(error);
                            break;
                        }
                    }

                    this.isRE = true;
                    this.Status = 3;
                    return;
                }
            }
        }//  end Re dwwin

        public void Re_vsjitdebugger()
        {
            Process[] process;
            while (!this.testProcess.HasExited && !(isRE))
            {
                process = Process.GetProcessesByName("vsjitdebugger");
                if (process.Length != 0)
                {
                    foreach (Process p in process)
                    {
                        try
                        {
                            Thread.BeginCriticalRegion();
                            if (!p.HasExited)
                            {

                                p.Kill();
                                p.WaitForExit(500);
                            }
                            Thread.EndCriticalRegion();
                        }
                        catch (Exception e)
                        {
                            string error = "Error from close vsjitdebugger.exe :" + e.Message;
                            Console.WriteLine(error);
                            Log.write(error);
                            break;
                        }
                    }

                    this.isRE = true;
                    this.Status = 3;
                    return;
                }
            }
        }//  end Re_vsjitdebugger
    }

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

