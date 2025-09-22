using Exam.Core;
using Exam.Core.Workers;
using Exam.Infrastructure.Threading;
using Exam.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Domain
{
    public class Bus : WorkerBase, IBus
    {
        public int Number { get; }
        public int Capacity { get; }

        private readonly IPassengerStop stop;
        private readonly Barrier phase;
        private readonly AutoResetEvent busEvent;
        private readonly SimulationConfig cfg;

        public Bus(int number,
                   int capacity,
                   IPassengerStop stop,
                   Barrier phase,
                   AutoResetEvent busEvent,
                   SimulationConfig cfg,
                   ILogger log)
            : base(log)
        {
            Number = number;
            Capacity = capacity;
            this.stop = stop;
            this.phase = phase;
            this.busEvent = busEvent;
            this.cfg = cfg;
        }

        public override async Task RunAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await busEvent.WaitOneAsync(ct);

                if (ct.IsCancellationRequested) break;

                Log.Info($"Автобус №{Number} під’їхав.");
                int boarded = stop.BoardPassengers(Capacity);

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    int pct = Capacity == 0 ? 0 : boarded * 100 / Capacity;
                    Log.Info($"[TP] Завантаження: {boarded}/{Capacity} ({pct}%).");
                });

                try { phase.SignalAndWait(ct); }
                catch (OperationCanceledException) { break; }

                Log.Info($"Автобус №{Number} забрав {boarded}. На зупинці: {stop.WaitingPassengers}");

                try { await Task.Delay(cfg.BusDriveTimeMs, ct); }
                catch (OperationCanceledException) { break; }
            }
        }
    }
}
