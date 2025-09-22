using Exam.Core;
using Exam.Core.Workers;
using Exam.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Domain
{
    public class Dispatcher : WorkerBase, IDispatcher
    {
        private readonly IPassengerStop stop;
        private readonly Barrier phase;
        private readonly AutoResetEvent busEvent;
        private readonly SimulationConfig cfg;
        private readonly Random rnd = new Random();

        public Dispatcher(IPassengerStop stop,
                          Barrier phase,
                          AutoResetEvent busEvent,
                          SimulationConfig cfg,
                          ILogger log)
            : base(log)
        {
            this.stop = stop;
            this.phase = phase;
            this.busEvent = busEvent;
            this.cfg = cfg;
        }

        public override async Task RunAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                int delay = rnd.Next(cfg.PassengerWaveDelayMsMin, cfg.PassengerWaveDelayMsMax + 1);
                try { await Task.Delay(delay, ct); }
                catch (OperationCanceledException) { break; }

                int wave = rnd.Next(cfg.MinNewPassengers, cfg.MaxNewPassengers + 1);

                using (var done = new ManualResetEventSlim(false))
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        try { stop.AddPassengers(wave); }
                        finally { done.Set(); }
                    });

                    done.Wait(ct);
                }

                busEvent.Set();

                try { phase.SignalAndWait(ct); }
                catch (OperationCanceledException) { break; }
            }
        }
    }
}
