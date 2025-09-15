using System;
using System.Threading;

namespace Exam
{
    internal class PassengerStop : IPassengerStop
    {
        private readonly object gate = new object();
        private int waiting;
        private readonly Semaphore sem;

        private readonly SimulationConfig cfg;
        private readonly ILogger log;

        public PassengerStop(SimulationConfig cfg, ILogger log)
        {
            this.cfg = cfg;
            this.log = log;
            waiting = 0;
            sem = new Semaphore(0, int.MaxValue);
        }

        public int WaitingPassengers
        {
            get { lock (gate) return waiting; }
        }

        public void AddPassengers(int count)
        {
            if (count <= 0) return;

            lock (gate)
            {
                waiting += count;
                log.Info($"На зупинку прибуло {count} пасажирів. Тепер їх {waiting}");
            }

            sem.Release(count);
        }
        public int BoardPassengers(int maxSeats)
        {
            if (maxSeats <= 0) return 0;

            int boarded = 0;

            for (int i = 0; i < maxSeats; i++)
            {
                if (sem.WaitOne(cfg.BoardWaitPerSeatMs))
                {
                    lock (gate)
                    {
                        if (waiting > 0)
                        {
                            waiting--;
                            boarded++;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            log.Info($"Посаджено {boarded}. Залишилось {WaitingPassengers}");
            return boarded;
        }
    }
}
