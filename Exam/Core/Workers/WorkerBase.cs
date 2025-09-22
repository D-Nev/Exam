using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exam.Interfaces;
using System.Threading.Tasks;

namespace Exam.Core.Workers

{
    public abstract class WorkerBase
    {
        protected readonly ILogger Log;

        protected WorkerBase(ILogger log)
        {
            Log = log;
        }
      
        public abstract Task RunAsync(CancellationToken ct);
    }
}
