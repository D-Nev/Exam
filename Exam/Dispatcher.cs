using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam
{
    internal class Dispatcher : WorkerBase, IDispatcher
    {
        private readonly IPassengerStop stop;
        private readonly Barrier barrier;
        private readonly AutoResetEvent busCallEvent;
        private readonly SimulationConfig config;
        private readonly Random rnd = new Random();

        public Dispatcher(IPassengerStop stop,
                          Barrier barrier,
                          AutoResetEvent busCallEvent,
                          SimulationConfig config,
                          ILogger logger)
            : base(logger)
        {
            this.stop = stop;
            this.barrier = barrier;
            this.busCallEvent = busCallEvent;
            this.config = config;
        }

        protected override void Run()
        {
            while (IsRunning)
            {
                int delay = rnd.Next(config.PassengerWaveDelayMsMin, config.PassengerWaveDelayMsMax + 1);
                Thread.Sleep(delay);
                if (!IsRunning) break;

                int newPassengers = rnd.Next(config.MinNewPassengers, config.MaxNewPassengers + 1);
                stop.AddPassengers(newPassengers);
                busCallEvent.Set();  
                barrier.SignalAndWait();     
            }
        }
    }
}

