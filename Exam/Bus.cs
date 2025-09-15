using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam
{
    internal class Bus : WorkerBase, IBus
    {
        public int Number { get; }
        public int Capacity { get; }

        private readonly IPassengerStop stop;
        private readonly Barrier sync;
        private readonly AutoResetEvent busEvt;
        private readonly SimulationConfig cfg;

        public Bus(int number,
                   int capacity,
                   IPassengerStop stop,
                   Barrier sync,
                   AutoResetEvent busEvt,
                   SimulationConfig cfg,
                   ILogger log)
            : base(log)
        {
            Number = number;
            Capacity = capacity;
            this.stop = stop;
            this.sync = sync;
            this.busEvt = busEvt;
            this.cfg = cfg;
        }

        protected override void Run()
        {
            while (IsRunning)
            {
                busEvt.WaitOne();
                if (!IsRunning) break;

                Log.Info($"Автобус №{Number} під’їхав.");
                int boarded = stop.BoardPassengers(Capacity);

                ThreadPool.QueueUserWorkItem(_ => {
                    int pct = Capacity == 0 ? 0 : boarded * 100 / Capacity;
                    Log.Info($"[TP] Завантаження: {boarded}/{Capacity} ({pct}%).");
                });
                sync.SignalAndWait();

                Log.Info($"Автобус №{Number} забрав {boarded}. На зупинці: {stop.WaitingPassengers}");

                Thread.Sleep(cfg.BusDriveTimeMs);
            }
        }
    }
}