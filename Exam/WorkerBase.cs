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
        protected readonly ILogger Logger;

        private Thread thread;
        private volatile bool running;

        protected WorkerBase(ILogger logger)
        {
            Logger = logger;
        }

        protected abstract void Run();

        public void Start()
        {
            if (running) return;
            running = true;
            thread = new Thread(Run)
            {
                IsBackground = true,
                Name = GetType().Name
            };
            thread.Start();
        }

        public void Stop()
        {
            running = false;
        }

        protected bool IsRunning => running;
    }
}

