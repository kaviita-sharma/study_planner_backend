using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.Core.DTOs
{
    public class TimeSlotDTO
    {
        public string Day { get; set; }
        public string StartTime { get; set; }  // Example: "12:00:00"
        public string EndTime { get; set; }    // Example: "14:00:00"
    }
}
