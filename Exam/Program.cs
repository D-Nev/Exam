using System;
using System.Threading;
using System.Threading.Tasks;
using Exam.Core;
using Exam.Domain;
using Exam.Infrastructure.Logging;
using Exam.Interfaces;

namespace Exam
{
    internal class Program
    {
        static async Task Main(string[] args)
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

            var busEvent = new AutoResetEvent(false);
            var phase = new Barrier(2);

            IPassengerStop stop = new PassengerStop(cfg, log);
            IDispatcher disp = new Dispatcher(stop, phase, busEvent, cfg, log);
            IBus bus = new Bus(cfg.BusNumber, cfg.BusCapacity, stop, phase, busEvent, cfg, log);

            using var cts = new CancellationTokenSource();

            log.Info("Симуляція запущена. Натисніть Enter, щоб завершити...");
            var busTask = bus.RunAsync(cts.Token);
            var dispTask = disp.RunAsync(cts.Token);

            Console.ReadLine();
            cts.Cancel();                 
            busEvent.Set();               

            try { await Task.WhenAll(busTask, dispTask); }
            catch (OperationCanceledException) { /* нормальне завершення */ }

            log.Info("Готово. Натисніть Enter для виходу.");
            Console.ReadLine();
        }
    }
}
