using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exam.Interfaces;
using System.Threading.Tasks;

namespace Exam.Infrastructure.Logging
{
    public class ConsoleLogger : ILogger
    {      
        private static readonly object Gate = new object();
        private static readonly string TimeFmt = "HH:mm:ss";

        public void Info(string message)
        {
            lock (Gate)
            {
                Console.WriteLine($"[{DateTime.Now.ToString(TimeFmt)}] {message}");
            }
        }
    }
}
