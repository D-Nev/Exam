using System;
using System.Threading;

namespace Exam
{
    internal class Dispatcher : WorkerBase, IDispatcher
    {
        private readonly IPassengerStop stop;
        private readonly Barrier sync;
        private readonly AutoResetEvent busEvt;
        private readonly SimulationConfig cfg;
        private readonly Random rnd = new Random();

        public Dispatcher(IPassengerStop stop,
        Barrier sync,AutoResetEvent busEvt, SimulationConfig cfg, ILogger log)  : base(log)
        {
            this.stop = stop;
            this.sync = sync;
            this.busEvt = busEvt;
            this.cfg = cfg;
        }

        protected override void Run()
        {
            while (IsRunning)
            {
                int delay = rnd.Next(cfg.PassengerWaveDelayMsMin, cfg.PassengerWaveDelayMsMax + 1);
                Thread.Sleep(delay);
                if (!IsRunning) break;

                int wave = rnd.Next(cfg.MinNewPassengers, cfg.MaxNewPassengers + 1);

                using (var done = new ManualResetEventSlim(false))
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        try
                        {
                            stop.AddPassengers(wave);
                        }
                        finally
                        {
                            done.Set();
                        }
                    });

                    done.Wait();
                }

                busEvt.Set();
                sync.SignalAndWait();
            }
        }
    }
}
