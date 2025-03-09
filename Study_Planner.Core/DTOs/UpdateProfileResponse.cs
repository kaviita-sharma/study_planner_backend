using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Planner.Core.DTOs
{

    public class UpdateProfileRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public UserPreferenceDTO Preferences { get; set; }
    }

    public class UserPreferenceDTO
    {
        public string LearningStyle { get; set; }
        public string PreferredStudyTime { get; set; }
        public int StudySessionDuration { get; set; }
        public int BreakDuration { get; set; }
    }

}
