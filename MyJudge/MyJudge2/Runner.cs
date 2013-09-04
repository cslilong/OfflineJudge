using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MyJudge2
{
    public class Runner
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

        public Runner(string inf, string outf, string runf, int tl, long ml)
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
}
