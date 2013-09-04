using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MyJudge2
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
}
