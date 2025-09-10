using System;
using System.Threading;

namespace Exam
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new SimulationConfig
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

            ILogger logger = new ConsoleLogger();

            var busCallEvent = new AutoResetEvent(false);
            var barrier = new Barrier(2);

            IPassengerStop stop = new PassengerStop(config, logger);
            IDispatcher dispatcher = new Dispatcher(stop, barrier, busCallEvent, config, logger);
            IBus bus = new Bus(config.BusNumber, config.BusCapacity, stop, barrier, busCallEvent, config, logger);

            bus.Start();
            dispatcher.Start();

            logger.Info("Симуляція запущена. Натисніть Enter, щоб завершити");
            Console.ReadLine();

            bus.Stop();
            dispatcher.Stop();

            logger.Info("Готово, натисніть Enter для виходу");
            Console.ReadLine();
        }
    }
}
