using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam
{
    internal class ConsoleLogger : ILogger
    {
        private readonly object gate = new object();

        public void Info(string message)
        {
            lock (gate)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
            }
        }
    }
}
