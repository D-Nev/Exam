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
        private readonly Barrier barrier;
        private readonly AutoResetEvent busCallEvent;
        private readonly SimulationConfig config;

        public Bus(int number,
                   int capacity,
                   IPassengerStop stop,
                   Barrier barrier,
                   AutoResetEvent busCallEvent,
                   SimulationConfig config,
                   ILogger logger)
            : base(logger)
        {
            Number = number;
            Capacity = capacity;
            this.stop = stop;
            this.barrier = barrier;
            this.busCallEvent = busCallEvent;
            this.config = config;
        }

        protected override void Run()
        {
            busCallEvent.Set();

            while (IsRunning)
            {
                busCallEvent.WaitOne();
                if (!IsRunning) break;

                Logger.Info($"Автобус №{Number} під’їхав.");
                int boarded = stop.BoardPassengers(Capacity);

                barrier.SignalAndWait();

                Logger.Info($"Автобус №{Number} забрав {boarded}. На зупинці: {stop.WaitingPassengers}");

                Thread.Sleep(config.BusDriveTimeMs);
            }
        }
    }
}
