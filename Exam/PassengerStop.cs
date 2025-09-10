using System;
using System.Threading;

namespace Exam
{
    internal class PassengerStop : IPassengerStop
    {
        private readonly object lockObj = new object();
        private int waitingPassengers;
        private readonly Semaphore availablePassengers;

        private readonly SimulationConfig config;
        private readonly ILogger logger;

        public PassengerStop(SimulationConfig config, ILogger logger)
        {
            this.config = config;
            this.logger = logger;
            waitingPassengers = 0;
            availablePassengers = new Semaphore(0, int.MaxValue);
        }

        public int WaitingPassengers
        {
            get { lock (lockObj) return waitingPassengers; }
        }

        public void AddPassengers(int count)
        {
            if (count <= 0) return;

            lock (lockObj)
            {
                waitingPassengers += count;
                logger.Info($"На зупинку прибуло {count} пасажирів. Тепер їх {waitingPassengers}");
            }

            availablePassengers.Release(count);
        }

        public int BoardPassengers(int maxSeats)
        {
            if (maxSeats <= 0) return 0;

            int boarded = 0;

            for (int i = 0; i < maxSeats; i++)
            {
                if (availablePassengers.WaitOne(config.BoardWaitPerSeatMs))
                {
                    lock (lockObj)
                    {
                        if (waitingPassengers > 0)
                        {
                            waitingPassengers--;
                            boarded++;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            logger.Info($"Посаджено {boarded}. Залишилось {WaitingPassengers}");
            return boarded;
        }
    }
}
