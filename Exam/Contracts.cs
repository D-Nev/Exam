using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam
{
    internal interface ILogger
    {
        void Info(string message);
    }

    internal interface IPassengerStop
    {
        int WaitingPassengers { get; }
        void AddPassengers(int count);
        int BoardPassengers(int maxSeats);
    }

    internal interface IBus
    {
        int Number { get; }
        int Capacity { get; }
        void Start();
        void Stop();
    }

    internal interface IDispatcher
    {
        void Start();
        void Stop();
    }
}
