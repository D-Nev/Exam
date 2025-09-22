using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Interfaces
{
    public interface IPassengerStop
    {
        int WaitingPassengers { get; }
        void AddPassengers(int count);
        int BoardPassengers(int maxSeats);
    }
}
