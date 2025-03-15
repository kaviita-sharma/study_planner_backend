using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.Core.DTOs
{
    public class EnrichedStudyPlanDTO
    {
        public int UserId { get; set; }
        public UserPreferenceDTO Preferences { get; set; }
        public List<TimeSlotDTO> AvailableTimeSlots { get; set; }
        public List<SubjectDetailsDTO> Subjects { get; set; }
    }

}
