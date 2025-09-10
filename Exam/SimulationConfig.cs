using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam
{
    internal class SimulationConfig
    {
        public int BusNumber { get; set; } = 150;
        public int BusCapacity { get; set; } = 4;
        public int MinNewPassengers { get; set; } = 1;
        public int MaxNewPassengers { get; set; } = 6;
        public int PassengerWaveDelayMsMin { get; set; } = 1500;
        public int PassengerWaveDelayMsMax { get; set; } = 4000;
        public int BusDriveTimeMs { get; set; } = 4000;
        public int BoardWaitPerSeatMs { get; set; } = 1000;
    }
}


