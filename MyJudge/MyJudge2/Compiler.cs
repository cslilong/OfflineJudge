using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics; // Process

namespace MyJudge2
{
    class Compiler
    {
        string file;
        string arg;
        string workdir;
        int timelimit;
        public int exitCode;

        public Compiler(string file, string arg, string workdir, int timelimit)
        {
            this.file = file;
            this.arg = arg;
            this.workdir = workdir;
            this.timelimit = timelimit;
            exitCode = 0;
        }

        public Compiler(string file, string arg, string workdir)
        {
            this.file = file;
            this.arg = arg;
            this.workdir = workdir;
            this.timelimit = 5000;
            exitCode = 0;
        }

        public string Compile()
        {
            Process process = new Process();
            process.StartInfo.FileName = file;
            process.StartInfo.Arguments = arg;
            process.StartInfo.WorkingDirectory = workdir;

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardOutput = true;

            try
            {
                process.Start();
            }
            catch (Exception e)
            {
                exitCode = -1;
                return null;
            }

            if (!process.WaitForExit(timelimit))
            {
                exitCode = -1;
                process.Kill();
                process.WaitForExit();
                return null;
            }
            while (!process.HasExited) process.Refresh();

            return process.StandardOutput.ReadToEnd();
        }

    }
}
