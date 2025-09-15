using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Exam
{
    internal abstract class WorkerBase
    {
        protected readonly ILogger Log;

        private Thread th;
        private volatile bool run;

        protected WorkerBase(ILogger log)
        {
            Log = log;
        }
        protected abstract void Run();

        public void Start()
        {
            if (run) return;
            run = true;
            th = new Thread(Run)
            {
                IsBackground = true,
                Name = GetType().Name
            };
            th.Start();
        }

        public void Stop()
        {
            run = false;
        }

        protected bool IsRunning => run;
    }
}

