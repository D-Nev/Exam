using System;
using System.Threading;

namespace Exam
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cfg = new SimulationConfig
            {
                BusNumber = 150,
                BusCapacity = 4,
                MinNewPassengers = 1,
                MaxNewPassengers = 6,
                PassengerWaveDelayMsMin = 1200,
                PassengerWaveDelayMsMax = 3000,
                BusDriveTimeMs = 3000,
                BoardWaitPerSeatMs = 800
            };

            ILogger log = new ConsoleLogger();

            var busEvt = new AutoResetEvent(false);
            var sync = new Barrier(2);

            IPassengerStop stop = new PassengerStop(cfg, log);
            IDispatcher disp = new Dispatcher(stop, sync, busEvt, cfg, log);
            IBus bus = new Bus(cfg.BusNumber, cfg.BusCapacity, stop, sync, busEvt, cfg, log);

            bus.Start();
            disp.Start();

            log.Info("Симуляція запущена. Натисніть Enter, щоб завершити...");
            Console.ReadLine();

            bus.Stop();
            disp.Stop();
            busEvt.Set();
            log.Info("Готово. Натисніть Enter для виходу.");
            Console.ReadLine();
        }
    }
}
